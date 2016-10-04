using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host
{
    public class HtmlStyleDictionary
    {
        public event EventHandler Invalidated;
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid == value)
                {
                    return;
                }

                isValid = value;

                if (!isValid)
                {
                    Invalidated.Raise(this);
                }
            }
        }

        private Bridge.Html5.HTMLElement element;

        private Dictionary<string, string> dictionary;
        private Dictionary<string, string> setProperties;
        private HashSet<string> clearProperties;

        public HtmlStyleDictionary(Bridge.Html5.HTMLElement element)
        {
            this.element = element;

            dictionary = new Dictionary<string, string>();
            setProperties = new Dictionary<string, string>();
            clearProperties = new HashSet<string>();

            IsValid = true;
        }

        public void SetValue(string key, string value)
        {
            string currentValue;
            if (dictionary.TryGetValue(key, out currentValue) && currentValue == value)
            {
                return;
            }

            dictionary[key] = value;
            setProperties[key] = value;
            clearProperties.Remove(key);

            IsValid = false;
        }

        public void ClearValue(string key)
        {
            if (!dictionary.ContainsKey(key))
            {
                return;
            }

            dictionary.Remove(key);
            setProperties.Remove(key);
            clearProperties.Add(key);

            IsValid = false;
        }

        public void Apply()
        {
            if (IsValid)
            {
                return;
            }

            foreach (KeyValuePair<string, string> pair in setProperties)
            {
                element.Style.SetProperty(pair.Key, pair.Value);
            }

            foreach (string key in clearProperties)
            {
                element.Style.RemoveProperty(key);
            }

            setProperties.Clear();
            clearProperties.Clear();

            IsValid = true;
        }
    }

    internal static class HtmlStyleDictionaryExtensions
    {
        public static void SetBackground(this HtmlStyleDictionary style, Brush background, Rect targetRect, IHtmlValueConverter converter)
        {
            style.ClearValue("background-color");
            style.ClearValue("background-image");

            if (background is SolidColorBrush)
            {
                style.SetValue("background-color", converter.ToColorString((SolidColorBrush)background));
            }
            else if (background != null)
            {
                style.SetValue("background-image", converter.ToImageString(background, targetRect));
            }
        }

        public static void SetBackgroundLocation(this HtmlStyleDictionary style, Point location, IHtmlValueConverter converter)
        {
            if (Point.IsNullOrEmpty(location))
            {
                style.ClearValue("background-position");
            }
            else
            {
                style.SetValue("background-position", converter.ToPixelString(location));
            }
        }

        public static void SetBackgroundSize(this HtmlStyleDictionary style, Size size, IHtmlValueConverter converter)
        {
            if (Size.IsNullOrEmpty(size))
            {
                style.ClearValue("background-size");
            }
            else
            {
                style.SetValue("background-size", converter.ToPixelString(size));
            }
        }

        public static void SetBackgroundBounds(this HtmlStyleDictionary style, Rect bounds, IHtmlValueConverter converter)
        {
            style.SetBackgroundLocation(bounds.Location, converter);
            style.SetBackgroundSize(bounds.Size, converter);
        }

        public static void SetBorderThickness(this HtmlStyleDictionary style, Thickness borderThickness, IHtmlValueConverter converter)
        {
            if (borderThickness == Thickness.Zero)
            {
                style.ClearValue("border-style");
                style.ClearValue("border-width");
                style.ClearValue("border-image-slice");
            }
            else
            {
                style.SetValue("border-style", "solid");
                style.SetValue("border-width", converter.ToPixelString(borderThickness));
                style.SetValue("border-image-slice", converter.ToImplicitValueString(borderThickness));
            }
        }

        public static void SetBorderBrush(this HtmlStyleDictionary style, Brush borderBrush, Size targetSize, IHtmlValueConverter converter)
        {
            style.ClearValue("border-color");
            style.ClearValue("border-image-source");

            if (borderBrush is SolidColorBrush)
            {
                style.SetValue("border-color", converter.ToColorString((SolidColorBrush)borderBrush));
            }
            else if (borderBrush != null)
            {
                style.SetValue("border-image-source", converter.ToImageString(borderBrush, new Rect(targetSize)));
            }
        }

        public static void SetBounds(this HtmlStyleDictionary style, Rect bounds, IHtmlValueConverter converter)
        {
            SetLocation(style, bounds.Location, converter);
            SetSize(style, bounds.Size, converter);
        }

        public static void SetLocation(this HtmlStyleDictionary style, Point location, IHtmlValueConverter converter)
        {
            style.SetValue("position", "absolute");
            style.SetValue("left", converter.ToPixelString(location.X));
            style.SetValue("top", converter.ToPixelString(location.Y));
        }

        public static void SetSize(this HtmlStyleDictionary style, Size size, IHtmlValueConverter converter)
        {
            if (size.Width.IsNaN())
            {
                style.ClearValue("width");
            }
            else
            {
                style.SetValue("width", converter.ToPixelString(size.Width));
            }

            if (size.Height.IsNaN())
            {
                style.ClearValue("height");
            }
            else
            {
                style.SetValue("height", converter.ToPixelString(size.Height));
            }
        }

        public static void SetClipToBounds(this HtmlStyleDictionary style, bool clipToBounds)
        {
            style.SetValue("overflow", clipToBounds ? "hidden" : "visible");
        }

        public static void SetIsHitTestVisible(this HtmlStyleDictionary style, bool isHitTestVisible)
        {
            style.SetValue("pointer-events", isHitTestVisible ? "auto" : "none");
        }

        public static void SetIsVisible(this HtmlStyleDictionary style, bool isVisible)
        {
            if (isVisible)
            {
                style.ClearValue("display");
            }
            else
            {
                style.SetValue("display", "none");
            }
        }

        public static void SetCornerRadius(this HtmlStyleDictionary style, CornerRadius cornerRadius, IHtmlValueConverter converter)
        {
            style.ClearValue("border-radius");
            style.ClearValue("border-top-left-radius");
            style.ClearValue("border-top-right-radius");
            style.ClearValue("border-bottom-left-radius");
            style.ClearValue("border-bottom-right-radius");

            if (cornerRadius != CornerRadius.Zero)
            {
                if (cornerRadius.IsUniform)
                {
                    style.SetValue("border-radius", converter.ToPixelString(cornerRadius.TopLeft));
                }
                else
                {
                    style.SetValue("border-top-left-radius", converter.ToPixelString(cornerRadius.TopLeft));
                    style.SetValue("border-top-right-radius", converter.ToPixelString(cornerRadius.TopRight));
                    style.SetValue("border-bottom-left-radius", converter.ToPixelString(cornerRadius.BottomLeft));
                    style.SetValue("border-bottom-right-radius", converter.ToPixelString(cornerRadius.BottomRight));
                }
            }
        }

        public static void SetForeground(this HtmlStyleDictionary style, Brush foreground, IHtmlValueConverter converter)
        {
            if (foreground == null)
            {
                style.ClearValue("color");
            }
            else if (foreground is SolidColorBrush)
            {
                style.SetValue("color", converter.ToColorString((SolidColorBrush)foreground));
            }
            else
            {
                throw new Granular.Exception("A \"{0}\" foreground brush is not supported", foreground.GetType());
            }
        }

        public static void SetOpacity(this HtmlStyleDictionary style, double opacity, IHtmlValueConverter converter)
        {
            if (opacity == 1.0)
            {
                style.ClearValue("opacity");
            }
            else
            {
                style.SetValue("opacity", converter.ToImplicitValueString(opacity));
            }
        }

        public static void SetTransform(this HtmlStyleDictionary style, Matrix transform, IHtmlValueConverter converter)
        {
            if (transform == Matrix.Identity)
            {
                style.ClearValue("transform");
                style.ClearValue("transform-origin");
            }
            else
            {
                style.SetValue("transform", converter.ToTransformString(transform));
                style.SetValue("transform-origin", "0 0");
            }
        }

        public static void SetFontFamily(this HtmlStyleDictionary style, FontFamily fontFamily, IHtmlValueConverter converter)
        {
            if (!fontFamily.FamilyNames.Any())
            {
                style.ClearValue("font-family");
            }
            else
            {
                style.SetValue("font-family", converter.ToFontFamilyNamesString(fontFamily));
            }
        }

        public static void SetFontSize(this HtmlStyleDictionary style, double fontSize, IHtmlValueConverter converter)
        {
            if (fontSize.IsNaN())
            {
                style.ClearValue("font-size");
            }
            else
            {
                style.SetValue("font-size", converter.ToPixelString(fontSize));
            }
        }

        public static void SetFontStyle(this HtmlStyleDictionary style, FontStyle fontStyle, IHtmlValueConverter converter)
        {
            if (fontStyle == FontStyle.Normal)
            {
                style.ClearValue("font-style");
            }
            else
            {
                style.SetValue("font-style", converter.ToFontStyleString(fontStyle));
            }
        }

        public static void SetFontWeight(this HtmlStyleDictionary style, FontWeight fontWeight, IHtmlValueConverter converter)
        {
            if (fontWeight == FontWeight.Normal)
            {
                style.ClearValue("font-weight");
            }
            else
            {
                style.SetValue("font-weight", converter.ToFontWeightString(fontWeight));
            }
        }

        public static void SetFontStretch(this HtmlStyleDictionary style, FontStretch fontStretch, IHtmlValueConverter converter)
        {
            if (fontStretch == FontStretch.Normal)
            {
                style.ClearValue("font-stretch");
            }
            else
            {
                style.SetValue("font-stretch", converter.ToFontStretchString(fontStretch));
            }
        }

        public static void SetTextAlignment(this HtmlStyleDictionary style, TextAlignment textAlignment, IHtmlValueConverter converter)
        {
            style.SetValue("text-align", converter.ToTextAlignmentString(textAlignment));
        }

        public static void SetTextTrimming(this HtmlStyleDictionary style, TextTrimming textTrimming)
        {
            if (textTrimming == TextTrimming.None)
            {
                style.ClearValue("text-overflow");
            }
            else
            {
                style.SetValue("text-overflow", "ellipsis");
            }
        }

        public static void SetTextWrapping(this HtmlStyleDictionary style, TextWrapping textWrapping, IHtmlValueConverter converter)
        {
            style.SetValue("white-space", converter.ToWhiteSpaceString(textWrapping));
        }

        public static void SetHorizontalScrollBarVisibility(this HtmlStyleDictionary style, ScrollBarVisibility scrollBarVisibility, IHtmlValueConverter converter)
        {
            style.SetValue("overflow-x", converter.ToOverflowString(scrollBarVisibility));
        }

        public static void SetVerticalScrollBarVisibility(this HtmlStyleDictionary style, ScrollBarVisibility scrollBarVisibility, IHtmlValueConverter converter)
        {
            style.SetValue("overflow-y", converter.ToOverflowString(scrollBarVisibility));
        }

        public static void SetBackgroundImage(this HtmlStyleDictionary style, ImageSource imageSource, IHtmlValueConverter converter)
        {
            if (imageSource == null)
            {
                style.ClearValue("background-image");
            }
            else
            {
                style.SetValue("background-image", converter.ToUrlString(((RenderImageSource)imageSource.RenderImageSource).Url));
            }
        }
    }
}
