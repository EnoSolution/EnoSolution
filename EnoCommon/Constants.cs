using System;
using System.Collections.Generic;
using System.Text;

namespace EnoCommon
{
    public static class Constant
    {
        // Note that some constants are stil not used

        public static readonly Dictionary<string, string> PACKET_TYPE = new Dictionary<string, string> 
        {
            { "00", "Reserved" },
            { "01", "Radio Telegram" },
            { "02", "Response to any packet" },
            { "03", "Radio Subtelegram" },
            { "04", "Event Message" },
            { "05", "Common Command" },
            { "06", "Smart Ack Command" },
            { "07", "Remote Management Command" },
            { "08", "Reserved for EnOcean" },
            { "09", "Radio Message" },
            { "0A", "ERP2 Protocol Radio Message" }
        };

        public static readonly Dictionary<string, string> RORG = new Dictionary<string, string> 
        {
            { "F6", "Repeated switch (RPS)" },
            { "D5", "Digital Data (1BS)" },
            { "A5", "Analog Data (4BS)" },
            { "D2", "Variable Length Data (VLD" }
        };

        public static readonly Dictionary<string, string> RPS = new Dictionary<string, string> 
        {
            { "00", "Switch(es) Released" },
            { "10", "Switch A Pressed Down" },
            { "30", "Switch A Pressed Up" },
            { "50", "Switch B Pressed Down" }, 
            { "70", "Switch B Pressed Up" }, 
            { "15", "Switches A & B Pressed Down" }, 
            { "37", "Switches A & B Pressed Up" }
        };

        public static readonly Dictionary<string, string> RET = new Dictionary<string, string> 
        {
            { "00", "RET_OK" }, 
            { "01", "RET_ERROR" },
            { "02", "RET_NOT_SUPPORTED" },
            { "03", "RET_WRONG_PARAM" },
            { "04", "RET_OPERATION_DENIED" }, 
            { "05", "RET_LOCK_SET" },
            { "06", "RET_BUFFER_TO_SMALL" }, 
            { "07", "RET_NO_FREE_BUFFER" }
        };

        public static readonly Dictionary<string, string> CO = new Dictionary<string, string> 
        {
            { "03", "CO_RD_VERSION" },
            { "08", "CO_RD_IDBASE" }
        };

        public static readonly Dictionary<string, string> MANUFACTURER = new Dictionary<string, string> 
        {
            { "000", "MANUFACTURER_RESERVED" },
            { "001", "PEHA" },
            { "002", "THERMOKON" }, 
            { "003", "SERVODAN" },
            { "004", "ECHOFLEX_SOLUTIONS" }, 
            { "005", "OMNIO_AG" }, 
            { "006", "HARDMEIER_ELECTRONICS" },
            { "007", "REGULVAR_INC" },
            { "008", "AD_HOC_ELECTRONICS" },
            { "009", "DISTECH_CONTROLS" }, 
            { "00A", "KIEBACK_AND_PETER" }, 
            { "00B", "ENOCEAN_GMBH" },
            { "00C", "PROBARE" },
            { "00D", "ELTAKO" }, 
            { "00E", "LEVITON" },
            { "00F", "HONEYWELL" },
            { "010", "SPARTAN_PERIPHERAL_DEVICES" }, 
            { "011", "SIEMENS" }, 
            { "012", "T_MAC" }, 
            { "013", "RELIABLE_CONTROLS_CORPORATION" },
            { "014", "ELSNER_ELEKTRONIK_GMBH" }, 
            { "015", "DIEHL_CONTROLS" }, 
            { "016", "BSC_COMPUTER" }, 
            { "017", "S_AND_S_REGELTECHNIK_GMBH" },
            { "018", "MASCO_CORPORATION" }, 
            { "019", "INTESIS_SOFTWARE_SL" }, 
            { "01A", "VIESSMANN" }, 
            { "01B", "LUTUO_TECHNOLOGY" },
            { "01C", "SCHNEIDER_ELECTRIC" }, 
            { "01D", "SAUTER" }, 
            { "01E", "BOOT_UP" }, 
            { "01F", "OSRAM_SYLVANIA" },
            { "020", "UNOTECH" }, 
            { "021", "DELTA_CONTROLS_INC" }, 
            { "022", "UNITRONIC_AG" }, 
            { "023", "NANOSENSE" },
            { "024", "THE_S4_GROUP" }, 
            { "025", "MSR_SOLUTIONS" }, 
            { "026", "GE" }, 
            { "027", "MAICO" },
            { "028", "RUSKIN_COMPANY" },
            { "029", "MAGNUM_ENERGY_SOLUTIONS" }, 
            { "02A", "KMC_CONTROLS" }, 
            { "02B", "ECOLOGIX_CONTROLS" },
            { "02C", "TRIO_2_SYS" }, 
            { "02D", "AFRISO_EURO_INDEX" }, 
            { "030", "NEC_ACCESSTECHNICA_LTD" }, 
            { "031", "ITEC_CORPORATION" },
            { "032", "SIMICX_CO_LTD" },
            { "034", "EUROTRONIC_TECHNOLOGY_GMBH" },
            { "035", "ART_JAPAN_CO_LTD" }, 
            { "036", "TIANSU_AUTOMATION_CONTROL_SYSTE_CO_LTD" },
            { "038", "GRUPPO_GIORDANO_IDEA_SPA" }, 
            { "039", "ALPHAEOS_AG" }, 
            { "03A", "TAG_TECHNOLOGIES" }, 
            { "03C", "CLOUD_BUILDINGS_LTD" },
            { "03E", "GIGA_CONCEPT" },
            { "03F", "SENSORTEC" },
            { "040", "JAEGER_DIREKT" },
            { "041", "AIR_SYSTEM_COMPONENTS_INC" },
            { "7FF", "MULTI_USER_MANUFACTURER" }
        };
    }
}
