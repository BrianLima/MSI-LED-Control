namespace MSI_LED_Custom.Lib
{
    public struct MotherBoardInfo
    {
        public bool OCGenie_Status;
        public bool SupportLED;
        public bool SupportLANLED;
        public int Frequency;
        public double BaseClock;
        public int Ratio;
        public int Range_Ratio_Min;
        public int Range_Ratio_Max;
        public int Range_BaseClock_Min;
        public int Range_BaseClock_Max;
        public float Voltage;
        public int Fan1_RPM;
        public int Fan2_RPM;
        public int Fan1_Percent;
        public int Fan2_Percent;
        public int Temperature;
        public float DARM_Clock;
        public float list_CoreUtilizationItem;
        public float RAMUtilization;
    }
}