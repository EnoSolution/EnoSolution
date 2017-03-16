using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using EnoCommon;

namespace EnoGateway
{
    public partial class ServiceEnoGateway : ServiceBase
    {
        //
        // BASE_ID of the USB gateway
        // ==========================
        //
        private byte[] gwBaseId = new byte[4];
        //
        // Serial port buffer and pointer
        // ==============================
        //
        private byte[] recBuffer = new byte[4096];
        private int ptrBuffer = 0;
        //
        // UDP sender and listener
        // =======================
        //
        private UdpClient udpClientSender = new UdpClient();
        private IPEndPoint remoteEp;
        private UdpClient udpClientListener = new UdpClient();
        private IPEndPoint localEp;
        //
        // Tasks and Cancellation Tokens 
        // =============================
        //
        private Task[] tasks = new Task[5];
        private CancellationTokenSource tokenSourceTask0 = new CancellationTokenSource();
        private CancellationToken tokenTask0;
        private CancellationTokenSource tokenSourceTask1 = new CancellationTokenSource();
        private CancellationToken tokenTask1;
        private CancellationTokenSource tokenSourceTask2 = new CancellationTokenSource();
        private CancellationToken tokenTask2;
        private CancellationTokenSource tokenSourceTask3 = new CancellationTokenSource();
        private CancellationToken tokenTask3;
        private CancellationTokenSource tokenSourceTask4 = new CancellationTokenSource();
        private CancellationToken tokenTask4;
        //
        // Queues for inter tasks communication
        // ====================================
        //
        private ConcurrentQueue<string> serialToUdpQueue = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> udpToSerialQueue = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> archiveQueue = new ConcurrentQueue<string>();

        public ServiceEnoGateway()
        {
            InitializeComponent();
        }

        #region Start & Stop
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                WriteLog("*************************************************************");
                WriteInfo("EnoGateway v1.0.0 is starting");
                WriteInfo("Parameters:");
                WriteInfo("  Serial Port ......: " + EnoGateway.Properties.Settings.Default.SerialComPort);
                WriteInfo("  Multicast Address : " + EnoGateway.Properties.Settings.Default.MulticastAddress);
                WriteInfo("  Multicast Port ...: " + EnoGateway.Properties.Settings.Default.MulticastPort);
                WriteInfo("  Listening Port ...: " + EnoGateway.Properties.Settings.Default.ListeningPort);
                //
                // Serial Port (defined in app.config)
                // ===================================
                //
                serialPort.PortName = EnoGateway.Properties.Settings.Default.SerialComPort;
                serialPort.RtsEnable = true;
                serialPort.DtrEnable = true;
                serialPort.ReadTimeout = -1;
                serialPort.WriteTimeout = -1;
                serialPort.BaudRate = 57600;
                serialPort.DataBits = 8;
                serialPort.StopBits = System.IO.Ports.StopBits.One;
                serialPort.Parity = System.IO.Ports.Parity.None;
                serialPort.Handshake = System.IO.Ports.Handshake.None;
                serialPort.Open();
                serialPort.DiscardInBuffer();
                //
                // UDP sender
                // ==========
                //
                IPAddress multicastAddress = IPAddress.Parse(EnoGateway.Properties.Settings.Default.MulticastAddress);
                udpClientSender.JoinMulticastGroup(multicastAddress);
                remoteEp = new IPEndPoint(multicastAddress, EnoGateway.Properties.Settings.Default.MulticastPort);
                //
                // UDP listener
                // ============
                //
                localEp = new IPEndPoint(IPAddress.Any, EnoGateway.Properties.Settings.Default.ListeningPort);
                udpClientListener.Client.Bind(localEp);
                //
                // Ask the USB Gateway to learn Version and BASE_ID
                // ================================================
                //
                byte[] buf1 = { 0x55, 0x00, 0x01, 0x00, 0x05, 0x70, 0x03, 0x09 };       // CO_RD_VERSION
                serialPort.Write(buf1, 0, 8);
                byte[] buf2 = { 0x55, 0x00, 0x01, 0x00, 0x05, 0x70, 0x08, 0x38 };       // CO_RD_IDBASE
                serialPort.Write(buf2, 0, 8);
                //
                // Tasks to manage EnOcean messages
                // ================================
                //
                tokenTask0 = tokenSourceTask0.Token;
                tokenTask1 = tokenSourceTask1.Token;
                tokenTask2 = tokenSourceTask2.Token;
                tokenTask3 = tokenSourceTask3.Token;
                tokenTask4 = tokenSourceTask4.Token;
                tasks[0] = Task.Factory.StartNew(() => { ReceiveFromSerial(tokenTask0); });
                tasks[1] = Task.Factory.StartNew(() => { ReceiveFromUdp(tokenTask1); });
                tasks[2] = Task.Factory.StartNew(() => { SendToSerial(tokenTask2); });
                tasks[3] = Task.Factory.StartNew(() => { SendToUdp(tokenTask3); });
                tasks[4] = Task.Factory.StartNew(() => { Archive(tokenTask4); });
                Thread.Sleep(1000);
                WriteInfo("EnoGateway Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version + " is started");
            }
            catch (System.Exception ex)
            {
                WriteError("Initialization error - Serial Port: " + serialPort.PortName + " - Exception: " + ex.Message);
                System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                svc.Stop();
            }
        }

