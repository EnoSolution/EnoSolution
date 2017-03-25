using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnoCommon
{
    public static class Tools
    {
        public static string MyDateTimeFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string MyDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        #region ByteArray to/from string
        /// <summary>
        /// Convert a byte array to an hexadecimal string with dash as separator (Example: F832A5 -> "F8-32-A5")
        /// </summary>
        /// <param name="buffer">Byte Array</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] buffer)
        {
            string hexStr = BitConverter.ToString(buffer);
            return hexStr;
        }

        /// <summary>
        /// Convert a portion of byte array to an hexadecimal string with dash as separator (Example: F832A5 -> "F8-32-A5")
        /// </summary>
        /// <param name="buffer">Byte Array</param>
        /// <param name="idx">Index of the first byte (0..n)</param>
        /// <param name="len">Number of bytes (1..n)</param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] buffer, int idx, int len)
        {
            byte[] newbuf = new byte[len];
            for (int i = 0; i < len; i++)
            {
                newbuf[i] = buffer[idx + i];
            }
            string hexStr = BitConverter.ToString(newbuf);
            return hexStr;
        }

        /// <summary>
        /// Convert a portion of byte array (Big-Endian notation) to an integer value (Example : 000A = 10)
        /// </summary>
        /// <param name="buffer">Byte Array</param>
        /// <param name="idx">Index of the first byte (0..n)</param>
        /// <param name="len">Number of bytes (1..n)</param>
        /// <returns></returns>
        public static int ByteArrayToInt(byte[] buffer, int idx, int len)
        {
            int pos = len;
            int value = 0;
            for (int i = idx; i < (idx + len); i++)
            {
                value = (value * 256) + buffer[i];
            }
            return value;
        }

        /// <summary>
        /// Convert a portion of byte array (Big-Endian notation) to an ASCII character string (Example : 656667 = "ABC")
        /// </summary>
        /// <param name="buffer">Byte Array</param>
        /// <param name="idx">Index of the first byte (0..n)</param>
        /// <param name="len">Number of bytes (1..n)</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] buffer, int idx, int len)
        {
            int pos = len;
            byte[] buf = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buf[i] = buffer[idx + i];
            }
            return Encoding.ASCII.GetString(buf);
        }

        /// <summary>
        /// Convert an hexadecimal string with separator to a byte array (Example: "FE-32-A5" -> F832A5)
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexStr)
        {
            int len = hexStr.Length / 3;
            if ((len * 3) < hexStr.Length) len++;
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buffer[i] = Convert.ToByte(hexStr.Substring(i * 3, 2), 16);
            }
            return buffer;
        }
        #endregion

        #region Reserved for future use
        public static string PacketTypeName(string packetType)
        {
            if (Constant.PACKET_TYPE.ContainsKey(packetType))
                return Constant.PACKET_TYPE[packetType];
            return "Unknown Packet Type";
        }

        public static string RORGName(string rorg)
        {
            if (Constant.RORG.ContainsKey(rorg))
                return Constant.RORG[rorg];
            return "Unknown RORG";
        }

        public static string RPSName(string rps)
        {
            if (Constant.RPS.ContainsKey(rps))
                return Constant.RPS[rps];
            return "Unknown RPS";
        }

        public static string ReturnCodeName(string ret)
        {
            if (Constant.RET.ContainsKey(ret))
                return Constant.RET[ret];
            return "Unknown Return Code";
        }

        public static string ReturnCommandName(string co)
        {
            if (Constant.CO.ContainsKey(co))
                return Constant.CO[co];
            return "Unknown Common Command Code";
        }
        #endregion
    }
}


