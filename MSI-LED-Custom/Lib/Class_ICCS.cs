using System;
using System.Management;
using System.Reflection;
using System.Management.Instrumentation;

namespace MSI_LED_Custom.Lib
{
    internal class Class_ICCS
    {
        private static Assembly assembly = (Assembly)null;
        public int ICCS_SDK_Version = 0;
        private string CurrentWorkPath = "";
        private Type type = (Type)null;
        private object obj = (object)null;
        private const int ICCS_8 = 1;
        private const int ICCS_9 = 2;
        private const int ICCS_11 = 3;

        public Class_ICCS()
        {
            foreach (ManagementObject managementObject in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor").Get())
            {
                if (managementObject["ProcessorId"] != null)
                {
                    this.CurrentWorkPath = AppDomain.CurrentDomain.BaseDirectory;
                    string upper = managementObject["ProcessorId"].ToString().Trim().ToUpper();
                    if (upper.Substring(11, 4).Equals("506E") || upper.Substring(11, 4).Equals("906E"))
                        this.ICCS_SDK_Version = 3;
                    else if (upper.Substring(11, 4).Equals("306C") || upper.Substring(11, 4).Equals("4067"))
                        this.ICCS_SDK_Version = 2;
                    if (!(Class_ICCS.assembly != (Assembly)null))
                        break;
                    this.obj = Class_ICCS.assembly.CreateInstance(this.type.FullName, true);
                    break;
                }
            }
        }
    }
}