        /// <summary>
        /// Service end
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                WriteInfo("EnoGateway Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version + " is stopping");
                tokenSourceTask0.Cancel();
                tasks[0].Wait();
                tokenSourceTask1.Cancel();
                UdpClient udpClientStop = new UdpClient();
                udpClientStop.Send(Encoding.ASCII.GetBytes("STOP"), 4, "127.0.0.1", EnoGateway.Properties.Settings.Default.ListeningPort);
                tasks[1].Wait();
                tokenSourceTask2.Cancel();
                tasks[2].Wait();
                tokenSourceTask3.Cancel();
                tasks[3].Wait();
                tokenSourceTask4.Cancel();
                tasks[4].Wait();
            }
            catch (Exception ex)
            {
                WriteError("OnStop Exception - " + ex.Message);
            }
            finally
            {
                if (serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Close();
                    }
                    catch (System.Exception)
                    {
                        WriteError("Unable to close " + serialPort.PortName);
                    }
                }
                WriteInfo("EnoGateway Version " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version + " is stopped");
                WriteLog("*************************************************************");
            }
        }
        #endregion

        #region Receive from serial
        /// <summary>
        /// Receive bytes from the serial port associated to the EnOcean USB gateway and extract valid ESP3 frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// This code apply to EnOcean Serial Protocol 3.0 (ESP3)                       ESP3 Packet:    ----Sync Byte -------------------------
        ///                                                                                             ----Header-----------------------------
        /// As soon as a Sync-Byte (value 0x55) is identified,  the subsequent                          -------Data Length --------------------
        /// 4 byte-Header  is  compared with  the  corresponding  CRC8H value.                          ------ Optional Data Length -----------
        /// If the result is a match  the Sync-Byte is correct.  Consequently,                          -------Packet Type --------------------
        /// the ESP3 packet is detected properly  and the subsequent data will                          -------CRC8 Header --------------------
        /// be passed.  If the header does not match the CRC8H, the value 0x55                          ----Data ------------------------------
        /// does not correspond to a Sync-Byte. The next 0x55 withing the data                          ----Optional Data ---------------------
        /// stream is picked and the verification is repeated.                                          ----CRC8 Data -------------------------
        /// 
        private void ReceiveFromSerial(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    while (serialPort.BytesToRead > 0)
                    {
                        recBuffer[ptrBuffer] = (byte)serialPort.ReadByte();                 // Read one byte
                        if (ptrBuffer == 0 && recBuffer[ptrBuffer] != 0x55) return;         // Wait for the Sync Byte
                        if (ptrBuffer == 5)                                                 // Wait for the Header of the frame (6 bytes received)
                        {
                            if (CRC8.GetCRC8Header(recBuffer) != recBuffer[5])              // Bad CRC8H
                            {
                                ShiftRecBuffer();                                           // Shift left the header trying to find a new Sync Byte
                                return;
                            }
                        }
                        if (ptrBuffer >= 5)                                                 // CRC8H is Ok. Analyse the frame length
                        {
                            uint dataLength = recBuffer[1] * (uint)256 + recBuffer[2];
                            uint optionalDataLength = recBuffer[3];
                            if (ptrBuffer == (dataLength + optionalDataLength + 6))         // Wait for receiving the full frame
                            {
                                if (CRC8.GetCRC8Data(recBuffer, ptrBuffer) != recBuffer[ptrBuffer])    // Bad CRC8D
                                {
                                    ShiftRecBuffer();                                       // Shift left the frame trying to find a new Sync Byte
                                    return;
                                }
                                byte[] frame = new byte[ptrBuffer + 1];
                                for (int i = 0; i < (ptrBuffer + 1); i++)
                                {
                                    frame[i] = recBuffer[i];
                                    recBuffer[i] = 0;
                                }
                                serialToUdpQueue.Enqueue(Tools.ByteArrayToHexString(frame));
                                ptrBuffer = 0;
                            }
                            else
                            {
                                ptrBuffer++;
                            }
                        }
                        else
                        {
                            ptrBuffer++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteError("ReceiveFromSerial Exception: " + ex.Message);
                    System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                    svc.Stop();
                }
                System.Threading.Thread.Sleep(100);
            }
            WriteInfo("  Bye from task ReceiveFromSerial");
        }

        /// <summary>
        /// Shift left the frame trying to find a new Sync Byte
        /// </summary>
        private void ShiftRecBuffer()
        {
            while (ptrBuffer != 0)
            {
                for (int i = 0; i < ptrBuffer; i++)
                {
                    recBuffer[i] = recBuffer[i + 1];
                }
                if (recBuffer[0] == 0x55)
                {
                    ptrBuffer = 0;
                }
                else
                {
                    ptrBuffer--;
                }
            }
        }
        #endregion

        #region Receive from UDP
        /// <summary>
        /// Receive ESP3 frames from UDP listener
        /// </summary>
        private void ReceiveFromUdp(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Byte[] data = udpClientListener.Receive(ref localEp);
                    if (Encoding.ASCII.GetString(data) != "STOP")
                    {
                        string strData = Tools.ByteArrayToHexString(data);
                        udpToSerialQueue.Enqueue(strData);
                    }
                }
                catch (SocketException ex)
                {
                    WriteError("ReceiveFromUdp Exception: " + ex.Message);
                    System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                    svc.Stop();
                }
            }
            WriteInfo("  Bye from task ReceiveFromUdp");
        }
        #endregion

        #region Send to serial
        /// <summary>
        /// Send ESP3 frame to serial port associated to the EnOcean USB gateway
        /// 
        /// The sender address must be in the range 00.00.00.00 and 00.00.00.7F because the EnOcean gateway can emulate 128 sender addresses from the Base_Id
        /// For example, if the Base_Id is FF.56.2A.80 and the sender address is 00.0.00.46, the resultant address is FF.56.2A.C6
        /// </summary>
        private void SendToSerial(CancellationToken token)
        {
            string frame;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    while (udpToSerialQueue.TryDequeue(out frame))
                    {
                        WriteError("Frame : " + frame);
                        byte[] buffer = Tools.HexStringToByteArray(frame);
                        WriteError("Buffer : " + Tools.ByteArrayToHexString(buffer));
                        int dataLength = Convert.ToInt32(frame.Substring(3, 5).Replace("-", ""), 16);
                        WriteError("dataLength : " + dataLength.ToString());
                        buffer[6 + dataLength - 5] = gwBaseId[0];
                        buffer[7 + dataLength - 5] = gwBaseId[1];
                        buffer[8 + dataLength - 5] = gwBaseId[2];
                        buffer[9 + dataLength - 5] |= gwBaseId[3];     // Logical OR between the lower byte of the Base_Id and the lower byte of the sender address 
                        WriteError("Buffer : " + Tools.ByteArrayToHexString(buffer));
                        CRC8.SetAllCRC8(ref buffer);
                        archiveQueue.Enqueue(DateTime.Now.MyDateTimeFormat() + " [TX] " + Tools.ByteArrayToHexString(buffer));
                        serialPort.Write(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception ex)
                {
                    WriteError("SendToSerial Exception: " + ex.Message);
                    System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                    svc.Stop();
                }
                System.Threading.Thread.Sleep(100);
            }
            WriteInfo("  Bye from task SendToSerial");
        }
        #endregion

        #region Send to UDP
        /// <summary>
        /// Send ESP3 frame using UDP
        /// </summary>
        /// <param name="frame">The received frame</param>
        private void SendToUdp(CancellationToken token)
        {
            string frame;
            while (!token.IsCancellationRequested)
            {
                while (serialToUdpQueue.TryDequeue(out frame))
                {
                    try
                    {
                        if (frame.StartsWith("55-00-01-00-02-65-00-00"))        // RET_OK (Command is understood and triggered)
                        {
                        }
                        else if (frame.StartsWith("55-00-21-00-02-26-00"))     // // Response to CO_RD_VERSION
                        {
                            WriteInfo("EnOcean Gateway (USB300/USB310):");
                            WriteInfo("  App_Description ..: " + Tools.ByteArrayToString(Tools.HexStringToByteArray(frame.Substring(69, 47)), 0, 16));
                            WriteInfo("  App_Version ......: " + Tools.HexStringToByteArray(frame.Substring(21, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(24, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(27, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(30, 2))[0].ToString());
                            WriteInfo("  Api_Version ......: " + Tools.HexStringToByteArray(frame.Substring(33, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(36, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(39, 2))[0].ToString() + "." + Tools.HexStringToByteArray(frame.Substring(42, 2))[0].ToString());
                            WriteInfo("  Chip_Id ..........: " + frame.Substring(45, 11).Replace("-", "."));
                        }
                        else if (frame.StartsWith("55-00-05-01-02-DB-00"))     // Response to CO_RD_IDBASE (return the BASE ID of the USB Gateway)
                        {
                            WriteInfo("  Base_Id ..........: " + frame.Substring(21, 11).Replace("-", "."));
                            gwBaseId = Tools.HexStringToByteArray(frame.Substring(21, 12));
                        }
                        else
                        {
                            archiveQueue.Enqueue(DateTime.Now.MyDateTimeFormat() + " [RX] " + frame);
                            byte[] telegram = Tools.HexStringToByteArray(frame);
                            udpClientSender.Send(telegram, telegram.Length, remoteEp);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteError("SendToUdp Exception: " + ex.Message);
                        System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                        svc.Stop();
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
            WriteInfo("  Bye from task SendToUdp");
        }
        #endregion

        #region Write archive file
        /// <summary>
        /// Log messages
        /// </summary>
        /// <param name="message"></param>
        private void Archive(CancellationToken token)
        {
            string message;
            //string yesterday = "";
            while (!token.IsCancellationRequested)
            {
                while (archiveQueue.TryDequeue(out message))
                {
                    try
                    {
                        string today = message.Substring(0, 10); ;
                        using (StreamWriter sw = new StreamWriter(EnoGateway.Properties.Settings.Default.ArchiveFilePath + "Archive_" + today + ".log", true))
                        {
                            sw.WriteLine(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteError("  Archive Exception: " + ex.Message);
                        System.ServiceProcess.ServiceController svc = new System.ServiceProcess.ServiceController("EnoGateway");
                        svc.Stop();
                    }
                }
                System.Threading.Thread.Sleep(100);
            }
            WriteInfo("  Bye from task WriteLog");
        }
        #endregion

        #region write log file
        private void WriteLog(string message)
        {
            using (StreamWriter sw = new StreamWriter(EnoGateway.Properties.Settings.Default.ArchiveFilePath + "EnoGateway.log", true))
            {
                sw.WriteLine(message);
            }
        }

        void WriteError(string message)
        {
            WriteLog("!!!! " + DateTime.Now.MyDateTimeFormat() + " -> " + message);
        }

        void WriteInfo(string message)
        {
            WriteLog("**** " + DateTime.Now.MyDateTimeFormat() + " -> " + message);
        }
        #endregion
    }
}
