using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host
{
    public class TextMeasurementService : ITextMeasurementService
    {
        public static readonly TextMeasurementService Default = new TextMeasurementService(HtmlValueConverter.Default);

        private IHtmlValueConverter converter;
        private Element htmlElement;

        private TextMeasurementService(IHtmlValueConverter converter)
        {
            this.converter = converter;
        }

        public Size Measure(string text, double fontSize, Typeface typeface, double maxWidth)
        {
            if (htmlElement == null)
            {
                htmlElement = Document.CreateElement("div");
                Document.Body.AppendChild(htmlElement);
            }

            HtmlStyleDictionary style = new HtmlStyleDictionary();
            style.SetValue("position", "absolute");
            style.SetValue("visibility", "hidden");
            style.SetFontSize(fontSize, converter);
            style.SetFontFamily(typeface.FontFamily, converter);
            style.SetFontStretch(typeface.Stretch, converter);
            style.SetFontStyle(typeface.Style, converter);
            style.SetFontWeight(typeface.Weight, converter);

            if (maxWidth.IsNaN())
            {
                style.SetTextWrapping(TextWrapping.NoWrap, converter);
            }
            else
            {
                style.SetTextWrapping(TextWrapping.Wrap, converter);
                style.SetValue("max-width", converter.ToPixelString(maxWidth));
            }

            htmlElement.SetAttribute("style", style.ToString());
            htmlElement.InnerHTML = converter.ToHtmlContentString(text.DefaultIfNullOrEmpty("A"));

            return new Size(text.IsNullOrEmpty() ? 0 : htmlElement.OffsetWidth + 2, htmlElement.OffsetHeight);
        }
    }
}
