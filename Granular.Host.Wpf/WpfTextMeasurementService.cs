extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;
using Granular.Host.Wpf.Render;

namespace Granular.Host.Wpf
{
    public class WpfTextMeasurementService : ITextMeasurementService
    {
        public static readonly WpfTextMeasurementService Default = new WpfTextMeasurementService(WpfValueConverter.Default);

        private IWpfValueConverter converter;

        private WpfTextMeasurementService(IWpfValueConverter converter)
        {
            this.converter = converter;
        }

        public Size Measure(string text, double fontSize, Typeface typeface, double maxWidth)
        {
            if (!text.IsNullOrEmpty() && text.EndsWith(Environment.NewLine))
            {
                text += " ";
            }

            wpf::System.Windows.Media.FormattedText formattedText = new wpf::System.Windows.Media.FormattedText(
                text.DefaultIfNullOrEmpty("A"),
                System.Globalization.CultureInfo.CurrentCulture,
                wpf::System.Windows.FlowDirection.LeftToRight,
                converter.Convert(typeface),
                fontSize,
                wpf::System.Windows.Media.Brushes.Black);

            formattedText.Trimming = wpf::System.Windows.TextTrimming.None;
            formattedText.MaxTextWidth = Double.IsInfinity(maxWidth) ? 0 : maxWidth;

            return new Size(text.IsNullOrEmpty() ? 0 : Math.Ceiling(formattedText.WidthIncludingTrailingWhitespace + 4), Math.Ceiling(formattedText.Height));
        }
    }
}
