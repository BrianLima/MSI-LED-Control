using Microsoft.Win32;
using SupportData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportModule
{
    public static class CCLED
    {
        private static string ClassName = "CCLED";
        public static string CurrentStatus = "";
        private static string JLEDStatusBuffer = "";
        public static byte[] LEDStatus = new byte[3];

        public static bool AllSwitch
        {
            get
            {
                return (int)CRegistry.GetKeyBinraryValue("LED", "LEDStatus")[0] != 2;
            }
            set
            {
                CCLED.LEDStatus[0] = value ? (byte)1 : (byte)2;
                CRegistry.SetKeyValue("LED", "LEDStatus", (object)CCLED.LEDStatus, RegistryValueKind.Binary);
            }
        }

        public static string JLEDStatus
        {
            get
            {
                return Convert.ToString(CRegistry.GetKeyBinraryValue("LED", "LEDStatus")[1]);
            }
            set
            {
                if (!byte.TryParse(value, out CCLED.LEDStatus[1]))
                    CCLED.LEDStatus[1] = (byte)0;
                CRegistry.SetKeyValue("LED", "LEDStatus", (object)CCLED.LEDStatus, RegistryValueKind.Binary);
            }
        }

        public static string JLEDQueue
        {
            get
            {
                return Convert.ToString(CRegistry.GetKeyBinraryValue("LED", "LEDStatus")[2]);
            }
            set
            {
                if (!byte.TryParse(value, out CCLED.LEDStatus[2]))
                    CCLED.LEDStatus[2] = (byte)0;
                CRegistry.SetKeyValue("LED", "LEDStatus", (object)CCLED.LEDStatus, RegistryValueKind.Binary);
            }
        }

        public static bool NotifyJLEDChange()
        {
            if (!CCLED.JLEDStatusBuffer.Equals(CCLED.JLEDStatus))
            {
                CCLED.JLEDStatusBuffer = CCLED.JLEDStatus;
                if (!CCLED.JLEDStatus.Equals("0"))
                    return true;
            }
            return false;
        }

        public static void NotifyChange()
        {
            if (DataCenter.FN_LED)
            {
                if (CLEDParser.List_PartItem.Count > 0)
                {
                    string str1 = string.Empty;
                    foreach (PartItem partItem in CLEDParser.List_PartItem)
                        str1 = str1 + partItem.ShowName + (partItem.DeviceName[0].Equals((object)EnumDeviceName.MB_Mystic_Light_NoIcon) ? "_NoIcon" : "") + "|";
                    if (str1.Length > 0 && str1.LastIndexOf('|') == str1.Length - 1)
                        str1 = str1.Remove(str1.Length - 1);
                    if (CLEDParser.SelectIndex < 0 || CLEDParser.SelectIndex >= CLEDParser.List_PartItem.Count)
                        CLEDParser.SelectIndex = 0;
                    string str2 = "0";
                    string str3 = CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect ? "T" : "F";
                    if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex > 0 || CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI == 2)
                        str3 = "F";
                    string str4 = CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex <= 0 ? (CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect ? (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendStyle.SelectIndex <= 0 ? (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI != 2 ? Convert.ToString(CLEDParser.List_PartItem[CLEDParser.SelectIndex].Style.SelectIndex) : "-1") : "0") : "-1") : "-1";
                    if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ShowColorRGB)
                    {
                        if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].Style.SelectIndex == 0 && CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex == 0)
                        {
                            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ColorRGBStatus = false;
                            str2 = "0";
                        }
                        else
                            str2 = CLEDParser.List_PartItem[CLEDParser.SelectIndex].ColorRGBStatus ? "1" : "2";
                    }
                    if (CCLED.AllSwitch)
                        CCLED.CurrentStatus = string.Format((IFormatProvider)DataCenter.CultureInfoUS, "1004{0}%{1}%{2}%{3}%{4}%{5}%{6}%{7}%{8}%{9}%{10}%{11}%{12}%{13}%{14}%{15}%{16}%{17}%{18}%{19}", (object)CCOC.GPUSnowStatus, CCLED.AllSwitch ? (object)"T" : (object)"F", (object)str1, (object)Convert.ToString(CLEDParser.SelectIndex), (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI, (object)str3, (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].Style.Value, (object)str4, (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.Value, (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex, (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendStyle.Value, (object)(CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect ? CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendStyle.SelectIndex : -1), CLEDParser.ColorType.Equals((object)EnumLEDType.NULL) || Enum.GetName(typeof(EnumLEDType), (object)CLEDParser.ColorType).Contains("Single") ? (object)"0" : (object)Enum.GetName(typeof(EnumLEDType), (object)CLEDParser.ColorType), (object)str2, (object)(CLEDParser.List_PartItem[CLEDParser.SelectIndex].CurrentCircleR.ToString() + ":" + CLEDParser.List_PartItem[CLEDParser.SelectIndex].BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS)), (object)CLEDParser.List_PartItem[CLEDParser.SelectIndex].GridViewSelectIndex, (object)(CLEDParser.DS300Index > -1 ? CLEDParser.DS300.LogoSwitch : 0), (object)(CLEDParser.DS300Index > -1 ? CLEDParser.DS300.DPISwitch : 0), (object)(CLEDParser.DS300Index > -1 ? CLEDParser.DS300.Intensity : 0), (object)(CLEDParser.DS300Index > -1 ? CLEDParser.DS300.LightingEffects : 0));
                    else
                        CCLED.CurrentStatus = "10040%F%%-1%0%-1%%-1%%-1%%-1%0%0%0:0.0%0%0%0%0%0";
                    //CLog.PrintLog(LogType.PASS, CCLED.ClassName, "NotifyChange", "CurrentStatus : " + CCLED.CurrentStatus);
                }
                //else
                    //CLog.PrintLog(LogType.LOG, CCLED.ClassName, "NotifyChange", "CModule List_PartItem.Count : " + (object)CLEDParser.List_PartItem.Count + ", SelectIndex : " + (object)CLEDParser.SelectIndex);
            }
            else
                CCLED.CurrentStatus = "9001";
        }

        internal static void SetAllSwitch(bool In_Value)
        {
            CCLED.AllSwitch = In_Value;
        }

        internal static void SetDeviceIndex(int In_Index)
        {
            CLEDParser.SelectIndex = In_Index;
            CLEDParser.SaveData(true);
        }

        internal static void SetControlByMSI(int In_Status)
        {
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI = In_Status;
            CLEDParser.SaveData();
        }

        internal static void SetLEDEffect(bool In_Value)
        {
            if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI == 2 && CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect)
                CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI = 1;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect = In_Value;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex = 0;
            CLEDParser.SaveData();
        }

        public static void SetStyleIndex(int In_Value)
        {
            if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI == 2)
                CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI = 1;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect = true;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].Style.SelectIndex = In_Value;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex = 0;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendStyle.SelectIndex = 0;
            CLEDParser.SaveData();
        }

        public static void SetMusicStyleIndex(int In_Value)
        {
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect = false;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].MusicStyle.SelectIndex = In_Value;
            CLEDParser.SaveData();
        }

        public static void SetExtendStyleIndex(int In_Value)
        {
            if (In_Value > 0)
                CLEDParser.List_PartItem[CLEDParser.SelectIndex].Style.SelectIndex = 0;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].LEDEffect = true;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendEffects = true;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ExtendStyle.SelectIndex = In_Value;
            CLEDParser.SaveData();
        }

        public static void SetColorRGB(string In_Value)
        {
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].ColorRGBStatus = In_Value.Equals("1");
            CLEDParser.SaveData();
        }

        public static void SetColor(int In_Value1, double In_Value2)
        {
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].CurrentCircleR = In_Value1;
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].BrightnessOffest = In_Value2;
            CLEDParser.SaveData();
        }

        public static void SetGridViewSelectIndex(int In_Value)
        {
            CLEDParser.List_PartItem[CLEDParser.SelectIndex].GridViewSelectIndex = In_Value;
            CLEDParser.SaveData();
        }

        public static void SetLogoSwitch(int In_Index)
        {
            CLEDParser.DS300.LogoSwitch = In_Index;
            CLEDParser.SaveData();
        }

        public static void SetDPISwitch(int In_Index)
        {
            CLEDParser.DS300.DPISwitch = In_Index;
            CLEDParser.SaveData();
        }

        public static void SetIntensity(int In_Index)
        {
            CLEDParser.DS300.Intensity = In_Index;
            CLEDParser.SaveData();
        }

        public static void SetLightingEffects(int In_Index)
        {
            CLEDParser.DS300.LightingEffects = In_Index;
            CLEDParser.SaveData();
        }

        public static void SetJLEDStatus(string In_Value)
        {
            CCLED.JLEDStatus = In_Value;
        }

        public static void SetJLEDQueue(string In_Value)
        {
            CCLED.JLEDQueue = In_Value;
        }
    }
}
