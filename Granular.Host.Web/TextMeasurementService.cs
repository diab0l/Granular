using System;
using System.Collections.Generic;
using Bridge.Html5;
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
        private HTMLElement htmlElement;

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

            htmlElement.SetHtmlStyleProperty("position", "absolute");
            htmlElement.SetHtmlStyleProperty("visibility", "hidden");
            htmlElement.SetHtmlFontSize(fontSize, converter);
            htmlElement.SetHtmlFontFamily(typeface.FontFamily, converter);
            htmlElement.SetHtmlFontStretch(typeface.Stretch, converter);
            htmlElement.SetHtmlFontStyle(typeface.Style, converter);
            htmlElement.SetHtmlFontWeight(typeface.Weight, converter);

            if (maxWidth.IsNaN() || !Double.IsFinite(maxWidth))
            {
                htmlElement.SetHtmlTextWrapping(TextWrapping.NoWrap, converter);
                htmlElement.ClearHtmlStyleProperty("max-width");
            }
            else
            {
                htmlElement.SetHtmlTextWrapping(TextWrapping.Wrap, converter);
                htmlElement.SetHtmlStyleProperty("max-width", converter.ToPixelString(maxWidth));
            }

            htmlElement.InnerHTML = converter.ToHtmlContentString(text.DefaultIfNullOrEmpty("A"));

            return new Size(text.IsNullOrEmpty() ? 0 : htmlElement.OffsetWidth + 2, htmlElement.OffsetHeight);
        }
    }
}
