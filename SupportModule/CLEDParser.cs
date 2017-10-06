using System.Collections.Generic;
using SupportData;
using System.Text;
using System;
using System.Reflection;
using Microsoft.Win32;
using System.Windows.Media;

namespace SupportModule
{
    public static class CLEDParser
    {
        private static string ClassName = "CLEDParser";
        public static List<PartItem> List_PartItem = new List<PartItem>();
        public static DS300Mouse DS300 = new DS300Mouse();
        public static EnumLEDType ColorType = EnumLEDType.NULL;
        public static int SelectIndex = 0;
        public static int DS300Index = -1;
        public static int ThirdPartyStartIndex = -1;
        public static CLEDParser.LoadDataPlugInCall LoadDataPlugIn = (CLEDParser.LoadDataPlugInCall)null;
        public static CLEDParser.SaveDataPlugInCall SaveDataPlugIn = (CLEDParser.SaveDataPlugInCall)null;
        private static string[] Array_DataFormat = (string[])null;
        private static string[] Array_Buffer = (string[])null;
        private static string[] Array_SubBuffer = (string[])null;
        private static PartItem _PartItem = (PartItem)null;
        private static PartItem _PartSubItem = (PartItem)null;
        private static string Settings_Buffer = "";
        private static string[] Settings_Main = (string[])null;
        private static string[] Settings_Sub = (string[])null;
        private static List<PartItem> GetItems = new List<PartItem>();
        private static PartItem GetItem = new PartItem();
        private static int GetItemIndex = 0;
        private static StringBuilder StringBuilder_Data = new StringBuilder();
        private static int VGAIndex = 0;

        public static List<string> GetExtendStyles(int Index)
        {
            if (CLEDParser.List_PartItem[Index].ExtendStyle.Text.Count > 0)
                return CLEDParser.List_PartItem[Index].ExtendStyle.Text;
            return new List<string>() { "Off", "On" };
        }

