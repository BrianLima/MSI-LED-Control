// Decompiled with JetBrains decompiler
// Type: SupportModule.CTransferColor
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using System;

namespace SupportModule
{
    public static class CTransferColor
    {
        private static int[] Array_CurrentRGB_Buffer = new int[3];
        private static int Mod = 0;
        public static int[][] ArrayColorValue71 = new int[7][]
        {
      new int[3]{ 15, 0, 0 },
      new int[3]{ 15, 15, 0 },
      new int[3]{ 0, 15, 0 },
      new int[3]{ 0, 15, 15 },
      new int[3]{ 0, 0, 15 },
      new int[3]{ 15, 0, 15 },
      new int[3]{ 15, 15, 15 }
        };
        public static int[][] ArrayColorValue72 = new int[7][]
        {
      new int[3]{ (int) byte.MaxValue, 0, 0 },
      new int[3]{ (int) byte.MaxValue, 240, 0 },
      new int[3]{ 0, 212, 22 },
      new int[3]{ 24, 6, (int) byte.MaxValue },
      new int[3]{ 0, 138, (int) byte.MaxValue },
      new int[3]{ 132, 0, (int) byte.MaxValue },
      new int[3]
      {
        (int) byte.MaxValue,
        (int) byte.MaxValue,
        (int) byte.MaxValue
      }
        };
        public static int[][] ArrayColorValue81 = new int[7][]
        {
      new int[3]{ (int) byte.MaxValue, 0, 0 },
      new int[3]{ (int) byte.MaxValue, 240, 0 },
      new int[3]{ 0, 212, 22 },
      new int[3]{ 24, 6, (int) byte.MaxValue },
      new int[3]{ 0, 138, (int) byte.MaxValue },
      new int[3]{ 132, 0, (int) byte.MaxValue },
      new int[3]
      {
        (int) byte.MaxValue,
        (int) byte.MaxValue,
        (int) byte.MaxValue
      }
        };
        public static int[][] ArrayColorValue9 = new int[8][]
        {
      new int[3]{ (int) byte.MaxValue, 0, 0 },
      new int[3]{ (int) byte.MaxValue, 102, 0 },
      new int[3]{ (int) byte.MaxValue, 240, 0 },
      new int[3]{ 0, 212, 22 },
      new int[3]{ 24, 6, (int) byte.MaxValue },
      new int[3]{ 0, 138, (int) byte.MaxValue },
      new int[3]{ 132, 0, (int) byte.MaxValue },
      new int[3]
      {
        (int) byte.MaxValue,
        (int) byte.MaxValue,
        (int) byte.MaxValue
      }
        };
        private static double H;
        private static double f;
        private static double p;
        private static double q;
        private static double t;

        public static int[] TransferColor(int In_CircleR, double In_Brightness)
        {
            if (In_Brightness <= 0.5)
                return CTransferColor.HSV2RGB(In_CircleR, In_Brightness, 1.0);
            return CTransferColor.HSV2RGB(In_CircleR, 1.0, 1.0 - In_Brightness);
        }

        private static int[] HSV2RGB(int In_CurrentR, double S, double V)
        {
            if (S != 1.0)
                S *= 2.0;
            if (V != 1.0)
                V *= 2.0;
            CTransferColor.H = In_CurrentR != 360 ? (double)(359 - In_CurrentR) / 60.0 : 0.0;
            CTransferColor.Mod = (int)Math.Truncate(CTransferColor.H);
            CTransferColor.f = CTransferColor.H - (double)CTransferColor.Mod;
            CTransferColor.p = V * (1.0 - S);
            CTransferColor.q = V * (1.0 - S * CTransferColor.f);
            CTransferColor.t = V * (1.0 - S * (1.0 - CTransferColor.f));
            switch (CTransferColor.Mod)
            {
                case 0:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (V * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.t * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue)
                    };
                    break;
                case 1:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (CTransferColor.q * (double) byte.MaxValue),
            (int) (byte) (V * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue)
                    };
                    break;
                case 2:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue),
            (int) (byte) (V * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.t * (double) byte.MaxValue)
                    };
                    break;
                case 3:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.q * (double) byte.MaxValue),
            (int) (byte) (V * (double) byte.MaxValue)
                    };
                    break;
                case 4:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (CTransferColor.t * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue),
            (int) (byte) (V * (double) byte.MaxValue)
                    };
                    break;
                case 5:
                    CTransferColor.Array_CurrentRGB_Buffer = new int[3]
                    {
            (int) (byte) (V * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.p * (double) byte.MaxValue),
            (int) (byte) (CTransferColor.q * (double) byte.MaxValue)
                    };
                    break;
            }
            if (CTransferColor.Array_CurrentRGB_Buffer[0] > 210 && CTransferColor.Array_CurrentRGB_Buffer[1] < 50 && CTransferColor.Array_CurrentRGB_Buffer[2] < 50)
            {
                CTransferColor.Array_CurrentRGB_Buffer[0] = (int)byte.MaxValue;
                CTransferColor.Array_CurrentRGB_Buffer[1] = 0;
                CTransferColor.Array_CurrentRGB_Buffer[2] = 0;
            }
            else if (CTransferColor.Array_CurrentRGB_Buffer[0] < 30 && CTransferColor.Array_CurrentRGB_Buffer[1] > 230 && CTransferColor.Array_CurrentRGB_Buffer[2] < 30)
            {
                CTransferColor.Array_CurrentRGB_Buffer[0] = 0;
                CTransferColor.Array_CurrentRGB_Buffer[1] = (int)byte.MaxValue;
                CTransferColor.Array_CurrentRGB_Buffer[2] = 0;
            }
            else if (CTransferColor.Array_CurrentRGB_Buffer[0] < 30 && CTransferColor.Array_CurrentRGB_Buffer[1] < 30 && CTransferColor.Array_CurrentRGB_Buffer[2] > 230)
            {
                CTransferColor.Array_CurrentRGB_Buffer[0] = 0;
                CTransferColor.Array_CurrentRGB_Buffer[1] = 0;
                CTransferColor.Array_CurrentRGB_Buffer[2] = (int)byte.MaxValue;
            }
            return CTransferColor.Array_CurrentRGB_Buffer;
        }
    }
}
