using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;
using MSI_LED_Custom;

namespace MSI_LED_Custom.Lib
{
    public class Class_Fun_MB
    {
        public WMI_MB WMI_MB_Info = new WMI_MB();
        public MBInfo MB_Info = new MBInfo();
        public MotherBoardInfo MotherBoard_Info = new MotherBoardInfo();
        private Class_ICCS ICCS = (Class_ICCS)null;
        public int SDK_Version = 0;
        public bool Init_MB_Status = false;
        public bool NecessaryFile = false;
        private bool bME9 = false;
        private bool bME11 = false;
        public bool VR_MB_Support = false;
        public int VR_CPU_Ratio = 0;
        private PerformanceCounter RAMCounter = (PerformanceCounter)null;
        private PerformanceCounter cpuCounter = (PerformanceCounter)null;
        public ulong installedMemory;

        public Class_Fun_MB(/*App App_Content*/)
        {
            //this.App_Content = App_Content;
            this.MB_Info.Chipest_Type = "O";
            this.MB_Info.Market = "M00-SI00";
            this.MB_Info.Product = "MS-0000";
            this.MB_Info.Version = "0.";
        }

        [DllImport("Lib\\DeviceManagerDLL.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void GetDeviceInfo(Guid Class_GUID, [MarshalAs(UnmanagedType.BStr)] string DisplayName, [MarshalAs(UnmanagedType.BStr)] string DEVPKEY, [MarshalAs(UnmanagedType.BStr)] out string str, bool bAudio = false, bool bLED = false, [MarshalAs(UnmanagedType.BStr)] string CheckID = "");

        public bool Check_NecessaryFile()
        {
            bool flag = false;
            string meVersion = this.get_MEVersion();
            if (!string.IsNullOrWhiteSpace(meVersion))
            {
                int int32 = Convert.ToInt32(meVersion.Split('.')[0]);
                //this.App_Content.LogMessage.WriteErrLog("ME Ver : " + int32.ToString());
                if (this.WMI_MB_Info.Chipest_Type.IndexOf("100") > -1)
                {
                    if (int32 >= 11)
                    {
                        flag = true;
                    }
                    else
                    {
                        //this.App_Content.LogMessage.ShowMessageBox((string)Application.Current.FindResource((object)"StringDataAPPMessage3"));
                        //this.App_Content.CloseAPP();
                    }
                }
                else if (int32 >= 9)
                {
                    flag = true;
                }
                else
                {
                    //this.App_Content.LogMessage.ShowMessageBox((string)Application.Current.FindResource((object)"StringDataAPPMessage3"));
                    //this.App_Content.CloseAPP();
                }
            }
            else
            {
                //this.App_Content.LogMessage.ShowMessageBox((string)Application.Current.FindResource((object)"StringDataAPPMessage3"));
                //this.App_Content.CloseAPP();
            }
            if (flag)
                this.NecessaryFile = true;
            return this.NecessaryFile;
        }

        public WMI_MB GetWMI()
        {
            try
            {
                foreach (ManagementObject managementObject in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard").Get())
                {
                    this.WMI_MB_Info.Manufacturer = managementObject["Manufacturer"].ToString();
                    string str1 = managementObject["Product"].ToString();
                    //this.App_Content.LogMessage.WriteErrLog("Product : " + str1);
                    if (str1.IndexOf("(") > -1)
                    {
                        this.WMI_MB_Info.Market = !(str1.Substring(str1.IndexOf("(") - 1, 1) == " ") ? str1.Substring(0, str1.IndexOf("(")) : str1.Substring(0, str1.IndexOf("(") - 1);
                        this.WMI_MB_Info.Product = str1.Substring(str1.IndexOf('(') + 1, str1.Length - str1.IndexOf('(') - 2);
                    }
                    else
                    {
                        this.WMI_MB_Info.Market = str1;
                        this.WMI_MB_Info.Product = "MS-XXXX";
                    }
                    //this.App_Content.LogMessage.WriteErrLog("WMI_MB_Info.Market : " + this.WMI_MB_Info.Market);
                    //this.App_Content.LogMessage.WriteErrLog("WMI_MB_Info.Product : " + this.WMI_MB_Info.Product);
                    if (this.WMI_MB_Info.Manufacturer.ToUpper().IndexOf("ASUS") >= 0 && (this.WMI_MB_Info.Market == "STRIX Z270E GAMING" || this.WMI_MB_Info.Market == "PRIME Z270-A"))
                    {
                      //  this.App_Content.Support_ASUS_MB = true;
                        MB.CheckMBVersion("", "", "", "F", Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
                    }
                    if (this.WMI_MB_Info.Market.IndexOf("X299") > -1)
                    {
                        //App.bMBSupportHK = true;
                        //App.bMBSupportOSD = true;
                    }
                    if (this.WMI_MB_Info.Market.IndexOf("Z9") > -1)
                        this.WMI_MB_Info.Chipest_Type = "Z";
                    else if (this.WMI_MB_Info.Market.IndexOf("X9") > -1)
                        this.WMI_MB_Info.Chipest_Type = "X";
                    //else if (App.CPU_Name.IndexOf("Intel") < 0)
                    //{
                    //    this.WMI_MB_Info.Chipest_Type = "AMD";
                    //    if (this.WMI_MB_Info.Product.IndexOf("7992") >= 0 || this.WMI_MB_Info.Product.IndexOf("7A") >= 0 || this.WMI_MB_Info.Product.IndexOf("7B") >= 0)
                    //    {
                    //        App.bMBSupportHK = true;
                    //        App.bMBSupportOSD = true;
                    //    }
                    //}
                    else
                        this.WMI_MB_Info.Chipest_Type = "O";
                    string str2 = managementObject["Version"].ToString();
                    this.WMI_MB_Info.Version = str2.Remove(str2.Length - 1, 1);
                    //this.App_Content.LogMessage.WriteErrLog("WMI_MB_Info.Version : " + this.WMI_MB_Info.Version);
                }
                foreach (ManagementObject managementObject in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor").Get())
                {
                    if (managementObject["ProcessorId"] != null)
                    {
                        string upper = managementObject["ProcessorId"].ToString().Trim().ToUpper();
                        if (upper.Substring(11, 4).Equals("506E") || upper.Substring(11, 4).Equals("906E"))
                        {
                            this.WMI_MB_Info.Chipest_Type = "100";
                            break;
                        }
                        break;
                    }
                }
                //this.App_Content.LogMessage.WriteErrLog("WMI_MB_Info.Chipest_Type : " + this.WMI_MB_Info.Chipest_Type);
            }
            catch (ManagementException ex)
            {
                //this.App_Content.LogMessage.WriteErrLog((Exception)ex);
            }
            return this.WMI_MB_Info;
        }

        public string get_MEVersion()
        {
            string str = "";
            Class_Fun_MB.GetDeviceInfo(new Guid(1295444349U, (ushort)58149, (ushort)4558, (byte)191, (byte)193, (byte)8, (byte)0, (byte)43, (byte)225, (byte)3, (byte)24), "Intel(R) Management Engine Interface ", "Driver_Version", out str, false, false, "");
            return str;
        }

        public bool Compare_Support_MB(List<string> List_Support)
        {
            this.GetWMI();
            if (!string.IsNullOrEmpty(this.WMI_MB_Info.Manufacturer) && !string.IsNullOrEmpty(this.WMI_MB_Info.Product) && !string.IsNullOrEmpty(this.WMI_MB_Info.Version))
            {
                //this.App_Content.LogMessage.PrintValueLog("Detection Motherboard results >> Manufacturer : " + this.WMI_MB_Info.Manufacturer + ", Product : " + this.WMI_MB_Info.Product + ", Version : " + this.WMI_MB_Info.Version);
                foreach (string str in List_Support)
                {
                    if (str.Remove(str.Length - 1, 1).IndexOf(this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version) == 0)
                    {
                        if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7970|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("TOMAHAWK") > 0 || this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7994|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7995|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7972|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("MORTAR") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7972|2.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("MORTAR") > 0 || this.WMI_MB_Info.Market.ToUpper().IndexOf("VDH") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A54|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("TOMAHAWK") > 0 || this.WMI_MB_Info.Market.ToUpper().IndexOf("S04") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A59|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0 || this.WMI_MB_Info.Market.ToUpper().IndexOf("S02") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A70|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("BAZOOKA") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A33|2.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A38|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("BAZOOKA") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
                                //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                                return true;
                            }
                        }
                        else
                        {
                            this.MB_Info.Product = this.WMI_MB_Info.Product;
                            this.MB_Info.Version = this.WMI_MB_Info.Version;
                            this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                            this.MB_Info.Market = this.WMI_MB_Info.Market;
                            //this.App_Content.LogMessage.PrintValueLog("This motherboard supports.");
                            return true;
                        }
                    }
                }
            }
            //this.App_Content.LogMessage.PrintValueLog("This motherboard does not support.");
            return false;
        }

        public int GetFrequency()
        {
            int num1 = 0;
            //if (App.WriteToLog)
            //{
            //    this.App_Content.LogMessage.WriteErrLog("Get Frequency Base Clock: " + this.MotherBoard_Info.BaseClock.ToString());
            //    this.App_Content.LogMessage.WriteErrLog("Get Frequency Ratio: " + this.MotherBoard_Info.Ratio.ToString());
            //}
            if (this.MotherBoard_Info.BaseClock > 0.0 && this.MotherBoard_Info.Ratio != -1)
            {
                double num2;
                try
                {
                    num2 = this.MotherBoard_Info.BaseClock;
                }
                catch
                {
                    num2 = !this.MB_Info.Chipest_Type.Equals("AMD") ? 100.0 : 200.0;
                }
                this.MotherBoard_Info.Frequency = (int)(num2 * (double)this.MotherBoard_Info.Ratio);
                num1 = this.MotherBoard_Info.Frequency;
            }
            else if (this.MotherBoard_Info.BaseClock == -1.0)
                return -1;
            return num1;
        }

        public void GetBaseClock()
        {
            double CPUClock = 0.0;
            if (this.MB_Info.Product.Contains("7693") || this.MB_Info.Product.Contains("7893") || this.MB_Info.Product.Contains("7992"))
            {
                this.MotherBoard_Info.BaseClock = MB.GetCPUClock(ref CPUClock) ? CPUClock : 200.0;
            }
            else
            {
                //if (App.CPU_Name.Contains("Intel") && this.ICCS == null)
                {
                    this.ICCS = new Class_ICCS();
                    this.SDK_Version = this.ICCS.ICCS_SDK_Version;
                }
                this.MotherBoard_Info.BaseClock = MB.GetCPUClock(ref CPUClock) ? CPUClock : 100.0;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] Class_Fun_MB.MEMORYSTATUSEX lpBuffer);

        public bool Init_MB()
        {
            if (!string.IsNullOrEmpty(this.MB_Info.Product) && !string.IsNullOrEmpty(this.MB_Info.Version))
            {
                this.Init_MB_Status = MB.CheckMBVersion(this.MB_Info.Product, this.MB_Info.Version, this.MB_Info.Market, "F", Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
                //this.App_Content.LogMessage.WriteErrLog("CheckMBVersion.");
                try
                {
                    this.cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                }
                catch (Exception ex)
                {
                  //  this.App_Content.LogMessage.WriteErrLog("Init CPU Performance catch : " + ex.Message);
                }
                try
                {
                    this.RAMCounter = new PerformanceCounter("Memory", "% Committed Bytes in Use");
                }
                catch (Exception ex)
                {
                    //this.App_Content.LogMessage.WriteErrLog("Init RAM Performance catch : " + ex.Message);
                }
                if (this.Init_MB_Status)
                {
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check SupportLED");
                    this.MotherBoard_Info.SupportLED = MB.SupportLED();
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check SupportLANLED");
                    this.MotherBoard_Info.SupportLANLED = MB.CheckLANLED();
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check Get Range BaseClock");
                    this.GetRange_BaseClock();
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check Get Range Ratio");
                    this.GetRange_Ratio();
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check Get Performance");
                    this.GetPerformance();
                    //this.App_Content.LogMessage.WriteErrLog("Init MB check Get RAM Performance Counters");
                    this.GetRAMPerformance();
                    this.CheckVRSupport();
                    return this.Init_MB_Status;
                }
            }
            return false;
        }

        public void Monitor()
        {
            //if (App.WriteToLog)
            //{
            //    this.App_Content.LogMessage.WriteErrLog("   ");
            //    this.App_Content.LogMessage.WriteErrLog("=== MB Monitor ===");
            //    this.App_Content.LogMessage.WriteErrLog("Get Base Clock");
            //}
            this.GetBaseClock();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Frequency");
            //this.GetFrequency();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Init MB Status: " + this.Init_MB_Status.ToString());
            //if (!this.Init_MB_Status)
            //    return;
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get DRAM Frequency");
            //this.GetDRAMFrequency();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Voltage");
            //this.GetVoltage();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Ratio");
            //this.GetRatio();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Temperature");
            //this.GetTemperature();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Fan1 RPM");
            //this.GetFan1_RPM();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Fan1 Percent");
            //this.GetFan1_Percent();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get Performance");
            //this.GetPerformance();
            //if (App.WriteToLog)
            //    this.App_Content.LogMessage.WriteErrLog("Get RAM Performance");
            this.GetRAMPerformance();
            int num =1 ;
            //if (this.App_Content._Window_Debug != null)
            //{
            //    bool? isChecked = this.App_Content._Window_Debug.checkBox_MB.IsChecked;
            //    num = (!isChecked.GetValueOrDefault() ? 0 : (isChecked.HasValue ? 1 : 0)) == 0 ? 1 : 0;
            //}
            //else
            //    num = 1;
            if (num == 0)
            {
                //this.App_Content._StringBuilder.Append("Product   Version   Ratio (Min)   Ratio (Max)   Base Clock (Min)   Base Clock (Max)\r");
                //this.App_Content._StringBuilder.AppendFormat("{0,7}   {1,7}   {2,10}x   {3,10}x   {4,12} MHz   {5,12} MHz\r", (object)this.MB_Info.Product, (object)this.MB_Info.Version, (object)this.MotherBoard_Info.Range_Ratio_Min, (object)this.MotherBoard_Info.Range_Ratio_Max, (object)this.MotherBoard_Info.Range_BaseClock_Min, (object)this.MotherBoard_Info.Range_BaseClock_Max);
                //this.App_Content._StringBuilder.Append("OCGenie Status   Base Clock    Ratio   CPU Clock   DARM Clock   Temperature   Fan 1       Fan 1   Fan 2       Fan 2   Voltage\r");
                //this.App_Content._StringBuilder.AppendFormat("{0, 14}   {1:F2} MHz   {2,5}x   {3, 5} MHz   {4, 6} MHz   {5, 8} ℃   {6, 5} RPM   {7, 3} %   {8, 5} RPM   {9, 3} %   {10:F3} v\r", (object)this.MotherBoard_Info.OCGenie_Status.ToString(), (object)(float)(this.MotherBoard_Info.BaseClock / 1000000.0), (object)this.MotherBoard_Info.Ratio, (object)this.MotherBoard_Info.Frequency, (object)this.MotherBoard_Info.DARM_Clock, (object)this.MotherBoard_Info.Temperature, (object)this.MotherBoard_Info.Fan1_RPM, (object)this.MotherBoard_Info.Fan1_Percent, (object)this.MotherBoard_Info.Fan2_RPM, (object)this.MotherBoard_Info.Fan2_Percent, (object)this.MotherBoard_Info.Voltage);
                //this.App_Content._Window_Debug.richTextBox_Monitor.Document.Blocks.Clear();
                //this.App_Content._Window_Debug.richTextBox_Monitor.AppendText(this.App_Content._StringBuilder.ToString());
                //this.App_Content._StringBuilder.Append("-----------------------------------------------------------------------------------------------------------------------------------------------------------------\r");
            }
        }

        public bool CheckMBVersion(string Model, string Version, string Market, string systemroot)
        {
            //TODO: Crashes when checking
            return MB.CheckMBVersion(Model, Version, Market, "T", Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)));
        }

        public bool CheckOCGenie()
        {
            bool result = false;
            bool flag = MB.CheckOCGenie(ref result);
            if (flag)
            {
                this.MotherBoard_Info.OCGenie_Status = result;
                return result;
            }
            this.MotherBoard_Info.OCGenie_Status = false;
            return flag;
        }

        public bool CheckOCSupport()
        {
            bool flag = false;
            bool result = false;
            flag = MB.CheckOCSupport(ref result);
            return result;
        }

        private void CheckVRSupport()
        {
            //if (App.CPU_ID == "")
            //    return;
            //if (App.CPU_ID.Substring(11, 4).Equals("506E"))
            //{
            //    if (App.CPU_Name.IndexOf("i7") >= 0)
            //    {
            //        if (App.CPU_Name.IndexOf("6700") < 0)
            //            return;
            //        this.VR_MB_Support = true;
            //        if (App.CPU_Name.IndexOf("6700K") >= 0)
            //            this.VR_CPU_Ratio = 43;
            //    }
            //    else
            //    {
            //        if (App.CPU_Name.IndexOf("i5") < 0 || App.CPU_Name.IndexOf("6500") < 0 && App.CPU_Name.IndexOf("6600") < 0)
            //            return;
            //        this.VR_MB_Support = true;
            //        if (App.CPU_Name.IndexOf("6600K") >= 0)
            //            this.VR_CPU_Ratio = 40;
            //    }
            //}
            //else if (App.CPU_ID.Substring(11, 4).Equals("906E"))
            //    this.VR_MB_Support = true;
            //else if (App.CPU_ID.Substring(11, 4).Equals("306F") || App.CPU_ID.Substring(11, 4).Equals("406F"))
            //{
            //    this.VR_MB_Support = true;
            //    if (App.CPU_Name.IndexOf("5960X") >= 0)
            //        this.VR_CPU_Ratio = 36;
            //    else if (App.CPU_Name.IndexOf("5930K") >= 0)
            //        this.VR_CPU_Ratio = 38;
            //    else if (App.CPU_Name.IndexOf("5820K") >= 0)
            //        this.VR_CPU_Ratio = 37;
            //    else if (App.CPU_Name.IndexOf("6950X") >= 0)
            //        this.VR_CPU_Ratio = 36;
            //    else if (App.CPU_Name.IndexOf("6900K") >= 0)
            //        this.VR_CPU_Ratio = 38;
            //    else if (App.CPU_Name.IndexOf("6850K") >= 0)
            //    {
            //        this.VR_CPU_Ratio = 39;
            //    }
            //    else
            //    {
            //        if (App.CPU_Name.IndexOf("6800K") < 0)
            //            return;
            //        this.VR_CPU_Ratio = 37;
            //    }
            //}
            //else
            //{
            //    if (App.CPU_Name.IndexOf("FX") < 0 || App.CPU_Name.IndexOf("8350") < 0 && App.CPU_Name.IndexOf("8370") < 0 && App.CPU_Name.IndexOf("9370") < 0 && App.CPU_Name.IndexOf("9590") < 0)
            //        return;
            //    this.VR_MB_Support = true;
            //}
        }

        public bool Set_OC_Mode(bool Bool_Status)
        {
            bool flag = false;
            if (Bool_Status)
            {
                //this.App_Content.MB_Mode = "OC";
                //this.App_Content.Registry.SetKeyValue("", "MB_Mode", (object)"OC", RegistryValueKind.String);
                //if (!this.App_Content.Fun_MB.CheckOCGenie())
                //{
                //    try
                //    {
                //        flag = MB.SetOCMode(true);
                //    }
                //    catch (Exception ex)
                //    {
                //        this.App_Content.LogMessage.WriteErrLog("Set OC Mode catch : " + ex.Message);
                //    }
                //}
            }
            else
                flag = MB.SetOCMode(false);
            return flag;
        }

        public bool Set_Gaming_Mode(bool bOC)
        {
            if (bOC)
                this.Set_OC_Mode(false);
            //this.App_Content.MB_Mode = "Gaming";
            //this.App_Content.Registry.SetKeyValue("", "MB_Mode", (object)"Gaming", RegistryValueKind.String);
            //TODO: This doesn't mater
            //App.ActiveX_Option.STATICBOOST.SetSysPwr(XStaticBoost.SysPwrType.Max);
            return MB.SetGamingMode();
        }

        public bool Set_Silent_Mode(bool bOC)
        {
            if (bOC)
                this.Set_OC_Mode(false);
           // this.App_Content.MB_Mode = "Silent";
           // this.App_Content.Registry.SetKeyValue("", "MB_Mode", (object)"Silent", RegistryValueKind.String);
            //App.ActiveX_Option.STATICBOOST.SetSysPwr(XStaticBoost.SysPwrType.Typical);
            return MB.SetSilentMode();
        }

        public void SetPowerCFG(string GUID)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "Powercfg.exe",
                Arguments = "-S " + GUID,
                CreateNoWindow = true,
                UseShellExecute = false
            });
        }

