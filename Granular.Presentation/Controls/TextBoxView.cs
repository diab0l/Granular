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
                SetRenderElementsProperty(renderElement => renderElement.Text = text);
                InvalidateMeasure();
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
                SetRenderElementsProperty(renderElement => renderElement.MaxLength = maxLength);
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
                SetRenderElementsProperty(renderElement => renderElement.CaretIndex = caretIndex);
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
                SetRenderElementsProperty(renderElement => renderElement.SelectionStart = selectionStart);
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
                SetRenderElementsProperty(renderElement => renderElement.SelectionLength = selectionLength);
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
                SetRenderElementsProperty(renderElement => renderElement.AcceptsReturn = acceptsReturn);
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
                SetRenderElementsProperty(renderElement => renderElement.AcceptsTab = acceptsTab);
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
                SetRenderElementsProperty(renderElement => renderElement.HorizontalScrollBarVisibility = horizontalScrollBarVisibility);
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
                SetRenderElementsProperty(renderElement => renderElement.VerticalScrollBarVisibility = verticalScrollBarVisibility);
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
                SetRenderElementsProperty(renderElement => renderElement.SpellCheck = spellCheck);
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

                if (textBoxRenderElements.Count > 0)
                {
                    throw new Granular.Exception("Can't set TextBoxView.IsPassword after render elements have been created");
                }

                isPassword = value;
            }
        }

        private Dictionary<IRenderElementFactory, ITextBoxRenderElement> textBoxRenderElements;
        double measuredFontSize;
        FontFamily measuredFontFamily;
        double measuredLineHeight;

        static TextBoxView()
        {
            UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsIsHitTestVisible()));
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).OnIsEnabledChanged()));
            Control.ForegroundProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.Foreground = (Brush)e.NewValue)));
            Control.FontFamilyProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.FontFamily = (FontFamily)e.NewValue)));
            Control.FontSizeProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.FontSize = (double)e.NewValue)));
            Control.FontStyleProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.FontStyle = (FontStyle)e.NewValue)));
            Control.FontStretchProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.FontStretch = (FontStretch)e.NewValue)));
            Control.FontWeightProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.FontWeight = (FontWeight)e.NewValue)));
            TextBox.TextAlignmentProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.TextAlignment = (TextAlignment)e.NewValue)));
            TextBox.TextWrappingProperty.OverrideMetadata(typeof(TextBoxView), new FrameworkPropertyMetadata(inherits: true, propertyChangedCallback: (sender, e) => ((TextBoxView)sender).SetRenderElementsProperty(renderElement => renderElement.TextWrapping = (TextWrapping)e.NewValue)));
        }

        public TextBoxView()
        {
            textBoxRenderElements = new Dictionary<IRenderElementFactory, ITextBoxRenderElement>();
            measuredLineHeight = Double.NaN;
        }

        private void SetRenderElementsProperty(Action<ITextBoxRenderElement> setter)
        {
            foreach (ITextBoxRenderElement textBoxRenderElement in textBoxRenderElements.Values)
            {
                setter(textBoxRenderElement);
            }
        }

        protected override object CreateContentRenderElementOverride(IRenderElementFactory factory)
        {
            ITextBoxRenderElement textBoxRenderElement;
            if (textBoxRenderElements.TryGetValue(factory, out textBoxRenderElement))
            {
                return textBoxRenderElement;
            }

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

            textBoxRenderElements.Add(factory, textBoxRenderElement);

            InvalidateMeasure();

            return textBoxRenderElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, GetLineHeight());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect bounds = new Rect(finalSize);
            SetRenderElementsProperty(renderElement => renderElement.Bounds = bounds);
            return finalSize;
        }

        public void FocusRenderElement()
        {
            foreach (ITextBoxRenderElement textBoxRenderElement in textBoxRenderElements.Values)
            {
                textBoxRenderElement.Focus();
            }
        }

        public void ClearFocusRenderElement()
        {
            foreach (ITextBoxRenderElement textBoxRenderElement in textBoxRenderElements.Values)
            {
                textBoxRenderElement.ClearFocus();
            }
        }

        public void ProcessRenderElementKeyEvent(KeyEventArgs e)
        {
            foreach (ITextBoxRenderElement textBoxRenderElement in textBoxRenderElements.Values)
            {
                textBoxRenderElement.ProcessKeyEvent(e);
            }
        }

        private void OnIsEnabledChanged()
        {
            SetRenderElementsIsHitTestVisible();
            SetRenderElementsIsReadOnly();
        }

        private void SetRenderElementsIsHitTestVisible()
        {
            SetRenderElementsProperty(renderElement => renderElement.IsHitTestVisible = IsHitTestVisible && IsEnabled);
        }

        private void SetRenderElementsIsReadOnly()
        {
            SetRenderElementsProperty(renderElement => renderElement.IsReadOnly = IsReadOnly || !IsEnabled);
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
