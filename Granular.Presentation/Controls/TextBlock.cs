using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Documents;

namespace System.Windows.Controls
{
    [ContentProperty("Text")]
    public class TextBlock : FrameworkElement
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnTextChanged(e)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.Foreground = (Brush)e.NewValue)));
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnFontFamilyChanged(e)));
        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnFontSizeChanged(e)));
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnFontStyleChanged(e)));
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnFontWeightChanged(e)));
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).OnFontStretchChanged(e)));
        public FontStretch FontStretch
        {
            get { return (FontStretch)GetValue(FontStretchProperty); }
            set { SetValue(FontStretchProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty = Block.TextAlignmentProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextAlignment = (TextAlignment)e.NewValue)));
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextTrimming = (TextTrimming)e.NewValue)));
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextWrapping = (TextWrapping)e.NewValue)));
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        private Dictionary<IRenderElementFactory, ITextBlockRenderElement> textBlockRenderElements;

        private MeasureCache measureCache;

        public TextBlock()
        {
            measureCache = new MeasureCache(4);
            textBlockRenderElements = new Dictionary<IRenderElementFactory, ITextBlockRenderElement>();
        }

        protected override object CreateContentRenderElementOverride(IRenderElementFactory factory)
        {
            ITextBlockRenderElement textBlockRenderElement;
            if (textBlockRenderElements.TryGetValue(factory, out textBlockRenderElement))
            {
                return textBlockRenderElement;
            }

            textBlockRenderElement = factory.CreateTextBlockRenderElement(this);

            textBlockRenderElement.Bounds = new Rect(VisualBounds.Size);
            textBlockRenderElement.FontFamily = this.FontFamily;
            textBlockRenderElement.Foreground = this.Foreground;
            textBlockRenderElement.FontSize = this.FontSize;
            textBlockRenderElement.FontStyle = this.FontStyle;
            textBlockRenderElement.FontStretch = this.FontStretch;
            textBlockRenderElement.FontWeight = this.FontWeight;
            textBlockRenderElement.Text = this.Text;
            textBlockRenderElement.TextAlignment = this.TextAlignment;
            textBlockRenderElement.TextTrimming = this.TextTrimming;
            textBlockRenderElement.TextWrapping = this.TextWrapping;

            textBlockRenderElements.Add(factory, textBlockRenderElement);

            return textBlockRenderElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size measuredSize;

            if (measureCache.TryGetMeasure(Size.FromWidth(availableSize.Width), out measuredSize))
            {
                return measuredSize;
            }

            measuredSize = ApplicationHost.Current.TextMeasurementService.Measure(Text ?? String.Empty, FontSize, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), availableSize.Width);
            measureCache.SetMeasure(Size.FromWidth(availableSize.Width), measuredSize);

            return measuredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (ITextBlockRenderElement textBlockRenderElement in textBlockRenderElements.Values)
            {
                textBlockRenderElement.Bounds = new Rect(finalSize);
            }

            return finalSize;
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.Text = Text);
            measureCache.Clear();
        }

        private void OnFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontFamily = FontFamily);
            measureCache.Clear();
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontSize = FontSize);
            measureCache.Clear();
        }

        private void OnFontStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontStyle = FontStyle);
            measureCache.Clear();
        }

        private void OnFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontWeight = FontWeight);
            measureCache.Clear();
        }

        private void OnFontStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontStretch = FontStretch);
            measureCache.Clear();
        }

        private void SetRenderElementsProperty(Action<ITextBlockRenderElement> setter)
        {
            foreach (ITextBlockRenderElement textBlockRenderElement in textBlockRenderElements.Values)
            {
                setter(textBlockRenderElement);
            }
        }
    }
}
