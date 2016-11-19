using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

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
}