        public void GetPowerCFG()
        {
            string Out_Data;
            if (!this.Process_ExecuteInfo("Powercfg.exe", "/GETACTIVESCHEME", true, out Out_Data))
                return;
            if (Out_Data.IndexOf("381b4222-f694-41f0-9685-ff5bb260df2e") > -1) { }
            //this.App_Content.Registry.SetKeyValue("", "CFG", (object)"BLD", RegistryValueKind.String);
            else if (Out_Data.IndexOf("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c") > -1) { }
            //this.App_Content.Registry.SetKeyValue("", "CFG", (object)"HP", RegistryValueKind.String);
            else if (Out_Data.IndexOf("a1841308-3541-4fab-bc81-f71556f20b4a") > -1) { }
                //this.App_Content.Registry.SetKeyValue("", "CFG", (object)"PS", RegistryValueKind.String);
        }

        private bool Process_ExecuteInfo(string FileName, string Arguments, bool Hidden, out string Out_Data)
        {
            Out_Data = (string)null;
            Process process = new Process();
            process.StartInfo.FileName = FileName;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = Hidden;
            process.StartInfo.RedirectStandardOutput = true;
            bool flag = process.Start();
            process.WaitForExit();
            Out_Data = process.StandardOutput.ReadToEnd();
            return flag;
        }

