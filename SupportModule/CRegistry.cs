// Decompiled with JetBrains decompiler
// Type: SupportModule.CRegistry
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using Microsoft.Win32;
using System;

namespace SupportModule
{
    public static class CRegistry
    {
        private static string Project_Name = "GamingApp";
        private static RegistryKey Registry_Base = Registry.LocalMachine;

        public static string MSI_Project_Path
        {
            get
            {
                if (Environment.Is64BitOperatingSystem)
                    return "SOFTWARE\\Wow6432Node\\MSI\\" + CRegistry.Project_Name;
                return "SOFTWARE\\MSI\\" + CRegistry.Project_Name;
            }
        }

        public static bool IsKeyExist(string KeyPath)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (registryKey != null)
                    return true;
            }
            return false;
        }

        public static bool IsKeyNameExist(string KeyPath, string KeyName)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (registryKey != null)
                {
                    if (registryKey.GetValue(KeyName) != null)
                        return true;
                }
            }
            return false;
        }

        public static bool CreateSubKey(string KeyPath, string SubKeyPath)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath, true))
            {
                if (registryKey != null)
                {
                    if (registryKey.CreateSubKey(SubKeyPath) != null)
                        return true;
                }
            }
            return false;
        }

        public static string GetKeyValue(string KeyPath, string KeyName)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (registryKey != null)
                {
                    object obj = registryKey.GetValue(KeyName);
                    if (obj != null && registryKey.GetValueKind(KeyName).Equals((object)RegistryValueKind.String))
                    {
                        if (obj.ToString().IndexOf(char.MinValue) > -1)
                            return obj.ToString().Remove(obj.ToString().IndexOf(char.MinValue));
                        return obj.ToString();
                    }
                }
            }
            return "";
        }

        public static int GetKeyIntValue(string KeyPath, string KeyName)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (registryKey != null)
                {
                    object obj = registryKey.GetValue(KeyName);
                    if (obj != null && registryKey.GetValueKind(KeyName).Equals((object)RegistryValueKind.DWord))
                    {
                        if (obj.ToString().IndexOf(char.MinValue) > -1)
                            return Convert.ToInt32(obj.ToString().Remove(obj.ToString().IndexOf(char.MinValue)));
                        return Convert.ToInt32(obj.ToString());
                    }
                }
            }
            return -1;
        }

        public static byte[] GetKeyBinraryValue(string KeyPath, string KeyName)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (registryKey != null)
                {
                    object obj = registryKey.GetValue(KeyName);
                    if (obj != null && registryKey.GetValueKind(KeyName).Equals((object)RegistryValueKind.Binary))
                        return (byte[])obj;
                }
            }
            return new byte[0];
        }

        public static bool SetKeyValue(string KeyPath, string KeyName, object KeyValue, RegistryValueKind Kind)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath, true))
            {
                if (registryKey != null)
                {
                    registryKey.SetValue(KeyName, KeyValue, Kind);
                    return true;
                }
            }
            using (RegistryKey subKey = CRegistry.Registry_Base.CreateSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath))
            {
                if (subKey != null)
                {
                    subKey.SetValue(KeyName, KeyValue, Kind);
                    return true;
                }
            }
            return false;
        }

        public static bool DeleteKey(string KeyPath, string KeyName)
        {
            using (RegistryKey registryKey = CRegistry.Registry_Base.OpenSubKey(CRegistry.MSI_Project_Path + "\\" + KeyPath, true))
            {
                if (registryKey != null)
                {
                    registryKey.DeleteValue(KeyName, false);
                    return true;
                }
            }
            return false;
        }
    }
}
