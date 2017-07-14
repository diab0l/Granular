using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace System.Windows.Media
{
    public class FormattedText
    {
        public string Text { get; private set; }

        public Typeface Typeface { get; private set; }
        public double Size { get; private set; }
        public Brush Foreground { get; private set; }

        public FlowDirection FlowDirection { get; set; }
        public double LineHeight { get; set; }
        public int MaxLineCount { get; set; }
        public double MaxTextHeight { get; set; }
        public double MaxTextWidth { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public TextTrimming Trimming { get; set; }

        public FormattedText(string textToFormat, CultureInfo culture, FlowDirection flowDirection, Typeface typeface, double size, Brush foreground)
        {
            this.Text = textToFormat;
            this.Typeface = typeface;
            this.Size = size;
            this.Foreground = foreground;
            this.FlowDirection = flowDirection;
            this.LineHeight = Double.NaN;
            this.MaxLineCount = Int32.MaxValue;
            this.MaxTextWidth = Double.PositiveInfinity;
            this.MaxTextHeight = Double.PositiveInfinity;
        }
    }
}