        public bool GetRange_Ratio()
        {
            int min = 0;
            int max = 0;
            bool cpuRange = MB.GetCPURange(ref min, ref max);
            if (cpuRange)
            {
                this.MotherBoard_Info.Range_Ratio_Min = min;
                this.MotherBoard_Info.Range_Ratio_Max = max;
            }
            else
            {
                this.MotherBoard_Info.Range_Ratio_Min = -1;
                this.MotherBoard_Info.Range_Ratio_Max = -1;
            }
            return cpuRange;
        }

        public bool GetRange_BaseClock()
        {
            int min = 0;
            int max = 0;
            bool fbRange = MB.GetFBRange(ref min, ref max);
            if (fbRange)
            {
                this.MotherBoard_Info.Range_BaseClock_Min = min;
                this.MotherBoard_Info.Range_BaseClock_Max = max;
            }
            else
            {
                this.MotherBoard_Info.Range_BaseClock_Min = -1;
                this.MotherBoard_Info.Range_BaseClock_Max = -1;
            }
            return fbRange;
        }

        public bool GetVoltage()
        {
            int Volt = 0;
            bool cpuSensorVolt = MB.GetCPUSensorVolt(ref Volt);
            this.MotherBoard_Info.Voltage = !cpuSensorVolt ? -1f : (float)Volt / 1000f;
            return cpuSensorVolt;
        }

