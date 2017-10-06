// Decompiled with JetBrains decompiler
// Type: SupportModule.CTransferFun
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using SupportData;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SupportModule
{
    internal static class CTransferFun
    {
        internal static List<EnumDeviceName> DeviceName(string In_Data)
        {
            List<EnumDeviceName> enumDeviceNameList = new List<EnumDeviceName>();
            string str1 = In_Data;
            char[] chArray = new char[1] { '+' };
            foreach (string str2 in str1.Split(chArray))
                enumDeviceNameList.Add((EnumDeviceName)Enum.Parse(typeof(EnumDeviceName), Convert.ToString(Convert.ToInt32(str2, 16))));
            return enumDeviceNameList;
        }

        internal static List<EnumChipest> Chipest(string In_Data)
        {
            List<EnumChipest> enumChipestList = new List<EnumChipest>();
            int startIndex = 0;
            while (startIndex < In_Data.Length)
            {
                enumChipestList.Add((EnumChipest)Enum.Parse(typeof(EnumChipest), Convert.ToString(Convert.ToInt32(In_Data.Substring(startIndex, 2), 16))));
                startIndex += 2;
            }
            return enumChipestList;
        }

        internal static List<EnumLEDType> LEDType(string In_Data)
        {
            List<EnumLEDType> enumLedTypeList = new List<EnumLEDType>();
            int startIndex = 0;
            while (startIndex < In_Data.Length)
            {
                enumLedTypeList.Add((EnumLEDType)Enum.Parse(typeof(EnumLEDType), Convert.ToString(Convert.ToInt32(In_Data.Substring(startIndex, 2), 16))));
                startIndex += 2;
            }
            return enumLedTypeList;
        }

        internal static List<EnumStyle> Styles(string In_Data)
        {
            List<EnumStyle> enumStyleList = new List<EnumStyle>();
            int startIndex = 0;
            while (startIndex < In_Data.Length)
            {
                enumStyleList.Add((EnumStyle)Enum.Parse(typeof(EnumStyle), Convert.ToString(Convert.ToInt32(In_Data.Substring(startIndex, 2), 16))));
                startIndex += 2;
            }
            return enumStyleList;
        }

        internal static List<string> String_Styles(string In_FrontString, List<EnumStyle> In_Data)
        {
            List<string> stringList = new List<string>();
            foreach (EnumStyle enumStyle in In_Data)
                stringList.Add(Convert.ToString(Application.Current.TryFindResource((object)(In_FrontString + Convert.ToString((object)enumStyle)))));
            return stringList;
        }

        internal static LEDStyle LEDStyle(string In_FrontString, string In_Data)
        {
            LEDStyle ledStyle = new LEDStyle();
            string[] strArray = In_Data.Split('+');
            if (strArray.Length == 2 && strArray[0].Length % 2 == 0 && strArray[0].Length == strArray[1].Length)
            {
                ledStyle.Value = strArray[1];
                ledStyle.LEDType = CTransferFun.LEDType(strArray[0]);
                ledStyle.Style = CTransferFun.Styles(strArray[1]);
            }
            else
            {
                ledStyle.Value = "";
                ledStyle.LEDType = CTransferFun.LEDType("");
                ledStyle.Style = CTransferFun.Styles("");
            }
            ledStyle.Text = CTransferFun.String_Styles(In_FrontString, ledStyle.Style);
            ledStyle.SelectIndex = ledStyle.Style.Count > 0 ? 0 : -1;
            return ledStyle;
        }

        internal static LEDStyle FrontLEDStyle(string In_FrontString, string In_Data)
        {
            LEDStyle ledStyle = new LEDStyle();
            string[] strArray1 = In_Data.Split('+');
            if (strArray1.Length == 2 && strArray1[0].Length % 2 == 0 && strArray1[0].Length == strArray1[1].Length)
            {
                strArray1[0] = "";
                for (int index = 0; index < strArray1[1].Length / 2; ++index)
                {
                    string[] strArray2;
                    (strArray2 = strArray1)[0] = strArray2[0] + "20";
                }
                ledStyle.Value = strArray1[1];
                ledStyle.LEDType = CTransferFun.LEDType(strArray1[0]);
                ledStyle.Style = CTransferFun.Styles(strArray1[1]);
            }
            else
            {
                ledStyle.Value = "";
                ledStyle.LEDType = CTransferFun.LEDType("");
                ledStyle.Style = CTransferFun.Styles("");
            }
            ledStyle.Text = CTransferFun.String_Styles(In_FrontString, ledStyle.Style);
            return ledStyle;
        }

        internal static List<List<int>> EntryAddress(string In_Data)
        {
            List<List<int>> intListList = new List<List<int>>();
            if (!string.IsNullOrWhiteSpace(In_Data))
            {
                string[] strArray1 = In_Data.Split(';');
                if (strArray1 != null && strArray1.Length > 0)
                {
                    foreach (string str1 in strArray1)
                    {
                        List<int> intList = new List<int>();
                        string[] strArray2 = str1.Split('+');
                        if (strArray2 != null && strArray2.Length > 0)
                        {
                            foreach (string str2 in strArray2)
                            {
                                if (!string.IsNullOrWhiteSpace(str2))
                                    intList.Add(Convert.ToInt32(str2, 16));
                            }
                            intListList.Add(intList);
                        }
                    }
                }
            }
            return intListList;
        }
    }
}
