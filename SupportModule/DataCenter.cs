// Decompiled with JetBrains decompiler
// Type: SupportModule.DataCenter
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using Microsoft.Win32;
using System;
using System.Globalization;
using System.Reflection;

namespace SupportModule
{
    public static class DataCenter
    {
        internal static int VersionCode = 0;
        internal static int RequirementVC = 106;
        public static int CommunicationPort = 26789;
        public static int Interval = 30;
        public static string CurrentWorkPath = AppDomain.CurrentDomain.BaseDirectory;
        public static CultureInfo CultureInfoUS = new CultureInfo("en-US");
        internal static string LocalAddress = "";
        internal static string LocalNetwork = "";

        public static string DisplayName
        {
            get
            {
                return CRegistry.GetKeyValue("", "DisplayName");
            }
            set
            {
                CRegistry.SetKeyValue("", "DisplayName", (object)value, RegistryValueKind.String);
            }
        }

        public static string VerifyCode
        {
            get
            {
                return CRegistry.GetKeyValue("", "VerifyCode");
            }
            set
            {
                CRegistry.SetKeyValue("", "VerifyCode", (object)value, RegistryValueKind.String);
            }
        }

        public static byte[] FNStatus
        {
            get
            {
                return CRegistry.GetKeyBinraryValue("", "FNStatus");
            }
            set
            {
                CRegistry.SetKeyValue("", "FNStatus", (object)value, RegistryValueKind.Binary);
            }
        }

        public static bool FN_OC
        {
            get
            {
                return DataCenter.FNStatus.Length > 0 && (int)DataCenter.FNStatus[0] == 1;
            }
        }

        public static bool FN_LED
        {
            get
            {
                return DataCenter.FNStatus.Length > 1 && (int)DataCenter.FNStatus[1] == 1;
            }
        }

        public static void Init()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DataCenter.VersionCode = Convert.ToInt32(string.Format((IFormatProvider)DataCenter.CultureInfoUS, "{0}{1}{2}{3:00}", (object)version.Major, (object)version.Minor, (object)version.Build, (object)version.Revision));
            if (!CRegistry.IsKeyExist("OC"))
                CRegistry.CreateSubKey("", "OC");
            if (!CRegistry.IsKeyExist("LED"))
                CRegistry.CreateSubKey("", "LED");
            CCOC.NotifyChange();
            CCLED.NotifyChange();
            CCLED.NotifyJLEDChange();
        }
    }
}
