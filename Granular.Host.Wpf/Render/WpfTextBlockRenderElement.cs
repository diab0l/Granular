extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfTextBlockRenderElement : System.Windows.Media.ITextBlockRenderElement, IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return textBlock; } }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(textBlock, bounds.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(textBlock, bounds.Top);
                textBlock.Width = bounds.Width;
                textBlock.Height = bounds.Height;
            }
        }

        private System.Windows.Media.Brush foreground;
        public System.Windows.Media.Brush Foreground
        {
            get { return foreground; }
            set
            {
                if (foreground == value)
                {
                    return;
                }

                if (foreground != null)
                {
                    foreground.Changed -= OnForegroundChanged;
                }

                foreground = value;
                textBlock.Foreground = converter.Convert(foreground);

                if (foreground != null)
                {
                    foreground.Changed += OnForegroundChanged;
                }
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                textBlock.FontFamily = converter.Convert(fontFamily);
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                textBlock.FontSize = fontSize;
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                fontStyle = value;
                textBlock.FontStyle = converter.Convert(fontStyle);
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                fontWeight = value;
                textBlock.FontWeight = converter.Convert(fontWeight);
            }
        }

        private FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                fontStretch = value;
                textBlock.FontStretch = converter.Convert(fontStretch);
            }
        }

        public string Text
        {
            get { return textBlock.Text; }
            set { textBlock.Text = value; }
        }

        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                textBlock.TextAlignment = converter.Convert(textAlignment);
            }
        }

        private TextTrimming textTrimming;
        public TextTrimming TextTrimming
        {
            get { return textTrimming; }
            set
            {
                textTrimming = value;
                textBlock.TextTrimming = converter.Convert(textTrimming);
            }
        }

        private TextWrapping textWrapping;
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set
            {
                textWrapping = value;
                textBlock.TextWrapping = converter.Convert(textWrapping);
            }
        }

        private wpf::System.Windows.Controls.TextBlock textBlock;
        private IWpfValueConverter converter;

        public WpfTextBlockRenderElement(IWpfValueConverter converter)
        {
            this.converter = converter;
            textBlock = new wpf::System.Windows.Controls.TextBlock();
            textBlock.Foreground = wpf::System.Windows.Media.Brushes.Red;
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            textBlock.Foreground = converter.Convert(Foreground);
        }
    }
}
