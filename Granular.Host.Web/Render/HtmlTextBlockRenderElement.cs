using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlTextBlockRenderElement : HtmlRenderElement, ITextBlockRenderElement
    {
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                {
                    return;
                }

                text = value;
                renderQueue.InvokeAsync(() => HtmlElement.TextContent = text);
            }
        }

        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                Style.SetBounds(bounds, converter);
            }
        }

        private Brush foreground;
        public Brush Foreground
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
                Style.SetForeground(Foreground, converter);

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
                if (fontFamily == value)
                {
                    return;
                }

                fontFamily = value;
                Style.SetFontFamily(fontFamily, converter);
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                if (fontSize == value)
                {
                    return;
                }

                fontSize = value;
                Style.SetFontSize(fontSize, converter);
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                if (fontStyle == value)
                {
                    return;
                }

                fontStyle = value;
                Style.SetFontStyle(fontStyle, converter);
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                if (fontWeight == value)
                {
                    return;
                }

                fontWeight = value;
                Style.SetFontWeight(fontWeight, converter);
            }
        }

        private FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                if (fontStretch == value)
                {
                    return;
                }

                fontStretch = value;
                Style.SetFontStretch(fontStretch, converter);
            }
        }

        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                if (textAlignment == value)
                {
                    return;
                }

                textAlignment = value;
                Style.SetTextAlignment(textAlignment, converter);
            }
        }

        private TextTrimming textTrimming;
        public TextTrimming TextTrimming
        {
            get { return textTrimming; }
            set
            {
                if (textTrimming == value)
                {
                    return;
                }

                textTrimming = value;
                Style.SetTextTrimming(textTrimming);
            }
        }

        private TextWrapping textWrapping;
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set
            {
                if (textWrapping == value)
                {
                    return;
                }

                textWrapping = value;
                Style.SetTextWrapping(textWrapping, converter);
            }
        }

        private RenderQueue renderQueue;
        private IHtmlValueConverter converter;

        public HtmlTextBlockRenderElement(RenderQueue renderQueue, IHtmlValueConverter converter) :
            base(renderQueue)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            bounds = Rect.Zero;
            fontFamily = FontFamily.Default;
            fontSize = Double.NaN;

            Style.SetBounds(Bounds, converter);
            Style.SetForeground(Foreground, converter);
            Style.SetFontFamily(FontFamily, converter);
            Style.SetFontSize(FontSize, converter);
            Style.SetFontStyle(FontStyle, converter);
            Style.SetFontWeight(FontWeight, converter);
            Style.SetFontStretch(FontStretch, converter);
            Style.SetIsHitTestVisible(false);
            Style.SetTextAlignment(TextAlignment, converter);
            Style.SetTextTrimming(TextTrimming);
            Style.SetTextWrapping(TextWrapping, converter);
        }

        private void OnForegroundChanged(object sender, EventArgs e)
        {
            Style.SetForeground(Foreground, converter);
        }
    }
}