        public bool GetFan1_RPM()
        {
            int RPM = 0;
            bool cpuFan = MB.GetCPUFan(ref RPM);
            this.MotherBoard_Info.Fan1_RPM = !cpuFan ? -1 : RPM;
            return cpuFan;
        }

        public bool GetFan2_RPM()
        {
            int RPM = 0;
            bool cpuFan2 = MB.GetCPUFan2(ref RPM);
            this.MotherBoard_Info.Fan2_RPM = !cpuFan2 ? -1 : RPM;
            return cpuFan2;
        }

        public bool GetFan1_Percent()
        {
            int FanControl_1 = 0;
            bool fanControl1 = MB.GetFanControl_1(ref FanControl_1);
            this.MotherBoard_Info.Fan1_Percent = !fanControl1 ? -1 : FanControl_1;
            return fanControl1;
        }

        public bool GetFan2_Percent()
        {
            int FanControl_2 = 0;
            bool fanControl2 = MB.GetFanControl_2(ref FanControl_2);
            this.MotherBoard_Info.Fan2_Percent = !fanControl2 ? -1 : FanControl_2;
            return fanControl2;
        }

        public bool GetTemperature()
        {
            int Temp = 0;
            bool cpuTemp = MB.GetCPUTemp(ref Temp);
            this.MotherBoard_Info.Temperature = !cpuTemp ? -1 : Temp;
            return cpuTemp;
        }

