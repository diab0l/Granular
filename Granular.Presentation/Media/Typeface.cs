using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media
{
    public class Typeface
    {
        public FontFamily FontFamily { get; private set; }
        public FontStretch Stretch { get; private set; }
        public FontStyle Style { get; private set; }
        public FontWeight Weight { get; private set; }

        public Typeface(string typefaceName, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal, FontStretch stretch = FontStretch.Normal) :
            this(new FontFamily(typefaceName), style, weight, stretch)
        {
            //
        }

        public Typeface(FontFamily fontFamily, FontStyle style = FontStyle.Normal, FontWeight weight = FontWeight.Normal, FontStretch stretch = FontStretch.Normal)
        {
            this.FontFamily = fontFamily;
            this.Style = style;
            this.Weight = weight;
            this.Stretch = stretch;
        }
    }
}
