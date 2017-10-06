// Decompiled with JetBrains decompiler
// Type: SupportModule.LEDStyle
// Assembly: SupportModule, Version=6.2.0.23, Culture=neutral, PublicKeyToken=null
// MVID: C792958B-8EBA-4C92-BA6B-B52FC5CBAF83
// Assembly location: C:\Program Files (x86)\MSI\Gaming APP\SupportModule.dll

using SupportData;
using System.Collections.Generic;

namespace SupportModule
{
    public class LEDStyle
    {
        public bool IsEnabled { get; set; }

        public string Value { get; set; }

        public List<EnumLEDType> LEDType { get; set; }

        public List<EnumStyle> Style { get; set; }

        public List<string> Text { get; set; }

        public int SelectIndex { get; set; }
    }
}
