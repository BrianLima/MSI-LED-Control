// Decompiled with JetBrains decompiler
// Type: SupportModule.PartItem
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using SupportData;
using System.Collections.Generic;
using System.Windows.Media;

namespace SupportModule
{
    public class PartItem
    {
        public int Index { get; set; }

        public string ItemType { get; set; }

        public int ControlByMSI { get; set; }

        public bool LEDEffect { get; set; }

        public bool LEDEffectIsEnabled { get; set; }

        public string MainDevice { get; set; }

        public List<EnumChipest> Chipest { get; set; }

        public string ShowName { get; set; }

        public List<EnumDeviceName> DeviceName { get; set; }

        public LEDStyle Style { get; set; }

        public LEDStyle MusicStyle { get; set; }

        public LEDStyle ExtendStyle { get; set; }

        public bool ExtendEffects { get; set; }

        public object ExtendParameter1 { get; set; }

        public bool ShowColorRGB { get; set; }

        public bool ColorRGBStatus { get; set; }

        public int CurrentCircleR { get; set; }

        public double BrightnessOffest { get; set; }

        public int CurrentR { get; set; }

        public int CurrentG { get; set; }

        public int CurrentB { get; set; }

        public SolidColorBrush SolidColorBrush_Current { get; set; }

        public int GridViewSelectIndex { get; set; }

        public int ChangeValue { get; set; }
    }
}
