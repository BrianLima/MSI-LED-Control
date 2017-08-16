using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Management.Instrumentation;

namespace MSI_LED_Custom.Lib
{
    class Class_Fun_MB
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

        public Class_Fun_MB()
        {
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
                if (this.WMI_MB_Info.Chipest_Type.IndexOf("100") > -1)
                {
                    if (int32 >= 11)
                        flag = true;
                }
                else if (int32 >= 9)
                    flag = true;
            }
            if (flag)
                this.NecessaryFile = true;
            return this.NecessaryFile;
        }

        public WMI_MB GetWMI()
        {
            try
            {
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                string str1 = "";
                foreach (ManagementBaseObject managementBaseObject in managementObjectSearcher.Get())
                    str1 = managementBaseObject["Name"].ToString();
                foreach (ManagementObject managementObject in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_BaseBoard").Get())
                {
                    this.WMI_MB_Info.Manufacturer = managementObject["Manufacturer"].ToString();
                    string str2 = managementObject["Product"].ToString();
                    if (str2.IndexOf("(") > -1)
                    {
                        this.WMI_MB_Info.Market = !(str2.Substring(str2.IndexOf("(") - 1, 1) == " ") ? str2.Substring(0, str2.IndexOf("(")) : str2.Substring(0, str2.IndexOf("(") - 1);
                        this.WMI_MB_Info.Product = str2.Substring(str2.IndexOf('(') + 1, str2.Length - str2.IndexOf('(') - 2);
                    }
                    else
                    {
                        this.WMI_MB_Info.Market = str2;
                        this.WMI_MB_Info.Product = "MS-XXXX";
                    }
                    this.WMI_MB_Info.Chipest_Type = this.WMI_MB_Info.Market.IndexOf("Z9") <= -1 ? (this.WMI_MB_Info.Market.IndexOf("X9") <= -1 ? (str1.IndexOf("Intel") >= 0 ? "O" : "AMD") : "X") : "Z";
                    string str3 = managementObject["Version"].ToString();
                    this.WMI_MB_Info.Version = str3.Remove(str3.Length - 1, 1);
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
            }
            catch
            {
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
                                return true;
                            }
                        }
                        else if (this.WMI_MB_Info.Product + "|" + this.WMI_MB_Info.Version == "MS-7A59|1.")
                        {
                            if (this.WMI_MB_Info.Market.ToUpper().IndexOf("GAMING") > 0)
                            {
                                this.MB_Info.Product = this.WMI_MB_Info.Product;
                                this.MB_Info.Version = this.WMI_MB_Info.Version;
                                this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                                this.MB_Info.Market = this.WMI_MB_Info.Market;
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
                                return true;
                            }
                        }
                        else
                        {
                            this.MB_Info.Product = this.WMI_MB_Info.Product;
                            this.MB_Info.Version = this.WMI_MB_Info.Version;
                            this.MB_Info.Chipest_Type = this.WMI_MB_Info.Chipest_Type;
                            this.MB_Info.Market = this.WMI_MB_Info.Market;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public int GetFrequency()
        {
            int num1 = 0;
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
                this.ICCS = new Class_ICCS();
                this.SDK_Version = this.ICCS.ICCS_SDK_Version;
                this.MotherBoard_Info.BaseClock = MB.GetCPUClock(ref CPUClock) ? CPUClock : 100.0;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] Class_Fun_MB.MEMORYSTATUSEX lpBuffer);

        public bool Init_MB()
        {
            if (string.IsNullOrEmpty(this.MB_Info.Product) || string.IsNullOrEmpty(this.MB_Info.Version))
                return false;
            this.Init_MB_Status = MB.CheckMBVersion(this.MB_Info.Product, this.MB_Info.Version, this.MB_Info.Market, "T");
            return this.Init_MB_Status;
        }

        public void Monitor()
        {
            this.GetBaseClock();
            this.GetFrequency();
            if (!this.Init_MB_Status)
                return;
            this.CheckOCGenie();
            this.GetDRAMFrequency();
            this.GetVoltage();
            this.GetRatio();
            this.GetTemperature();
            this.GetFan1_RPM();
            this.GetFan1_Percent();
            this.GetPerformance();
            this.GetRAMPerformance();
        }

        public bool CheckMBVersion(string Model, string Version, string Market)
        {
            return MB.CheckMBVersion(Model, Version, Market, "T");
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
        }

        public bool Set_OC_Mode(bool Bool_Status)
        {
            bool flag = false;
            if (Bool_Status)
            {
                if (!this.CheckOCGenie())
                {
                    try
                    {
                        flag = MB.SetOCMode(true);
                    }
                    catch
                    {
                    }
                }
            }
            else
                flag = MB.SetOCMode(false);
            return flag;
        }

        public bool Set_Gaming_Mode(bool bOC)
        {
            if (bOC)
                this.Set_OC_Mode(false);
            return MB.SetGamingMode();
        }

        public bool Set_Silent_Mode(bool bOC)
        {
            if (bOC)
                this.Set_OC_Mode(false);
            return MB.SetSilentMode();
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
                return this.MotherBoard_Info.Ratio * 100 / this.MotherBoard_Info.Range_Ratio_Max;
            }
            catch
            {
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

        public float GetPerformanceCounters(PerformanceCounter cpuCounter)
        {
            float num = 0.0f;
            bool flag = false;
            try
            {
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                num = cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                flag = true;
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
            if (flag)
            {
                try
                {
                    cpuCounter.CategoryName = "Processor";
                    cpuCounter.CounterName = "% Processor Time";
                    cpuCounter.InstanceName = "_Total";
                    num = cpuCounter.NextValue();
                }
                catch
                {
                }
            }
            return num;
        }

        public PerformanceCounter GetRAMFerformanceCounters()
        {
            PerformanceCounter performanceCounter = new PerformanceCounter("Memory", "Available MBytes", true);
            Class_Fun_MB.MEMORYSTATUSEX lpBuffer = new Class_Fun_MB.MEMORYSTATUSEX();
            if (Class_Fun_MB.GlobalMemoryStatusEx(lpBuffer))
                this.installedMemory = lpBuffer.ullTotalPhys / 1024UL / 1024UL;
            return performanceCounter;
        }

        public void GetPerformance()
        {
            this.MotherBoard_Info.list_CoreUtilizationItem = this.GetPerformanceCounters(this.cpuCounter);
        }

        public void GetRAMPerformance()
        {
            this.MotherBoard_Info.RAMUtilization = (float)((double)((float)this.installedMemory - this.RAMCounter.NextValue()) / (double)this.installedMemory * 100.0);
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