        public static void VerifySupportDevice(string In_VerifyDevice)
        {
            CRegistry.SetKeyValue("", "Module Version", (object)Convert.ToString((object)Assembly.GetExecutingAssembly().GetName().Version), RegistryValueKind.String);
            if (string.IsNullOrWhiteSpace(In_VerifyDevice))
                return;
            int num1 = -1;
            int num2 = 0;
            string[] MainItem = In_VerifyDevice.Split(',');
            if (MainItem != null)
            {
                for (int v = 0; v < MainItem.Length; ++v)
                {
                    if (MainItem[v].Contains(":"))
                    {
                        CLEDParser.Array_DataFormat = MainItem[v].Split(':');
                        if (CLEDParser.Array_DataFormat != null && CLEDParser.Array_DataFormat.Length == 2)
                        {
                            MainItem[v] = CLEDParser.Array_DataFormat[0];
                            num2 = int.Parse(CLEDParser.Array_DataFormat[1]);
                        }
                    }
                    List<string> listData = DataBase.ListData;
                    Predicate<string> match = (Predicate<string>)(x => x.IndexOf(MainItem[v] + "|") == 4);
                    string str;
                    if ((str = listData.Find(match)) != null)
                    {
                        CLEDParser.Array_DataFormat = str.Split('|');
                        if (MainItem[v].Equals(CLEDParser.Array_DataFormat[1]))
                        {
                           // CLog.PrintLog(LogType.PASS, CLEDParser.ClassName, "VerifySupportDevice", string.Format("Type : {0:10}, MainDevice : {1}", (object)CLEDParser.Array_DataFormat[0], (object)CLEDParser.Array_DataFormat[2]));
                            switch (CLEDParser.Array_DataFormat[0])
                            {
                                case "M01":
                                    for (int index = 3; index < CLEDParser.Array_DataFormat.Length; ++index)
                                    {
                                        CLEDParser._PartItem = new PartItem();
                                        CLEDParser._PartItem.ItemType = "M01";
                                        CLEDParser._PartItem.MainDevice = CLEDParser.Array_DataFormat[2];
                                        CLEDParser.Array_Buffer = CLEDParser.Array_DataFormat[index].Split(',');
                                        if (CLEDParser.Array_Buffer.Length == 8)
                                        {
                                            CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                            CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                            CLEDParser._PartItem.DeviceName = CTransferFun.DeviceName(CLEDParser.Array_Buffer[1]);
                                            CLEDParser._PartItem.Chipest = CTransferFun.Chipest(CLEDParser.Array_Buffer[2]);
                                            CLEDParser._PartItem.LEDEffect = true;
                                            CLEDParser._PartItem.Style = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[3]);
                                            CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[4]);
                                            CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[5]);
                                            CLEDParser._PartItem.ExtendParameter1 = (object)CTransferFun.EntryAddress(CLEDParser.Array_Buffer[6]);
                                            CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                            CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                            CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, CLEDParser.Array_Buffer[7]);
                                        }
                                    }
                                    break;
                                case "V01":
                                    CLEDParser.Array_Buffer = CLEDParser.Array_DataFormat[3].Split(',');
                                    CLEDParser._PartItem = new PartItem();
                                    CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                    CLEDParser._PartItem.ItemType = "V01";
                                    CLEDParser._PartItem.MainDevice = CLEDParser.Array_DataFormat[2];
                                    CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                    CLEDParser._PartItem.DeviceName = CTransferFun.DeviceName(CLEDParser.Array_Buffer[1]);
                                    CLEDParser._PartItem.Chipest = CTransferFun.Chipest(CLEDParser.Array_Buffer[2]);
                                    CLEDParser._PartItem.LEDEffect = true;
                                    CLEDParser._PartItem.Style = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[3]);
                                    CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[4]);
                                    CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                    CLEDParser._PartItem.ShowColorRGB = CLEDParser.Array_Buffer[5].Equals("T");
                                    ++num1;
                                    CLEDParser._PartItem.ExtendParameter1 = (object)new List<int>()
                  {
                    num1,
                    num2
                  };
                                    CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                    CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                    CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, CLEDParser.Array_Buffer[6]);
                                    if (CLEDParser._PartItem.DeviceName.Count > 1)
                                    {
                                        for (int index = 0; index < CLEDParser._PartItem.DeviceName.Count; ++index)
                                        {
                                            CLEDParser.Array_SubBuffer = CLEDParser.Array_Buffer[1].Split('+');
                                            CLEDParser._PartSubItem = new PartItem();
                                            CLEDParser._PartSubItem.Index = CLEDParser.List_PartItem.Count;
                                            CLEDParser._PartSubItem.ItemType = "SV1";
                                            CLEDParser._PartSubItem.MainDevice = CLEDParser._PartItem.MainDevice;
                                            CLEDParser._PartSubItem.ShowName = "- " + Convert.ToString((object)CLEDParser._PartItem.DeviceName[index]);
                                            CLEDParser._PartSubItem.DeviceName = new List<EnumDeviceName>()
                      {
                        CLEDParser._PartItem.DeviceName[index]
                      };
                                            CLEDParser._PartSubItem.Chipest = new List<EnumChipest>()
                      {
                        CLEDParser._PartItem.Chipest[0]
                      };
                                            CLEDParser._PartSubItem.LEDEffect = CLEDParser._PartItem.LEDEffect = true;
                                            if (CLEDParser._PartItem.DeviceName[index].Equals((object)EnumDeviceName.Front))
                                            {
                                                CLEDParser._PartSubItem.Style = CTransferFun.FrontLEDStyle("StringData", CLEDParser.Array_Buffer[3]);
                                                CLEDParser._PartSubItem.MusicStyle = CTransferFun.FrontLEDStyle("StringData", CLEDParser.Array_Buffer[4]);
                                                CLEDParser._PartSubItem.ShowColorRGB = false;
                                            }
                                            else
                                            {
                                                CLEDParser._PartSubItem.Style = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[3]);
                                                CLEDParser._PartSubItem.MusicStyle = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[4]);
                                                CLEDParser._PartSubItem.ShowColorRGB = CLEDParser._PartItem.ShowColorRGB;
                                            }
                                            CLEDParser._PartSubItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                            CLEDParser._PartSubItem.ExtendParameter1 = CLEDParser._PartItem.ExtendParameter1;
                                            CLEDParser.ParseIsEnabled(CLEDParser._PartSubItem);
                                            CLEDParser.List_PartItem.Add(CLEDParser._PartSubItem);
                                            CLEDParser.ParseDefaultColor(CLEDParser._PartSubItem.Index, CLEDParser.Array_Buffer[6]);
                                        }
                                        break;
                                    }
                                    break;
                                case "S01":
                                    using (List<PartItem>.Enumerator enumerator = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("V01"))).GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            PartItem Item = enumerator.Current;
                                            List<PartItem> all = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ShowName.Equals(Item.ShowName)));
                                            if (all.Count >= 2)
                                            {
                                                CLEDParser.Array_Buffer = CLEDParser.Array_DataFormat[2].Split(',');
                                                CLEDParser._PartItem = new PartItem();
                                                CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                                CLEDParser._PartItem.ItemType = "S01";
                                                CLEDParser._PartItem.MainDevice = all[0].MainDevice;
                                                CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                                CLEDParser._PartItem.DeviceName = CTransferFun.DeviceName(CLEDParser.Array_Buffer[1]);
                                                CLEDParser._PartItem.Chipest = CTransferFun.Chipest(CLEDParser.Array_Buffer[2]);
                                                CLEDParser._PartItem.LEDEffect = true;
                                                CLEDParser._PartItem.Style = CTransferFun.LEDStyle("StringData", CLEDParser.Array_Buffer[3]);
                                                CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("", "");
                                                CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                                CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                                CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                                CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, CLEDParser.Array_Buffer[4]);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                case "T01":
                                    CLEDParser.Array_Buffer = CLEDParser.Array_DataFormat[3].Split(',');
                                    CLEDParser._PartItem = new PartItem();
                                    CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                    CLEDParser._PartItem.ItemType = "T01";
                                    CLEDParser._PartItem.MainDevice = CLEDParser.Array_DataFormat[2];
                                    CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                    CLEDParser._PartItem.DeviceName = new List<EnumDeviceName>()
                  {
                    EnumDeviceName.ThirdParty1
                  };
                                    CLEDParser._PartItem.Chipest = new List<EnumChipest>()
                  {
                    EnumChipest.NULL
                  };
                                    CLEDParser._PartItem.LEDEffect = true;
                                    CLEDParser._PartItem.Style = CTransferFun.LEDStyle("String3RDStyle", CLEDParser.Array_Buffer[1]);
                                    CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("", "");
                                    CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                    CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                    CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                    CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, CLEDParser.Array_Buffer[2]);
                                    break;
                            }
                        }
                    }
                }
            }
            CLEDParser.LoadData();
            CLEDParser.ThirdPartyStartIndex = CLEDParser.List_PartItem.Count;
        }

        public static int DyamicVerifyDevice(EnumDeviceAction In_DeviceAction, string In_VerifyDevice)
        {
            if (!string.IsNullOrWhiteSpace(In_VerifyDevice))
            {
                switch (In_DeviceAction)
                {
                    case EnumDeviceAction.Mount:
                        List<string> listData = DataBase.ListData;
                        Predicate<string> match = (Predicate<string>)(x => x.IndexOf(In_VerifyDevice) == 4);
                        string str;
                        if ((str = listData.Find(match)) != null)
                        {
                            CLEDParser.Array_DataFormat = str.Split('|');
                            if (In_VerifyDevice.Equals(CLEDParser.Array_DataFormat[1]))
                            {
                                CLEDParser.Array_Buffer = CLEDParser.Array_DataFormat[3].Split(',');
                                switch (CLEDParser.Array_DataFormat[0])
                                {
                                    case "T02":
                                        if (CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("T02"))).Count != 0)
                                            return -1;
                                        CLEDParser._PartItem = new PartItem();
                                        CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                        CLEDParser._PartItem.ItemType = "T02";
                                        CLEDParser._PartItem.MainDevice = CLEDParser.Array_DataFormat[2];
                                        CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                        CLEDParser._PartItem.DeviceName = new List<EnumDeviceName>()
                    {
                      EnumDeviceName.ThirdParty2
                    };
                                        CLEDParser._PartItem.Chipest = new List<EnumChipest>()
                    {
                      EnumChipest.NULL
                    };
                                        CLEDParser._PartItem.Style = CTransferFun.LEDStyle("", "");
                                        CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("", "");
                                        CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                        CLEDParser.DS300Index = CLEDParser._PartItem.Index;
                                        CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                        CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                        CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, "FF0000");
                                        CLEDParser.LoadData();
                                        return CLEDParser.List_PartItem.Count - 1;
                                    case "T03":
                                        if (CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("T03") && x.MainDevice.Equals(CLEDParser.Array_DataFormat[2]))).Count != 0)
                                            return -1;
                                        CLEDParser._PartItem = new PartItem();
                                        CLEDParser._PartItem.Index = CLEDParser.List_PartItem.Count;
                                        CLEDParser._PartItem.ItemType = "T03";
                                        CLEDParser._PartItem.MainDevice = CLEDParser.Array_DataFormat[2];
                                        CLEDParser._PartItem.ShowName = CLEDParser.Array_Buffer[0];
                                        CLEDParser._PartItem.DeviceName = new List<EnumDeviceName>()
                    {
                      EnumDeviceName.ThirdParty3
                    };
                                        CLEDParser._PartItem.Chipest = new List<EnumChipest>()
                    {
                      EnumChipest.NULL
                    };
                                        CLEDParser._PartItem.ControlByMSI = 2;
                                        CLEDParser._PartItem.Style = CTransferFun.LEDStyle("String3RDStyle", CLEDParser.Array_Buffer[1]);
                                        CLEDParser._PartItem.MusicStyle = CTransferFun.LEDStyle("", "");
                                        CLEDParser._PartItem.ExtendStyle = CTransferFun.LEDStyle("", "");
                                        CLEDParser.ParseIsEnabled(CLEDParser._PartItem);
                                        CLEDParser.List_PartItem.Add(CLEDParser._PartItem);
                                        CLEDParser.ParseDefaultColor(CLEDParser._PartItem.Index, CLEDParser.Array_Buffer[2]);
                                        CLEDParser.LoadData();
                                        return CLEDParser.List_PartItem.Count - 1;
                                }
                            }
                            break;
                        }
                        break;
                    case EnumDeviceAction.UnMount:
                        int index = CLEDParser.List_PartItem.FindIndex((Predicate<PartItem>)(x => x.MainDevice.Equals(In_VerifyDevice)));
                        if (index > -1)
                        {
                            if (CLEDParser.List_PartItem[index].ItemType.Equals("T02"))
                                CLEDParser.DS300Index = -1;
                            lock (CLEDParser.List_PartItem)
                                CLEDParser.List_PartItem.RemoveAt(index);
                            for (int thirdPartyStartIndex = CLEDParser.ThirdPartyStartIndex; thirdPartyStartIndex < CLEDParser.List_PartItem.Count; ++thirdPartyStartIndex)
                                CLEDParser.List_PartItem[thirdPartyStartIndex].Index = thirdPartyStartIndex;
                        }
                        return index;
                }
            }
            return -1;
        }

        public static void ChangeMapVGAIndex(int[] In_Index)
        {
            List<PartItem> all = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.IndexOf("V") == 0 || x.ItemType.IndexOf("SV") == 0));
            if (all == null || all.Count <= 0)
                return;
            int index1 = 0;
            for (int index2 = 0; index2 < In_Index.Length; ++index2)
            {
                for (int index3 = 0; index3 <= all[index1].DeviceName.Count; ++index3)
                    (CLEDParser.List_PartItem[all[index1 + index3].Index].ExtendParameter1 as List<int>)[1] = In_Index[index2];
                index1 = all[index1].DeviceName.Count + 1;
            }
        }

        public static void SynchronizeVGAData(int In_Index)
        {
            if (CLEDParser.List_PartItem[In_Index].DeviceName.Count <= 1)
                return;
            for (int index = In_Index; index < In_Index + CLEDParser.List_PartItem[In_Index].DeviceName.Count + 1; ++index)
            {
                CLEDParser.List_PartItem[index].LEDEffect = CLEDParser.List_PartItem[In_Index].LEDEffect;
                CLEDParser.List_PartItem[index].Style.SelectIndex = CLEDParser.List_PartItem[In_Index].Style.SelectIndex;
                CLEDParser.List_PartItem[index].MusicStyle.SelectIndex = CLEDParser.List_PartItem[In_Index].MusicStyle.SelectIndex;
                CLEDParser.List_PartItem[index].CurrentCircleR = CLEDParser.List_PartItem[In_Index].CurrentCircleR;
                CLEDParser.List_PartItem[index].BrightnessOffest = CLEDParser.List_PartItem[In_Index].BrightnessOffest;
                CLEDParser.List_PartItem[index].ShowColorRGB = CLEDParser.List_PartItem[In_Index].ShowColorRGB;
                CLEDParser.List_PartItem[index].ColorRGBStatus = CLEDParser.List_PartItem[In_Index].ColorRGBStatus;
                CLEDParser.List_PartItem[index].GridViewSelectIndex = CLEDParser.List_PartItem[In_Index].GridViewSelectIndex;
            }
        }

        public static void ParseColorValue()
        {
            CLEDParser.ParseColorValue(CLEDParser.SelectIndex);
        }

        public static void ParseColorValue(int In_SelectIndex)
        {
            if (In_SelectIndex == CLEDParser.DS300Index)
            {
                CLEDParser.ColorType = EnumLEDType.RGB_R;
            }
            else
            {
                CLEDParser.ColorType = EnumLEDType.NULL;
                if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ControlByMSI != 2)
                {
                    if (CLEDParser.List_PartItem[In_SelectIndex].LEDEffect && CLEDParser.List_PartItem[In_SelectIndex].ExtendEffects)
                    {
                        if (CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.SelectIndex == 2)
                            CLEDParser.ColorType = EnumLEDType.NULL;
                        else if (CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.SelectIndex == 1)
                            CLEDParser.ColorType = CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.LEDType[CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.SelectIndex];
                        else if (CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.SelectIndex == 0)
                            CLEDParser.ColorType = CLEDParser.List_PartItem[In_SelectIndex].ExtendStyle.LEDType[0];
                    }
                    else if (CLEDParser.List_PartItem[In_SelectIndex].MusicStyle.Style.Count > 0 && CLEDParser.List_PartItem[In_SelectIndex].MusicStyle.SelectIndex > 0)
                        CLEDParser.ColorType = CLEDParser.List_PartItem[In_SelectIndex].MusicStyle.LEDType[CLEDParser.List_PartItem[In_SelectIndex].MusicStyle.SelectIndex];
                    else if (CLEDParser.List_PartItem[In_SelectIndex].LEDEffect && CLEDParser.List_PartItem[In_SelectIndex].Style.IsEnabled && CLEDParser.List_PartItem[In_SelectIndex].Style.SelectIndex > -1)
                        CLEDParser.ColorType = CLEDParser.List_PartItem[In_SelectIndex].Style.LEDType[CLEDParser.List_PartItem[In_SelectIndex].Style.SelectIndex];
                    if (CLEDParser.ColorType.Equals((object)EnumLEDType.NULL))
                    {
                        if (CLEDParser.List_PartItem[In_SelectIndex].Style.IsEnabled)
                            CLEDParser.ColorType = CLEDParser.List_PartItem[In_SelectIndex].Style.LEDType[0];
                        if (CLEDParser.ColorType.Equals((object)EnumLEDType.RGB_R))
                            CLEDParser.ColorType = EnumLEDType.RGB_Single;
                        else if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color71_R) || CLEDParser.ColorType.Equals((object)EnumLEDType.Color71_W))
                            CLEDParser.ColorType = EnumLEDType.Color7_Single;
                        else if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color81))
                            CLEDParser.ColorType = EnumLEDType.Color8_Single;
                        else if (CLEDParser.ColorType >= EnumLEDType.Color91 && CLEDParser.ColorType <= EnumLEDType.Color71_W)
                            CLEDParser.ColorType = EnumLEDType.Color9_Single;
                    }
                    if (CLEDParser.ColorType.Equals((object)EnumLEDType.R) || CLEDParser.ColorType.Equals((object)EnumLEDType.W))
                        CLEDParser.ColorType = EnumLEDType.NULL;
                    else if (!CLEDParser.List_PartItem[In_SelectIndex].LEDEffect && CLEDParser.ColorType >= EnumLEDType.Color91 && CLEDParser.ColorType <= EnumLEDType.Color95)
                        CLEDParser.ColorType = EnumLEDType.Color9_Single;
                }
                if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color92) && CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex == 8)
                {
                    CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex = 0;
                    CLEDParser.SaveData();
                }
                else if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color93) && CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex == 7)
                {
                    CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex = 0;
                    CLEDParser.SaveData();
                }
                if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color94) && CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex != 0)
                {
                    CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex = 0;
                    CLEDParser.SaveData();
                }
                else if (CLEDParser.ColorType.Equals((object)EnumLEDType.Color95) && CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex != 8)
                {
                    CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex = 8;
                    CLEDParser.SaveData();
                }
                if (CLEDParser.ColorType.Equals((object)EnumLEDType.RGB_R))
                {
                    int[] numArray = CTransferColor.TransferColor(CLEDParser.List_PartItem[In_SelectIndex].CurrentCircleR, CLEDParser.List_PartItem[In_SelectIndex].BrightnessOffest);
                    CLEDParser.List_PartItem[In_SelectIndex].CurrentR = numArray[0];
                    CLEDParser.List_PartItem[In_SelectIndex].CurrentG = numArray[1];
                    CLEDParser.List_PartItem[In_SelectIndex].CurrentB = numArray[2];
                    if (In_SelectIndex == CLEDParser.SelectIndex) { }
                        //CLog.PrintLog(LogType.PASS, CLEDParser.ClassName, "ParseColorValue", string.Format("Current [0 - F] [{0}] >> ColorType : RGB_R, R : {1}, G : {2}, B : {3}", (object)In_SelectIndex, (object)numArray[0], (object)numArray[1], (object)numArray[2]));
                }
                else if (CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex > -1)
                {
                    switch (CLEDParser.ColorType)
                    {
                        case EnumLEDType.Color7_Single:
                        case EnumLEDType.Color71_R:
                        case EnumLEDType.Color71_W:
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentR = CTransferColor.ArrayColorValue71[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][0];
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentG = CTransferColor.ArrayColorValue71[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][1];
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentB = CTransferColor.ArrayColorValue71[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][2];
                            break;
                        case EnumLEDType.Color72:
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentR = CTransferColor.ArrayColorValue72[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][0];
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentG = CTransferColor.ArrayColorValue72[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][1];
                            CLEDParser.List_PartItem[In_SelectIndex].CurrentB = CTransferColor.ArrayColorValue72[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][2];
                            break;
                        case EnumLEDType.Color8_Single:
                        case EnumLEDType.Color81:
                            if (CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex < 7)
                            {
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentR = CTransferColor.ArrayColorValue81[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][0];
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentG = CTransferColor.ArrayColorValue81[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][1];
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentB = CTransferColor.ArrayColorValue81[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][2];
                                break;
                            }
                            break;
                        case EnumLEDType.Color9_Single:
                        case EnumLEDType.Color91:
                        case EnumLEDType.Color92:
                        case EnumLEDType.Color93:
                        case EnumLEDType.Color94:
                        case EnumLEDType.Color95:
                            if (CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex < 8)
                            {
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentR = CTransferColor.ArrayColorValue9[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][0];
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentG = CTransferColor.ArrayColorValue9[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][1];
                                CLEDParser.List_PartItem[In_SelectIndex].CurrentB = CTransferColor.ArrayColorValue9[CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex][2];
                                break;
                            }
                            break;
                    }
                    if (In_SelectIndex == CLEDParser.SelectIndex) { /*i'm skippingh CLOG*/}
                        //CLog.PrintLog(LogType.PASS, CLEDParser.ClassName, "ParseColorValue", string.Format("Current [00 - FF] [{0}] >> ColorType : {1}, R : {1}, G : {2}, B : {3}", (object)In_SelectIndex, (object)Convert.ToString((object)CLEDParser.ColorType), (object)CLEDParser.List_PartItem[In_SelectIndex].CurrentR, (object)CLEDParser.List_PartItem[In_SelectIndex].CurrentG, (object)CLEDParser.List_PartItem[In_SelectIndex].CurrentB));
                }
            }
        }

        public static void ParseDefaultColor(int In_SelectIndex, string In_Value)
        {
            CLEDParser.List_PartItem[In_SelectIndex].CurrentR = (int)byte.MaxValue;
            CLEDParser.List_PartItem[In_SelectIndex].CurrentG = 0;
            CLEDParser.List_PartItem[In_SelectIndex].CurrentB = 0;
            CLEDParser.List_PartItem[In_SelectIndex].CurrentCircleR = 0;
            CLEDParser.List_PartItem[In_SelectIndex].BrightnessOffest = 0.5;
            if (In_Value.Length == 6)
            {
                SolidColorBrush solidColorBrush = (SolidColorBrush)new BrushConverter().ConvertFrom((object)("#" + In_Value));
                CLEDParser.List_PartItem[In_SelectIndex].CurrentR = (int)solidColorBrush.Color.R;
                PartItem partItem1 = CLEDParser.List_PartItem[In_SelectIndex];
                System.Windows.Media.Color color = solidColorBrush.Color;
                int g1 = (int)color.G;
                partItem1.CurrentG = g1;
                PartItem partItem2 = CLEDParser.List_PartItem[In_SelectIndex];
                color = solidColorBrush.Color;
                int b1 = (int)color.B;
                partItem2.CurrentB = b1;
                color = solidColorBrush.Color;
                int r = (int)color.R;
                color = solidColorBrush.Color;
                int g2 = (int)color.G;
                int num1;
                if (r == g2)
                {
                    color = solidColorBrush.Color;
                    int g3 = (int)color.G;
                    color = solidColorBrush.Color;
                    int b2 = (int)color.B;
                    num1 = g3 != b2 ? 1 : 0;
                }
                else
                    num1 = 1;
                if (num1 == 0)
                {
                    PartItem partItem3 = CLEDParser.List_PartItem[In_SelectIndex];
                    color = solidColorBrush.Color;
                    double num2 = (double)((int)color.R / (int)byte.MaxValue);
                    partItem3.BrightnessOffest = num2;
                }
                else
                {
                    color = solidColorBrush.Color;
                    double val1_1 = (double)color.R / (double)byte.MaxValue;
                    color = solidColorBrush.Color;
                    double val1_2 = (double)color.G / (double)byte.MaxValue;
                    color = solidColorBrush.Color;
                    double val2 = (double)color.B / (double)byte.MaxValue;
                    double num2 = Math.Max(val1_1, Math.Max(val1_2, val2));
                    double num3 = Math.Min(val1_1, Math.Min(val1_2, val2));
                    double num4 = num2 - num3;
                    if (num2 == val1_1)
                        CLEDParser.List_PartItem[In_SelectIndex].CurrentR = val1_2 < val2 ? Convert.ToInt32(Math.Abs(60.0 * ((val1_2 - val2) / num4))) : Convert.ToInt32(360.0 - 60.0 * ((val1_2 - val2) / num4));
                    else if (num2 == val1_2)
                        CLEDParser.List_PartItem[In_SelectIndex].CurrentR = Convert.ToInt32(360.0 - (60.0 * ((val2 - val1_1) / num4) + 120.0));
                    else if (num2 == val2)
                        CLEDParser.List_PartItem[In_SelectIndex].CurrentR = Convert.ToInt32(360.0 - (60.0 * ((val1_1 - val1_2) / num4) + 240.0));
                    CLEDParser.List_PartItem[In_SelectIndex].BrightnessOffest = 1.0 - (num2 + num3) / 2.0;
                }
            }
            else
            {
                if (In_Value.Length != 2)
                    return;
                CLEDParser.List_PartItem[In_SelectIndex].GridViewSelectIndex = Convert.ToInt32(In_Value);
            }
        }

        public static bool LoadData()
        {
            try
            {
                CCLED.LEDStatus = CRegistry.GetKeyBinraryValue("LED", "LEDStatus");
                if (CCLED.LEDStatus.Length != 3)
                    CCLED.LEDStatus = new byte[3];
                CCLED.AllSwitch = (int)CCLED.LEDStatus[0] != 2;
                CLEDParser.GetItems.Clear();
                CLEDParser.Settings_Buffer = CRegistry.GetKeyValue("LED", "LEDSettings");
                CLEDParser.Settings_Main = CLEDParser.Settings_Buffer.Split('|');
                if (CLEDParser.Settings_Main != null)
                {
                    CLEDParser.Settings_Sub = CLEDParser.Settings_Main[0].Split(':');
                    CLEDParser.GetItems = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ShowName.IndexOf(CLEDParser.Settings_Sub[0]) == 0));
                    if (CLEDParser.GetItems != null && CLEDParser.GetItems.Count > 0)
                    {
                        if (CLEDParser.Settings_Sub.Length == 2)
                        {
                            int index = int.Parse(CLEDParser.Settings_Sub[1]);
                            if (index >= CLEDParser.GetItems.Count)
                                index = 0;
                            CLEDParser.GetItem = CLEDParser.GetItems[index];
                        }
                        else
                            CLEDParser.GetItem = CLEDParser.GetItems[0];
                        CLEDParser.SelectIndex = CLEDParser.GetItem.Index;
                        if (CLEDParser.SelectIndex == -1)
                            CLEDParser.SelectIndex = 0;
                        for (int index = 1; index < CLEDParser.Settings_Main.Length; ++index)
                        {
                            CLEDParser.Settings_Sub = CLEDParser.Settings_Main[index].Split(',');
                            if (CLEDParser.Settings_Sub != null)
                            {
                                switch (CLEDParser.Settings_Sub[0])
                                {
                                    case "M01":
                                        List<PartItem> listPartItem1 = CLEDParser.List_PartItem;
                                        Predicate<PartItem> match1 = (Predicate<PartItem>)(x => x.ItemType.Equals("M01") && x.MainDevice.Equals(CLEDParser.Settings_Sub[1]) && x.ShowName.Equals(CLEDParser.Settings_Sub[2]));
                                        if ((CLEDParser.GetItem = listPartItem1.Find(match1)) != null)
                                        {
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[3].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[4]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].MusicStyle.SelectIndex = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ExtendEffects = CLEDParser.Settings_Sub[6].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ExtendStyle.SelectIndex = int.Parse(CLEDParser.Settings_Sub[7]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[8]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[9], (IFormatProvider)DataCenter.CultureInfoUS);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].GridViewSelectIndex = int.Parse(CLEDParser.Settings_Sub[10]);
                                            CLEDParser.ParseColorValue(CLEDParser.GetItem.Index);
                                            break;
                                        }
                                        break;
                                    case "V01":
                                        CLEDParser.GetItems = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("V01") && x.MainDevice.Equals(CLEDParser.Settings_Sub[2])));
                                        CLEDParser.GetItemIndex = CLEDParser.GetItems.Count != 1 ? Convert.ToInt32(CLEDParser.Settings_Sub[1]) : 0;
                                        if (CLEDParser.GetItems != null && CLEDParser.GetItemIndex < CLEDParser.GetItems.Count)
                                        {
                                            CLEDParser.GetItem = CLEDParser.GetItems[CLEDParser.GetItemIndex];
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[4].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].MusicStyle.SelectIndex = int.Parse(CLEDParser.Settings_Sub[6]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[7]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[8], (IFormatProvider)DataCenter.CultureInfoUS);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ColorRGBStatus = CLEDParser.Settings_Sub[9].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].GridViewSelectIndex = int.Parse(CLEDParser.Settings_Sub[10]);
                                            CLEDParser.ParseColorValue(CLEDParser.GetItem.Index);
                                            break;
                                        }
                                        break;
                                    case "SV1":
                                        CLEDParser.GetItems = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("SV1") && x.MainDevice.Equals(CLEDParser.Settings_Sub[2]) && x.ShowName.Equals(CLEDParser.Settings_Sub[3])));
                                        CLEDParser.GetItemIndex = CLEDParser.GetItems.Count != 1 ? Convert.ToInt32(CLEDParser.Settings_Sub[1]) : 0;
                                        if (CLEDParser.GetItems != null && CLEDParser.GetItemIndex < CLEDParser.GetItems.Count)
                                        {
                                            CLEDParser.GetItem = CLEDParser.GetItems[CLEDParser.GetItemIndex];
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[4].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].MusicStyle.SelectIndex = int.Parse(CLEDParser.Settings_Sub[6]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[7]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[8], (IFormatProvider)DataCenter.CultureInfoUS);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ColorRGBStatus = CLEDParser.Settings_Sub[9].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].GridViewSelectIndex = int.Parse(CLEDParser.Settings_Sub[10]);
                                            CLEDParser.ParseColorValue(CLEDParser.GetItem.Index);
                                            break;
                                        }
                                        break;
                                    case "S01":
                                        List<PartItem> listPartItem2 = CLEDParser.List_PartItem;
                                        Predicate<PartItem> match2 = (Predicate<PartItem>)(x => x.ItemType.Equals("S01") && x.MainDevice.Equals(CLEDParser.Settings_Sub[1]));
                                        if ((CLEDParser.GetItem = listPartItem2.Find(match2)) != null)
                                        {
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ShowName = CLEDParser.Settings_Sub[2];
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[3].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[4]);
                                            break;
                                        }
                                        break;
                                    case "T01":
                                        CLEDParser.GetItem = CLEDParser.List_PartItem.Find((Predicate<PartItem>)(x => x.ItemType.Equals("T01") && x.MainDevice.Equals(CLEDParser.Settings_Sub[1])));
                                        if (CLEDParser.GetItem != null)
                                        {
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ControlByMSI = int.Parse(CLEDParser.Settings_Sub[3]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[4].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[6]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[7], (IFormatProvider)DataCenter.CultureInfoUS);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ColorRGBStatus = CLEDParser.Settings_Sub[8].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].GridViewSelectIndex = int.Parse(CLEDParser.Settings_Sub[9]);
                                            CLEDParser.ParseColorValue(CLEDParser.GetItem.Index);
                                            break;
                                        }
                                        break;
                                    case "T02":
                                        CLEDParser.GetItem = CLEDParser.List_PartItem.Find((Predicate<PartItem>)(x => x.ItemType.Equals("T02")));
                                        if (CLEDParser.GetItem != null)
                                        {
                                            CLEDParser.DS300.LogoSwitch = int.Parse(CLEDParser.Settings_Sub[1]);
                                            CLEDParser.DS300.DPISwitch = int.Parse(CLEDParser.Settings_Sub[2]);
                                            CLEDParser.DS300.Intensity = int.Parse(CLEDParser.Settings_Sub[3]);
                                            CLEDParser.DS300.LightingEffects = int.Parse(CLEDParser.Settings_Sub[4]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[6], (IFormatProvider)DataCenter.CultureInfoUS);
                                            int[] numArray = CTransferColor.TransferColor(CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR, CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentR = numArray[0];
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentG = numArray[1];
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentB = numArray[2];
                                            break;
                                        }
                                        break;
                                    case "T03":
                                        CLEDParser.GetItem = CLEDParser.List_PartItem.Find((Predicate<PartItem>)(x => x.ItemType.Equals("T03") && x.MainDevice.Equals(CLEDParser.Settings_Sub[1])));
                                        if (CLEDParser.GetItem != null)
                                        {
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ControlByMSI = int.Parse(CLEDParser.Settings_Sub[3]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].LEDEffect = CLEDParser.Settings_Sub[4].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].Style.SelectIndex = int.Parse(CLEDParser.Settings_Sub[5]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].CurrentCircleR = int.Parse(CLEDParser.Settings_Sub[6]);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].BrightnessOffest = double.Parse(CLEDParser.Settings_Sub[7], (IFormatProvider)DataCenter.CultureInfoUS);
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].ColorRGBStatus = CLEDParser.Settings_Sub[8].Equals("T");
                                            CLEDParser.List_PartItem[CLEDParser.GetItem.Index].GridViewSelectIndex = int.Parse(CLEDParser.Settings_Sub[9]);
                                            CLEDParser.ParseColorValue(CLEDParser.GetItem.Index);
                                            break;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
                if (CLEDParser.LoadDataPlugIn != null)
                    CLEDParser.LoadDataPlugIn();
            }
            catch (Exception ex)
            {
                //CLog.PrintLog(LogType.CRASH, CLEDParser.ClassName, "LoadData", "Exception : " + ex.ToString());
                return false;
            }
            return true;
        }

        private static void ParseIsEnabled(PartItem In_Item)
        {
            if (In_Item.ExtendStyle.Style.Count > 0)
                In_Item.ExtendStyle.IsEnabled = true;
            if (In_Item.MusicStyle.Style.Count > 0)
                In_Item.MusicStyle.IsEnabled = true;
            if (In_Item.MusicStyle.SelectIndex > 0)
            {
                In_Item.LEDEffectIsEnabled = false;
                In_Item.Style.IsEnabled = false;
            }
            else
            {
                In_Item.LEDEffectIsEnabled = true;
                if (In_Item.Style.Style.Count > 0)
                    In_Item.Style.IsEnabled = true;
            }
        }

        public static bool SaveData()
        {
            return CLEDParser.SaveData(false);
        }

        public static bool SaveData(bool ChangeIndex)
        {
            try
            {
                if (CLEDParser.SaveDataPlugIn != null)
                    CLEDParser.SaveDataPlugIn();
                if (!ChangeIndex && CLEDParser.List_PartItem[CLEDParser.SelectIndex].ItemType.Equals("V01"))
                    CLEDParser.SynchronizeVGAData(CLEDParser.SelectIndex);
                CLEDParser.StringBuilder_Data.Clear();
                if (CLEDParser.List_PartItem[CLEDParser.SelectIndex].ItemType.IndexOf("V") == 0 || CLEDParser.List_PartItem[CLEDParser.SelectIndex].ItemType.IndexOf("SV") == 0)
                {
                    List<PartItem> all = CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.IndexOf("V") == 0));
                    for (int index = all.Count - 1; index >= 0; --index)
                    {
                        if (CLEDParser.SelectIndex >= all[index].Index)
                        {
                            CLEDParser.StringBuilder_Data.Append(CLEDParser.List_PartItem[CLEDParser.SelectIndex].ShowName + ":" + (object)index + "|");
                            break;
                        }
                    }
                }
                else
                    CLEDParser.StringBuilder_Data.Append(CLEDParser.List_PartItem[CLEDParser.SelectIndex].ShowName + "|");
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("M01"))))
                    CLEDParser.StringBuilder_Data.Append(string.Format("M01,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}|", (object)partItem.MainDevice, (object)partItem.ShowName, partItem.LEDEffect ? (object)"T" : (object)"F", (object)partItem.Style.SelectIndex, (object)partItem.MusicStyle.SelectIndex, partItem.ExtendEffects ? (object)"T" : (object)"F", (object)partItem.ExtendStyle.SelectIndex, (object)partItem.CurrentCircleR, (object)partItem.BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS), (object)Convert.ToString(partItem.GridViewSelectIndex)));
                CLEDParser.VGAIndex = 0;
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("V01"))))
                {
                    CLEDParser.StringBuilder_Data.Append(string.Format("V01,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}|", (object)CLEDParser.VGAIndex, (object)partItem.MainDevice, (object)partItem.ShowName, partItem.LEDEffect ? (object)"T" : (object)"F", (object)partItem.Style.SelectIndex, (object)partItem.MusicStyle.SelectIndex, (object)partItem.CurrentCircleR, (object)partItem.BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS), partItem.ColorRGBStatus ? (object)"T" : (object)"F", (object)Convert.ToString(partItem.GridViewSelectIndex)));
                    if (partItem.DeviceName.Count > 1)
                    {
                        for (int index = partItem.Index + 1; index <= partItem.Index + partItem.DeviceName.Count; ++index)
                            CLEDParser.StringBuilder_Data.Append(string.Format("SV1,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}|", (object)CLEDParser.VGAIndex, (object)CLEDParser.List_PartItem[index].MainDevice, (object)CLEDParser.List_PartItem[index].ShowName, CLEDParser.List_PartItem[index].LEDEffect ? (object)"T" : (object)"F", (object)CLEDParser.List_PartItem[index].Style.SelectIndex, (object)CLEDParser.List_PartItem[index].MusicStyle.SelectIndex, (object)CLEDParser.List_PartItem[index].CurrentCircleR, (object)CLEDParser.List_PartItem[index].BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS), CLEDParser.List_PartItem[index].ColorRGBStatus ? (object)"T" : (object)"F", (object)Convert.ToString(CLEDParser.List_PartItem[index].GridViewSelectIndex)));
                    }
                    ++CLEDParser.VGAIndex;
                }
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("S01"))))
                    CLEDParser.StringBuilder_Data.Append(string.Format("S01,{0},{1},{2},{3}|", (object)partItem.MainDevice, (object)partItem.ShowName, partItem.LEDEffect ? (object)"T" : (object)"F", (object)partItem.Style.SelectIndex));
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("T01"))))
                    CLEDParser.StringBuilder_Data.Append(string.Format("T01,{0},{1},{2},{3},{4},{5},{6},{7},{8}|", (object)partItem.MainDevice, (object)partItem.ShowName, (object)partItem.ControlByMSI, partItem.LEDEffect ? (object)"T" : (object)"F", (object)partItem.Style.SelectIndex, (object)partItem.CurrentCircleR, (object)partItem.BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS), partItem.ColorRGBStatus ? (object)"T" : (object)"F", (object)Convert.ToString(partItem.GridViewSelectIndex)));
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("T02"))))
                    CLEDParser.StringBuilder_Data.Append(string.Format("T02,{0},{1},{2},{3},{4},{5}|", (object)CLEDParser.DS300.LogoSwitch, (object)CLEDParser.DS300.DPISwitch, (object)Convert.ToString(CLEDParser.DS300.Intensity), (object)Convert.ToString(CLEDParser.DS300.LightingEffects), (object)partItem.CurrentCircleR, (object)partItem.BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS)));
                foreach (PartItem partItem in CLEDParser.List_PartItem.FindAll((Predicate<PartItem>)(x => x.ItemType.Equals("T03"))))
                    CLEDParser.StringBuilder_Data.Append(string.Format("T03,{0},{1},{2},{3},{4},{5},{6},{7},{8}|", (object)partItem.MainDevice, (object)partItem.ShowName, (object)partItem.ControlByMSI, partItem.LEDEffect ? (object)"T" : (object)"F", (object)partItem.Style.SelectIndex, (object)partItem.CurrentCircleR, (object)partItem.BrightnessOffest.ToString("G2", (IFormatProvider)DataCenter.CultureInfoUS), partItem.ColorRGBStatus ? (object)"T" : (object)"F", (object)Convert.ToString(partItem.GridViewSelectIndex)));
                CRegistry.SetKeyValue("LED", "LEDSettings", (object)Convert.ToString((object)CLEDParser.StringBuilder_Data).Remove(CLEDParser.StringBuilder_Data.Length - 1), RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                //CLog.PrintLog(LogType.CRASH, CLEDParser.ClassName, "LoadData", "Exception : " + ex.ToString());
                return false;
            }
            return true;
        }

        public delegate void LoadDataPlugInCall();

        public delegate void SaveDataPlugInCall();
    }
}