        public bool GetDRAMFrequency()
        {
            int DARMClock = 0;
            bool dramFrequency = MB.GetDRAMFrequency(ref DARMClock);
            this.MotherBoard_Info.DARM_Clock = !dramFrequency ? -1f : (float)DARMClock;
            return dramFrequency;
        }

        public bool GetRatio()
        {
            int CPURatio = 0;
            bool cpuRatio = MB.GetCPURatio(ref CPURatio);
            this.MotherBoard_Info.Ratio = !cpuRatio ? -1 : CPURatio;
            return cpuRatio;
        }

        public int Get_Performance()
        {
            if (!this.Init_MB_Status)
                return 0;
            try
            {
                if (false)
                {
                    //this.App_Content.LogMessage.WriteErrLog("MotherBoard_Info.Ratio: " + this.MotherBoard_Info.Ratio.ToString());
                    //this.App_Content.LogMessage.WriteErrLog("MotherBoard_Info.Range_Ratio_Max: " + this.MotherBoard_Info.Range_Ratio_Max.ToString());
                }
                return this.MotherBoard_Info.Ratio * 100 / this.MotherBoard_Info.Range_Ratio_Max;
            }
            catch (Exception ex)
            {
            //    this.App_Content.LogMessage.WriteErrLog("Get_Performance catch : " + ex.Message);
                return 0;
            }
        }

