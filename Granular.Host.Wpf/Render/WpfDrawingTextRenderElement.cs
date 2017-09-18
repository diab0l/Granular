extern alias wpf;

using System;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfDrawingTextRenderElement : IDrawingTextRenderElement, IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return element; } }

        private FormattedText formattedText;
        public FormattedText FormattedText
        {
            get { return formattedText; }
            set
            {
                if (formattedText == value)
                {
                    return;
                }

                formattedText = value;
                SetFormattedText();
            }
        }

        private Point origin;
        public Point Origin
        {
            get { return origin; }
            set
            {
                if (origin == value)
                {
                    return;
                }

                origin = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(element, origin.X);
                wpf::System.Windows.Controls.Canvas.SetTop(element, origin.Y);
            }
        }

        private wpf::System.Windows.Controls.TextBlock element;
        private WpfRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfDrawingTextRenderElement(WpfRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;
            this.element = new wpf::System.Windows.Controls.TextBlock();
        }

        private void SetFormattedText()
        {
            element.Text = FormattedText.Text;
            element.FontFamily = converter.Convert(FormattedText.Typeface.FontFamily);
            element.FontStretch = converter.Convert(FormattedText.Typeface.Stretch);
            element.FontStyle = converter.Convert(FormattedText.Typeface.Style);
            element.FontWeight = converter.Convert(FormattedText.Typeface.Weight);
            element.FontSize = FormattedText.Size;
            element.Foreground = converter.Convert(FormattedText.Foreground, factory);
            //element.FlowDirection = converter.Convert(FormattedText.FlowDirection);
            element.LineHeight = FormattedText.LineHeight;
            //element.LineCount = converter.Convert(FormattedText.MaxLineCount);
            element.MaxHeight = FormattedText.MaxTextHeight;
            element.MaxWidth = FormattedText.MaxTextWidth;
            element.TextAlignment = converter.Convert(FormattedText.TextAlignment);
            element.TextTrimming = converter.Convert(FormattedText.Trimming);
        }
    }
}