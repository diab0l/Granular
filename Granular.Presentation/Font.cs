using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public enum FontStretch
    {
        Normal,
        Condensed,
        Expanded,
        ExtraCondensed,
        ExtraExpanded,
        Medium,
        SemiCondensed,
        SemiExpanded,
        UltraCondensed,
        UltraExpanded
    }

    public enum FontWeight
    {
        Normal,
        Black,
        Bold,
        DemiBold,
        ExtraBlack,
        ExtraBold,
        ExtraLight,
        Heavy,
        Light,
        Medium,
        Regular,
        SemiBold,
        Thin,
        UltraBlack,
        UltraBold,
        UltraLight
    }

    public enum FontStyle
    {
        Normal,
        Italic,
        Oblique
    }

    public enum TextWrapping
    {
        //WrapWithOverflow,
        Wrap,
        NoWrap,
    }

    public enum TextTrimming
    {
        None,
        CharacterEllipsis
        //WordEllipsis,
    }

    public class FontFamily
    {
        public static readonly FontFamily Default = new FontFamily(String.Empty);

        public string FamilyName { get { return FamilyNames.FirstOrDefault(); } }
        public IEnumerable<string> FamilyNames { get; private set; }

        public FontFamily(string familyName)
        {
            this.FamilyNames = new [] { familyName };
        }

        public FontFamily(IEnumerable<string> familyNames)
        {
            this.FamilyNames = familyNames;
        }

        public static FontFamily Parse(string value)
        {
            return new FontFamily(value.Split(',').Select(s => s.Trim()).ToArray());
        }
    }
}