        public int Get_Noise_Level()
        {
            if (this.Init_MB_Status)
                return this.MotherBoard_Info.Fan1_Percent;
            return 0;
        }

        public void GetCPUMaxTurboRatio(byte[] ratios)
        {
            MB.GetCPUMaxTurboRatio(ratios);
        }

        public void SetCPUTurboRatio(byte[] turboratio, bool bdefault = false)
        {
            MB.SetCPUTurboRatio(turboratio, bdefault);
        }

        public void LEDControl(int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDControl(ledmode);
        }

        public void LEDBOTControl(int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDBOTControl(ledmode);
        }

        public void LEDMysticControl(int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDMysticControl(ledmode);
        }

        public void LEDAudioControl(int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDAudioControl(ledmode);
        }

        public void SetBreathingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetBreathingMode();
        }

        public void SetMysticBreathingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticBreathingMode();
        }

        public void SetAudioBreathingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAudioBreathingMode();
        }

        public void SetFlashingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetFlashingMode();
        }

        public void SetMysticFlashingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticFlashingMode();
        }

        public void SetAudioFlashingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAudioFlashingMode();
        }

        public void SetDualBlinkingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetDualBlinkingMode();
        }

        public void SetMysticDualBlinkingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticDualBlinkingMode();
        }

        public void SetAudioDualBlinkingMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAudioDualBlinkingMode();
        }

        public void SetMysticMarqueeMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticMarqueeMode();
        }

        public void SetMysticRainbowMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticRainbowMode();
        }

        public void SetMysticMeteorMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticMeteorMode();
        }

        public void SetMysticLightningMode()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLightningMode();
        }

        public void SetMysticSequenceMode(int mode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticSequenceMode(mode);
        }

        public void SetLEDModelName(int model)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetLEDModelName(model);
        }

        public void ResetLED()
        {
            if (!this.Init_MB_Status)
                return;
            MB.ResetLED();
        }

        public void SetColorMode(int R, int G, int B)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetColorMode(R, G, B);
        }

        public void SetMysticLEDColor(int R, int G, int B)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLEDColor(R, G, B);
        }

        public void SetMusicLED(bool mystic, bool on, int mode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMusicLED(mystic, on, mode);
        }

        public void LEDMysticControlV2(int index1, int index2, int range, int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDMysticControlV2(index1, index2, range, ledmode);
        }

        public void LEDMysticControlV2(int index1, int index2, int index3, int range, int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDMysticControlV2_1(index1, index2, index3, range, ledmode);
        }

        public void LEDMonoControlV2(int index1, int index2, int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDMonoControlV2(index1, index2, ledmode);
        }

        public void LEDAllControlV2(int index70, int index71, int index80, int index81, int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDAllControlV2(index70, index71, index80, index81, ledmode);
        }

        public void LEDAllControlV2(int index70, int index71, int index80, int index81, int index82, int ledmode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.LEDAllControlV2_1(index70, index71, index80, index81, index82, ledmode);
        }

        public void SetMysticBreathingModeV2(int index1, int index2, int range, int onTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticBreathingModeV2(index1, index2, range, onTime);
        }

        public void SetMysticBreathingModeV2(int index1, int index2, int index3, int range, int onTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticBreathingModeV2_1(index1, index2, index3, range, onTime);
        }

        public void SetMonoBreathingModeV2(int index1, int index2, int range, int onTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMonoBreathingModeV2(index1, index2, range, onTime);
        }

        public void SetAllBreathingModeV2(int index70, int index71, int index80, int index81, int range, int onTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAllBreathingModeV2(index70, index71, index80, index81, range, onTime);
        }

        public void SetMysticFlashingModeV2(int index1, int index2, int range, int onTime, int offTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticFlashingModeV2(index1, index2, range, onTime, offTime);
        }

        public void SetMysticFlashingModeV2(int index1, int index2, int index3, int range, int onTime, int offTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticFlashingModeV2_1(index1, index2, index3, range, onTime, offTime);
        }

        public void SetMonoFlashingModeV2(int index1, int index2, int range, int onTime, int offTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMonoFlashingModeV2(index1, index2, range, onTime, offTime);
        }

        public void SetAllFlashingModeV2(int index70, int index71, int index80, int index81, int range, int onTime, int offTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAllFlashingModeV2(index70, index71, index80, index81, range, onTime, offTime);
        }

        public void SetMysticDualBlinkingModeV2(int index1, int index2, int range, int onTime, int offTime, int darkTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticDualBlinkingModeV2(index1, index2, range, onTime, offTime, darkTime);
        }

        public void SetMysticDualBlinkingModeV2(int index1, int index2, int index3, int range, int onTime, int offTime, int darkTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticDualBlinkingModeV2_1(index1, index2, index3, range, onTime, offTime, darkTime);
        }

        public void SetMonoDualBlinkingModeV2(int index1, int index2, int range, int onTime, int offTime, int darkTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMonoDualBlinkingModeV2(index1, index2, range, onTime, offTime, darkTime);
        }

        public void SetAllDualBlinkingModeV2(int index70, int index71, int index80, int index81, int range, int onTime, int offTime, int darkTime)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetAllDualBlinkingModeV2(index70, index71, index80, index81, range, onTime, offTime, darkTime);
        }

        public void SetMysticLEDColorV2(int index1, int index2, int R, int G, int B)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLEDColorV2(index1, index2, R, G, B);
        }

        public void SetMysticLEDColorV2(int index1, int index2, int index3, int R, int G, int B)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLEDColorV2_1(index1, index2, index3, R, G, B);
        }

        public void SetMusicLEDV2(int mode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMusicLEDV2(mode);
        }

        public void SetMusicVolumeV2(int left, int right)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMusicVolumeV2(left, right);
        }

        public void SetMysticMarqueeModeV2()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticMarqueeModeV2();
        }

        public void SetMysticRainbowModeV2()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticRainbowModeV2();
        }

        public void SetMysticMeteorModeV2()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticMeteorModeV2();
        }

        public void SetMysticLightningModeV2(int index1, int index2, int range)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLightningModeV2(index1, index2, range);
        }

        public void SetMysticLightningModeV2(int index1, int index2, int index3, int range)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticLightningModeV2_1(index1, index2, index3, range);
        }

        public void SetMysticStackV2()
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetMysticStackV2();
        }

        public void KeepRenesasLED()
        {
            if (!this.Init_MB_Status)
                return;
            MB.KeepRenesasLED();
        }

        public void SetExtendSequence(int mode)
        {
            if (!this.Init_MB_Status)
                return;
            MB.SetExtendSequence(mode);
        }

        public void CloseLEDControl(bool bBackToDefault)
        {
            if (!this.Init_MB_Status)
                return;
            MB.CloseLEDControl(bBackToDefault);
        }

        public bool CheckLANLED()
        {
            if (this.Init_MB_Status)
                return MB.CheckLANLED();
            return false;
        }

        public void ControlLANLED(int value)
        {
            if (!this.Init_MB_Status)
                return;
            MB.ControlLANLED(value);
        }

        public void ControlFANLED(int value)
        {
            if (!this.Init_MB_Status)
                return;
            MB.ControlFANLED(value);
        }

        public void ControlBTLED(int value)
        {
            if (!this.Init_MB_Status)
                return;
            MB.ControlBTLED(value);
        }

        public bool CheckBTLED()
        {
            if (this.Init_MB_Status)
                return MB.CheckBTLED();
            return false;
        }

        public void GetPerformance()
        {
            try
            {
                if (this.cpuCounter != null)
                    this.MotherBoard_Info.list_CoreUtilizationItem = this.cpuCounter.NextValue();
                else
                    this.MotherBoard_Info.list_CoreUtilizationItem = 0.0f;
            }
            catch (Exception ex)
            {
                this.MotherBoard_Info.list_CoreUtilizationItem = 0.0f;
                //this.App_Content.LogMessage.WriteErrLog("Get Performance catch error : " + ex.Message);
                new Process()
                {
                    StartInfo = {
            FileName = "cmd.exe",
            Arguments = "/c lodctr /R",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
          }
                }.Start();
            }
        }

        public void GetRAMPerformance()
        {
            try
            {
                if (this.RAMCounter != null)
                    this.MotherBoard_Info.RAMUtilization = this.RAMCounter.NextValue();
                else
                    this.MotherBoard_Info.RAMUtilization = 0.0f;
            }
            catch (Exception ex)
            {
                this.MotherBoard_Info.RAMUtilization = 0.0f;
         //       this.App_Content.LogMessage.WriteErrLog("Get RAM Performance catch error : " + ex.Message);
                new Process()
                {
                    StartInfo = {
            FileName = "cmd.exe",
            Arguments = "/c lodctr /R",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
          }
                }.Start();
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(Class_Fun_MB.MEMORYSTATUSEX));
            }
        }
    }
}
