using MSI_LED_Custom.Lib;
using SupportModule;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MSI_LED_Custom
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        /// 

        
        public static bool overwriteSecurityChecks;
        public static List<int> adapterIndexes;
        public static Manufacturer manufacturer;
        public static Color ledColor = Color.FromArgb(255, 255, 0, 0);
        public static AnimationType animationType = AnimationType.NoAnimation;
        public static LedManager_Common ledManager;

        public static StringBuilder SSimulationData { get; private set; }

        [STAThread]
        static void Main()
        {
            Program.adapterIndexes = new List<int>();
            Program.overwriteSecurityChecks = false;

            int gpuCountNda = 0;
            if (_NDA.NDA_Initialize())
            {
                
                bool canGetGpuCount = _NDA.NDA_GetGPUCounts(out gpuCountNda);
                if (canGetGpuCount == false)
                {
                    return;
                }

                if (gpuCountNda > 0 && InitializeNvidiaAdapters(gpuCountNda))
                {
                    manufacturer = Manufacturer.Nvidia;
                }
            }

            if (gpuCountNda == 0 && _ADL.ADL_Initialize())
            {
                int gpuCountAdl;
                bool canGetGpuCount = _ADL.ADL_GetGPUCounts(out gpuCountAdl);
                if (canGetGpuCount == false)
                {
                    return;
                }

                if (gpuCountAdl > 0 && InitializeAmdAdapters(gpuCountAdl))
                {
                    manufacturer = Manufacturer.AMD;
                }
            }

            Class_Fun_MB Fun_MB = new Class_Fun_MB();
                         Fun_MB.WMI_MB_Info = Fun_MB.GetWMI();
            //Fun_MB.MB_Info.Chipest_Type = Fun_MB.WMI_MB_Info.Chipest_Type;
            //Fun_MB.MB_Info.Market = Fun_MB.WMI_MB_Info.Market;
            //Fun_MB.MB_Info.Product = Fun_MB.WMI_MB_Info.Product;
            //Fun_MB.MB_Info.Version = Fun_MB.WMI_MB_Info.Version;

            var t = Fun_MB.Compare_Support_MB(new List<string> { "MS-7A34|3.x" }); //This is my particular Tomahawk Arctic, for a whole list, check support.cfg inside the gaming app folder
            ///////////////////////////////////////////////////////////////
            //PLACE a breakpoint here, this seems to be the important bit//
            ///////////////////////////////////////////////////////////////
            //MB.LEDControl(0);
            Fun_MB.Init_MB();//iF THIS WORKS WE MIGHT HAVE IT CONTROL THE LEDS SOMEHOW

            Fun_MB.CheckMBVersion("", "", "", "");


            SSimulationData = new StringBuilder();

            switch (Fun_MB.MB_Info.Product)
            {
                case "MS-7883":
                case "MS-7966":
                case "MS-7976":
                case "MS-7A43":
                case "MS-7A45":
                case "MS-7A46":
                case "MS-7A77":
                case "MS-7979":
                case "MS-7978":
                case "MS-7984":
                case "MS-7A12":
                case "MS-7A20":
                case "MS-7A21":
                case "MS-7A16":
                case "MS-7992":
                case "MS-7A63":
                    SSimulationData.Append(Fun_MB.MB_Info.Product.Substring(3, 4) + ",");
                    break;
                case "MS-7A54":
                    string str = Fun_MB.MB_Info.Product.Substring(3, 4);
                    if (Fun_MB.MB_Info.Market.IndexOf("TOMAHAWK") > -1 || Fun_MB.MB_Info.Market.IndexOf("S04") > -1)
                    {
                        SSimulationData.Append(str + "GAMING,");
                        break;
                    }
                    SSimulationData.Append(str + "SLI,");
                    break;
                default:
                    SSimulationData.Append(Fun_MB.MB_Info.Market + ",");
                    break;
            }




            Fun_MB.ResetLED();
            Fun_MB.Monitor();

            var a = CCLED.CurrentStatus;
            //CCLED.SetColorRGB("");
            CLEDParser.VerifySupportDevice(SSimulationData.ToString());

            for (int index = 0; index < CLEDParser.List_PartItem.Count; ++index)
            {
                if (CLEDParser.List_PartItem[index].ItemType == "V01")
                {
                    if (index == CLEDParser.SelectIndex)
                    {
                        ServiceController serviceController = new ServiceController("AsRogAuraService");
                        try
                        {
                            if (serviceController.Status.Equals((object)ServiceControllerStatus.Running))
                                serviceController.Stop();
                        }
                        catch (Exception ex)
                        {
                        }
                        if (CLEDParser.List_PartItem[index].LEDEffect)
                        {
                            //switch (CLEDParser.List_PartItem[index].Style.Style[CLEDParser.List_PartItem[index].Style.SelectIndex])
                            {
                                //case EnumStyle.NoAnimation:
                                    MB.AuraNoanimation((byte)CLEDParser.List_PartItem[index].CurrentR, (byte)CLEDParser.List_PartItem[index].CurrentG, (byte)CLEDParser.List_PartItem[index].CurrentB);
                                //    break;
                                //case EnumStyle.Breathing:
                                //    MB.AuraBreathing((byte)CLEDParser.List_PartItem[index].CurrentR, (byte)CLEDParser.List_PartItem[index].CurrentG, (byte)CLEDParser.List_PartItem[index].CurrentB);
                                //    break;
                                //case EnumStyle.Flashing:
                                //    MB.AuraFlashing((byte)CLEDParser.List_PartItem[index].CurrentR, (byte)CLEDParser.List_PartItem[index].CurrentG, (byte)CLEDParser.List_PartItem[index].CurrentB);
                                //    break;
                                }
                            }
                        }
                        else
                            MB.AuraNoanimation((byte)0, (byte)0, (byte)0);
                        break;
                    }
                    break;
                }


                var y = 
            MB.SetMysticLEDColor(255,255, 255);

           // var t = Fun_MB.Compare_Support_MB(new List<string> { "MS-7A34|3.x" }); //This is my particular Tomahawk Arctic, for a whole list, check support.cfg inside the gaming app folder
            Fun_MB.Init_MB();

            //f ()
            {

            }
            Fun_MB.ResetLED();



            ledManager = new LedManager_Common(manufacturer, animationType);
            ledManager.InitLedManagers();
            ledManager.StartAll();
            ledManager.UpdateAll(ledColor, animationType);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            
            ledManager.StopAll();
            Fun_MB.ResetLED();


        }

        private static bool InitializeAmdAdapters(int gpuCount)
        {
            for (int i = 0; i < gpuCount; i++)
            {
                AdlGraphicsInfo graphicsInfo;
                if (_ADL.ADL_GetGraphicsInfo(i, out graphicsInfo) == false)
                {
                    return false;
                }

                // PCI\VEN_1002&DEV_67DF&SUBSYS_34111462&REV_CF\4&25438C51&0&0008
                var pnpSegments = graphicsInfo.Card_PNP.Split('\\');

                if (pnpSegments.Length < 2)
                {
                    continue;
                }

                // VEN_1002&DEV_67DF&SUBSYS_34111462&REV_CF
                var codeSegments = pnpSegments[1].Split('&');

                if (codeSegments.Length < 3)
                {
                    continue;
                }

                string vendorCode = codeSegments[0].Substring(4, 4).ToUpper();
                string deviceCode = codeSegments[1].Substring(4, 4).ToUpper();
                string subVendorCode = codeSegments[2].Substring(11, 4).ToUpper();

                if (overwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase))
                    {
                        adapterIndexes.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeAmd, StringComparison.OrdinalIgnoreCase)
                    && subVendorCode.Equals(Constants.SubVendorCodeMsi, StringComparison.OrdinalIgnoreCase)
                    && Constants.SupportedDeviceCodes.Any(dc => deviceCode.Equals(dc, StringComparison.OrdinalIgnoreCase)))
                {
                    adapterIndexes.Add(i);
                }
            }

            return true;
        }

        private static bool InitializeNvidiaAdapters(int gpuCount)
        {
            for (int i = 0; i < gpuCount; i++)
            {
                NdaGraphicsInfo graphicsInfo;
                if (_NDA.NDA_GetGraphicsInfo(i, out graphicsInfo) == false)
                {
                    return false;
                }

                string vendorCode = graphicsInfo.Card_pDeviceId.Substring(4, 4).ToUpper();
                string deviceCode = graphicsInfo.Card_pDeviceId.Substring(0, 4).ToUpper();
                string subVendorCode = graphicsInfo.Card_pSubSystemId.Substring(4, 4).ToUpper();

                if (overwriteSecurityChecks)
                {
                    if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase))
                    {
                        adapterIndexes.Add(i);
                    }
                }
                else if (vendorCode.Equals(Constants.VendorCodeNvidia, StringComparison.OrdinalIgnoreCase)
                    && subVendorCode.Equals(Constants.SubVendorCodeMsi, StringComparison.OrdinalIgnoreCase)
                    && Constants.SupportedDeviceCodes.Any(dc => deviceCode.Equals(dc, StringComparison.OrdinalIgnoreCase)))
                {
                    adapterIndexes.Add(i);
                }
            }
            return true;
        }
    }
}
