using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows.Controls
{
    internal class TextBoxView : FrameworkElement
    {
        public event EventHandler TextChanged;
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

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.Text = text;
                }

                TextChanged.Raise(this);
            }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                if (maxLength == value)
                {
                    return;
                }

                maxLength = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.MaxLength = maxLength;
                }
            }
        }

        public event EventHandler CaretIndexChanged;
        private int caretIndex;
        public int CaretIndex
        {
            get { return caretIndex; }
            set
            {
                if (caretIndex == value)
                {
                    return;
                }

                caretIndex = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.CaretIndex = caretIndex;
                }

                CaretIndexChanged.Raise(this);
            }
        }

        public event EventHandler SelectionStartChanged;
        private int selectionStart;
        public int SelectionStart
        {
            get { return selectionStart; }
            set
            {
                if (selectionStart == value)
                {
                    return;
                }

                selectionStart = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.SelectionStart = selectionStart;
                }

                SelectionStartChanged.Raise(this);
            }
        }

        public event EventHandler SelectionLengthChanged;
        private int selectionLength;
        public int SelectionLength
        {
            get { return selectionLength; }
            set
            {
                if (selectionLength == value)
                {
                    return;
                }

                selectionLength = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.SelectionLength = selectionLength;
                }

                SelectionLengthChanged.Raise(this);
            }
        }

        private bool acceptsReturn;
        public bool AcceptsReturn
        {
            get { return acceptsReturn; }
            set
            {
                if (acceptsReturn == value)
                {
                    return;
                }

                acceptsReturn = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.AcceptsReturn = acceptsReturn;
                }
            }
        }

        private bool acceptsTab;
        public bool AcceptsTab
        {
            get { return acceptsTab; }
            set
            {
                if (acceptsTab == value)
                {
                    return;
                }

                acceptsTab = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.AcceptsTab = acceptsTab;
                }
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly == value)
                {
                    return;
                }

                isReadOnly = value;
                SetRenderElementsIsReadOnly();
            }
        }

        private ScrollBarVisibility horizontalScrollBarVisibility;
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return horizontalScrollBarVisibility; }
            set
            {
                if (horizontalScrollBarVisibility == value)
                {
                    return;
                }

                horizontalScrollBarVisibility = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.HorizontalScrollBarVisibility = horizontalScrollBarVisibility;
                }
            }
        }

        private ScrollBarVisibility verticalScrollBarVisibility;
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return verticalScrollBarVisibility; }
            set
            {
                if (verticalScrollBarVisibility == value)
                {
                    return;
                }

                verticalScrollBarVisibility = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.VerticalScrollBarVisibility = verticalScrollBarVisibility;
                }
            }
        }

        private bool spellCheck;
        public bool SpellCheck
        {
            get { return spellCheck; }
            set
            {
                if (spellCheck == value)
                {
                    return;
                }

                spellCheck = value;

                if (textBoxRenderElement != null)
                {
                    textBoxRenderElement.SpellCheck = spellCheck;
                }
            }
        }

        private bool isPassword;
        public bool IsPassword
        {
            get { return isPassword; }
            set
            {
                if (isPassword == value)
                {
                    return;
                }

                if (textBoxRenderElement != null)
                {
                    throw new Granular.Exception("Can't set TextBoxView.IsPassword after render elements have been created");
                }

                isPassword = value;
            }
        }

        private ITextBoxRenderElement textBoxRenderElement;
        private double measuredFontSize;
        private FontFamily measuredFontFamily;
        private double measuredLineHeight;

        static TextBoxView()
        {
            UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsIsHitTestVisible()));
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnIsEnabledChanged()));
            Control.ForegroundProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnForegroundChanged(e)));
            Control.FontFamilyProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnFontFamilyChanged(e)));
            Control.FontSizeProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnFontSizeChanged(e)));
            Control.FontStyleProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnFontStyleChanged(e)));
            Control.FontStretchProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnFontStretchChanged(e)));
            Control.FontWeightProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnFontWeightChanged(e)));
            TextBox.TextAlignmentProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnTextAlignmentChanged(e)));
            TextBox.TextWrappingProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnTextWrappingChanged(e)));
        }

        public TextBoxView()
        {
            measuredLineHeight = Double.NaN;
        }

        protected override object CreateRenderElementContentOverride(IRenderElementFactory factory)
        {
            if (textBoxRenderElement == null)
            {
                textBoxRenderElement = factory.CreateTextBoxRenderElement(this);

                textBoxRenderElement.CaretIndex = this.CaretIndex;
                textBoxRenderElement.SelectionLength = this.SelectionLength;
                textBoxRenderElement.SelectionStart = this.SelectionStart;
                textBoxRenderElement.Text = this.Text;
                textBoxRenderElement.MaxLength = this.MaxLength;
                textBoxRenderElement.Bounds = new Rect(this.VisualSize);
                textBoxRenderElement.AcceptsReturn = this.AcceptsReturn;
                textBoxRenderElement.AcceptsTab = this.AcceptsTab;
                textBoxRenderElement.IsPassword = isPassword;
                textBoxRenderElement.IsReadOnly = this.IsReadOnly || !this.IsEnabled;
                textBoxRenderElement.IsHitTestVisible = this.IsHitTestVisible && this.IsEnabled;
                textBoxRenderElement.SpellCheck = this.spellCheck;
                textBoxRenderElement.HorizontalScrollBarVisibility = this.HorizontalScrollBarVisibility;
                textBoxRenderElement.VerticalScrollBarVisibility = this.VerticalScrollBarVisibility;

                textBoxRenderElement.Foreground = (Brush)GetValue(Control.ForegroundProperty);
                textBoxRenderElement.FontSize = (double)GetValue(Control.FontSizeProperty);
                textBoxRenderElement.FontFamily = (FontFamily)GetValue(Control.FontFamilyProperty);
                textBoxRenderElement.FontStretch = (FontStretch)GetValue(Control.FontStretchProperty);
                textBoxRenderElement.FontStyle = (FontStyle)GetValue(Control.FontStyleProperty);
                textBoxRenderElement.FontWeight = (FontWeight)GetValue(Control.FontWeightProperty);
                textBoxRenderElement.TextAlignment = (TextAlignment)GetValue(TextBox.TextAlignmentProperty);
                textBoxRenderElement.TextWrapping = (TextWrapping)GetValue(TextBox.TextWrappingProperty);

                textBoxRenderElement.TextChanged += (sender, e) => this.Text = textBoxRenderElement.Text;
                textBoxRenderElement.CaretIndexChanged += (sender, e) => this.CaretIndex = ((ITextBoxRenderElement)sender).CaretIndex;
                textBoxRenderElement.SelectionStartChanged += (sender, e) => this.SelectionStart = ((ITextBoxRenderElement)sender).SelectionStart;
                textBoxRenderElement.SelectionLengthChanged += (sender, e) => this.SelectionLength = ((ITextBoxRenderElement)sender).SelectionLength;

                InvalidateMeasure();
            }

            return textBoxRenderElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, GetLineHeight());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect bounds = new Rect(finalSize);

            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.Bounds = bounds;
            }

            return finalSize;
        }

        public void FocusRenderElement()
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.Focus();
            }
        }

        public void ClearFocusRenderElement()
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.ClearFocus();
            }
        }

        public void ProcessRenderElementKeyEvent(KeyEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.ProcessKeyEvent(e);
            }
        }

        private void OnIsEnabledChanged()
        {
            SetRenderElementsIsHitTestVisible();
            SetRenderElementsIsReadOnly();
        }

        private void OnForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.Foreground = (Brush)e.NewValue;
            }
        }

        private void OnFontFamilyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.FontFamily = (FontFamily)e.NewValue;
            }
        }

        private void OnFontSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.FontSize = (double)e.NewValue;
            }
        }

        private void OnFontStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.FontStyle = (FontStyle)e.NewValue;
            }
        }

        private void OnFontStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.FontStretch = (FontStretch)e.NewValue;
            }
        }

        private void OnFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.FontWeight = (FontWeight)e.NewValue;
            }
        }

        private void OnTextAlignmentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.TextAlignment = (TextAlignment)e.NewValue;
            }
        }

        private void OnTextWrappingChanged(DependencyPropertyChangedEventArgs e)
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.TextWrapping = (TextWrapping)e.NewValue;
            }
        }

        private void SetRenderElementsIsHitTestVisible()
        {
            textBoxRenderElement.IsHitTestVisible = IsHitTestVisible && IsEnabled;
        }

        private void SetRenderElementsIsReadOnly()
        {
            if (textBoxRenderElement != null)
            {
                textBoxRenderElement.IsReadOnly = IsReadOnly || !IsEnabled;
            }
        }

        private double GetLineHeight()
        {
            double fontSize = (double)GetValue(Control.FontSizeProperty);
            FontFamily fontFamily = (FontFamily)GetValue(Control.FontFamilyProperty);

            if (!measuredLineHeight.IsNaN() && measuredFontSize.IsClose(fontSize) && measuredFontFamily == fontFamily)
            {
                return measuredLineHeight;
            }

            measuredFontSize = fontSize;
            measuredFontFamily = fontFamily;
            measuredLineHeight = ApplicationHost.Current.TextMeasurementService.Measure(String.Empty, fontSize, new Typeface(fontFamily), Double.PositiveInfinity).Height;

            return measuredLineHeight;
        }
    }
}
