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

        public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.Foreground = (Brush)e.NewValue)));
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

        public static readonly DependencyProperty TextAlignmentProperty = Block.TextAlignmentProperty.AddOwner(typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextAlignment = (TextAlignment)e.NewValue)));
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(TextBlock), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextTrimming = (TextTrimming)e.NewValue)));
        public TextTrimming TextTrimming
        {
            get { return (TextTrimming)GetValue(TextTrimmingProperty); }
            set { SetValue(TextTrimmingProperty, value); }
        }

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBlock), new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBlock)sender).textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.TextWrapping = (TextWrapping)e.NewValue)));
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        private RenderElementDictionary<ITextBlockRenderElement> textBlockRenderElements;
        private Size noWrapSize;

        public TextBlock()
        {
            textBlockRenderElements = new RenderElementDictionary<ITextBlockRenderElement>(CreateRenderElement);
        }

        protected override object CreateContentRenderElementOverride(IRenderElementFactory factory)
        {
            return textBlockRenderElements.GetRenderElement(factory);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (noWrapSize == null)
            {
                noWrapSize = ApplicationHost.Current.TextMeasurementService.Measure(Text ?? String.Empty, FontSize, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), Double.PositiveInfinity);
            }

            if (TextWrapping == TextWrapping.NoWrap || noWrapSize.Width <= availableSize.Width)
            {
                return noWrapSize;
            }

            return ApplicationHost.Current.TextMeasurementService.Measure(Text ?? String.Empty, FontSize, new Typeface(FontFamily, FontStyle, FontWeight, FontStretch), availableSize.Width);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (ITextBlockRenderElement textBlockRenderElement in textBlockRenderElements.Elements)
            {
                textBlockRenderElement.Bounds = new Rect(finalSize);
            }

            return finalSize;
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.Text = Text);
            noWrapSize = null;
        }

        private void OnFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontFamily = FontFamily);
            noWrapSize = null;
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontSize = FontSize);
            noWrapSize = null;
        }

        private void OnFontStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontStyle = FontStyle);
            noWrapSize = null;
        }

        private void OnFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontWeight = FontWeight);
            noWrapSize = null;
        }

        private void OnFontStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            textBlockRenderElements.SetRenderElementsProperty(textBlockRenderElement => textBlockRenderElement.FontStretch = FontStretch);
            noWrapSize = null;
        }

        private ITextBlockRenderElement CreateRenderElement(IRenderElementFactory factory)
        {
            ITextBlockRenderElement textBlockRenderElement = factory.CreateTextBlockRenderElement(this);

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

            return textBlockRenderElement;
        }
    }
}
