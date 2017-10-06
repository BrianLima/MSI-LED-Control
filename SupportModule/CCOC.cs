// Decompiled with JetBrains decompiler
// Type: SupportModule.CCOC
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SupportModule
{
    public static class CCOC
    {
        private static string ClassName = "CCOC";
        public static string CurrentStatus = "";
        private static byte[] OCStatus = new byte[14];
        private static byte[] OCStatus_Buffer = new byte[14];

        public static string OCSettings
        {
            get
            {
                return CRegistry.GetKeyValue("OC", "OCSettings");
            }
        }

        private static byte SupportMode
        {
            get
            {
                return CCOC.OCStatus[0];
            }
        }

        private static byte MBSupportOC
        {
            get
            {
                return CCOC.OCStatus[1];
            }
        }

        private static byte MBOCGenieStatus
        {
            get
            {
                return CCOC.OCStatus[2];
            }
        }

        private static int CPUClock
        {
            get
            {
                return ((int)CCOC.OCStatus[3] << 8) + (int)CCOC.OCStatus[4];
            }
        }

        private static int CPUPerformance
        {
            get
            {
                return (int)CCOC.OCStatus[5];
            }
        }

        private static int GPUClock
        {
            get
            {
                return ((int)CCOC.OCStatus[6] << 8) + (int)CCOC.OCStatus[7];
            }
        }

        private static int GPUPerformance
        {
            get
            {
                return (int)CCOC.OCStatus[8];
            }
        }

        internal static byte GPUSnowStatus
        {
            get
            {
                return CCOC.OCStatus[9];
            }
            set
            {
                CRegistry.SetKeyValue("OC", "OCSettings", (object)value, RegistryValueKind.String);
                //CLog.PrintLog(LogType.PASS, CCOC.ClassName, "GPUSnowStatus", "Value : " + Convert.ToString(value));
            }
        }

        internal static byte CurrentMode
        {
            get
            {
                return CCOC.OCStatus[10];
            }
            set
            {
                CRegistry.SetKeyValue("OC", "OCSettings", (object)value, RegistryValueKind.String);
                //CLog.PrintLog(LogType.PASS, CCOC.ClassName, "CurrentMode", "Value : " + Convert.ToString(value));
            }
        }

        internal static byte CurrentFunctionMode
        {
            get
            {
                return CCOC.OCStatus[11];
            }
            set
            {
                CRegistry.SetKeyValue("OC", "OCSettings", (object)value, RegistryValueKind.String);
                //CLog.PrintLog(LogType.PASS, CCOC.ClassName, "CurrentFunctionMode", "Value : " + Convert.ToString(value));
            }
        }

        private static byte VRMode
        {
            get
            {
                return CCOC.OCStatus[12];
            }
        }

        private static byte GPUNotSupport
        {
            get
            {
                return CCOC.OCStatus[13];
            }
        }

        public static int[] VGAMapIndex
        {
            get
            {
                if (CRegistry.GetKeyBinraryValue("OC", "CrossFire").Length == 0)
                    return (int[])null;
                return ((IEnumerable<byte>)CRegistry.GetKeyBinraryValue("OC", "CrossFire")).Select<byte, int>((Func<byte, int>)(x => (int)x)).ToArray<int>();
            }
            set
            {
                if (value == null)
                    CRegistry.DeleteKey("OC", "CrossFire");
                else
                    CRegistry.SetKeyValue("OC", "CrossFire", (object)((IEnumerable<int>)value).Select<int, byte>((Func<int, byte>)(x => (byte)x)).ToArray<byte>(), RegistryValueKind.Binary);
            }
        }

        public static void NotifyChange()
        {
            if (DataCenter.FN_OC)
            {
                CCOC.OCStatus = CRegistry.GetKeyBinraryValue("OC", "OCStatus");
                if (CCOC.OCStatus.Length == 0)
                    CCOC.OCStatus = new byte[14];
                CCOC.CurrentStatus = string.Format((IFormatProvider)DataCenter.CultureInfoUS, "{0}%{1}%{2}%{3}%{4}%{5}%{6}%{7}%{8}%{9}%{10}%{11}", (object)CCOC.SupportMode, (object)CCOC.MBSupportOC, (object)CCOC.MBOCGenieStatus, (object)CCOC.CPUClock, (object)CCOC.CPUPerformance, (object)CCOC.GPUClock, (object)CCOC.GPUPerformance, (object)CCOC.GPUSnowStatus, (object)CCOC.CurrentMode, (object)CCOC.CurrentFunctionMode, (object)CCOC.VRMode, (object)CCOC.GPUNotSupport);
            }
            else
                CCOC.CurrentStatus = "9001";
        }

        public static void SetInfo(int In_SupportMode, int In_MBSupportOC, int In_MBOCGenieStatus, int In_CPUClock, int In_CPUPerformance, int In_GPUClock, int In_GPUPerformance, int In_GPUSnowStatus, int In_CurrentMode, int In_CurrentFunctionMode, int In_VRMode, int In_GPUNotSupport)
        {
            CCOC.OCStatus_Buffer[0] = Convert.ToByte(In_SupportMode);
            CCOC.OCStatus_Buffer[1] = Convert.ToByte(In_MBSupportOC);
            CCOC.OCStatus_Buffer[2] = Convert.ToByte(In_MBOCGenieStatus);
            CCOC.OCStatus_Buffer[3] = Convert.ToByte(In_CPUClock >> 8);
            CCOC.OCStatus_Buffer[4] = Convert.ToByte(In_CPUClock & (int)byte.MaxValue);
            CCOC.OCStatus_Buffer[5] = Convert.ToByte(In_CPUPerformance);
            CCOC.OCStatus_Buffer[6] = Convert.ToByte(In_GPUClock >> 8);
            CCOC.OCStatus_Buffer[7] = Convert.ToByte(In_GPUClock & (int)byte.MaxValue);
            CCOC.OCStatus_Buffer[8] = Convert.ToByte(In_GPUPerformance);
            CCOC.OCStatus_Buffer[9] = Convert.ToByte(In_GPUSnowStatus);
            CCOC.OCStatus_Buffer[10] = Convert.ToByte(In_CurrentMode);
            CCOC.OCStatus_Buffer[11] = Convert.ToByte(In_CurrentFunctionMode);
            CCOC.OCStatus_Buffer[12] = Convert.ToByte(In_VRMode);
            CCOC.OCStatus_Buffer[13] = Convert.ToByte(In_GPUNotSupport);
            if (((IEnumerable<byte>)CCOC.OCStatus).SequenceEqual<byte>((IEnumerable<byte>)CCOC.OCStatus_Buffer))
                return;
            Array.Copy((Array)CCOC.OCStatus_Buffer, (Array)CCOC.OCStatus, CCOC.OCStatus.Length);
            CRegistry.SetKeyValue("OC", "OCStatus", (object)CCOC.OCStatus, RegistryValueKind.Binary);
        }
    }
}
