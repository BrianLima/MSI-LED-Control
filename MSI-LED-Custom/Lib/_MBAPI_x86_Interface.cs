using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MSI_LED_Custom.Lib
{
    static class MB
    {
        internal const string MB_DLL_FileName = "Lib\\MBAPI_x86.dll";

        [DllImport("Lib\\MBAPI_x86.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckMBVersion(string _csMB, string _csMBVer, string _csMBMarket, string _csMBSIOInit);

        [DllImport("Lib\\MBAPI_x86.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetDRAMInfo(int slot, [MarshalAs(UnmanagedType.BStr)] out string manufac, [MarshalAs(UnmanagedType.BStr)] out string partnum);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool InitialDDRTIMING();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetOCMode(bool bOC);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool CheckOCGenie(ref bool result);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool CheckOCSupport(ref bool result);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetGamingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetSilentMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPURange(ref int min, ref int max);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetFBRange(ref int min, ref int max);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUSensorVolt(ref int Volt);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUFan(ref int RPM);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUFan2(ref int RPM);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUTemp(ref int Temp);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetDRAMFrequency(ref int DARMClock);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPURatio(ref int CPURatio);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUClock(ref double CPUClock);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetFanControl_1(ref int FanControl_1);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetFanControl_2(ref int FanControl_2);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool GetCPUMaxTurboRatio([MarshalAs(UnmanagedType.LPArray), Out] byte[] ratios);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetCPUTurboRatio([MarshalAs(UnmanagedType.LPArray), In] byte[] turboratio, bool bdefault = false);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SupportLED();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDControl(int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDBOTControl(int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDMysticControl(int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDAudioControl(int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetBreathingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticBreathingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAudioBreathingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetFlashingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticFlashingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAudioFlashingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetDualBlinkingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticDualBlinkingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAudioDualBlinkingMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticMarqueeMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticRainbowMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticMeteorMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLightningMode();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticSequenceMode(int mode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetLEDModelName(int model);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ResetLED();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetColorMode(int R, int G, int B);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLEDColor(int R, int G, int B);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMusicLED(bool mystic, bool on, int mode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDMysticControlV2(int index1, int index2, int range, int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDMysticControlV2_1(int index1, int index2, int index3, int range, int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDMonoControlV2(int index1, int index2, int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDAllControlV2(int index70, int index71, int index80, int index81, int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool LEDAllControlV2_1(int index70, int index71, int index80, int index81, int index82, int ledmode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticBreathingModeV2(int index1, int index2, int range, int onTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticBreathingModeV2_1(int index1, int index2, int index3, int range, int onTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMonoBreathingModeV2(int index1, int index2, int range, int onTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAllBreathingModeV2(int index70, int index71, int index80, int index81, int range, int onTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticFlashingModeV2(int index1, int index2, int range, int onTime, int offTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticFlashingModeV2_1(int index1, int index2, int index3, int range, int onTime, int offTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMonoFlashingModeV2(int index1, int index2, int range, int onTime, int offTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAllFlashingModeV2(int index70, int index71, int index80, int index81, int range, int onTime, int offTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticDualBlinkingModeV2(int index1, int index2, int range, int onTime, int offTime, int darkTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticDualBlinkingModeV2_1(int index1, int index2, int index3, int range, int onTime, int offTime, int darkTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMonoDualBlinkingModeV2(int index1, int index2, int range, int onTime, int offTime, int darkTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetAllDualBlinkingModeV2(int index70, int index71, int index80, int index81, int range, int onTime, int offTime, int darkTime);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLEDColorV2(int index1, int index2, int R, int G, int B);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLEDColorV2_1(int index1, int index2, int index3, int R, int G, int B);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMusicLEDV2(int mode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMusicVolumeV2(int left, int right);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticMarqueeModeV2();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticRainbowModeV2();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticMeteorModeV2();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLightningModeV2(int index1, int index2, int range);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticLightningModeV2_1(int index1, int index2, int index3, int range);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetMysticStackV2();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool KeepRenesasLED();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetExtendSequence(int mode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool CloseLEDControl(bool bBackToDefault);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool CheckLANLED();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlLANLED(int value);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlFANLED(int value);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlBTLED(int value);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool CheckBTLED();

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlKingStonDRAMLED(int r, int g, int b, int speed, int style);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlCorsairDRAMLED(int changetime, int darktime, int colornum, int r, int g, int b, int brightN, int brightD);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool SetCorsairDRAMLED(int mode);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlGALAXDRAMLED(int style, int r, int g, int b);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlGALAXDRAMLED_Byte(int data0, int data1, int data2, int data3);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ControlMICRONDRAMLED(int style, int r, int g, int b);

        [DllImport("Lib\\MBAPI_x86.dll")]
        public static extern bool ReleaseDll();
    }
}
