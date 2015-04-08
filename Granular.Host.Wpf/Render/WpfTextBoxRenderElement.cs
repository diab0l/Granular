extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfTextBoxRenderElement : ITextBoxRenderElement, IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return textBox; } }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(textBox, bounds.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(textBox, bounds.Top);
                textBox.Width = bounds.Width;
                textBox.Height = bounds.Height;
            }
        }

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

                if (textBox.Text != text)
                {
                    textBox.Text = text;
                    textBox.UpdateLayout();
                }

                TextChanged.Raise(this);
            }
        }

        public int MaxLength
        {
            get { return textBox.MaxLength; }
            set { textBox.MaxLength = value; }
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
                if (textBox.CaretIndex != caretIndex)
                {
                    textBox.CaretIndex = caretIndex;
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
                if (textBox.SelectionStart != selectionStart)
                {
                    textBox.SelectionStart = selectionStart;
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
                if (textBox.SelectionLength != selectionLength)
                {
                    textBox.SelectionLength = selectionLength;
                }
                SelectionLengthChanged.Raise(this);
            }
        }

        public bool AcceptsReturn
        {
            get { return textBox.AcceptsReturn; }
            set { textBox.AcceptsReturn = value; }
        }

        public bool AcceptsTab
        {
            get { return textBox.AcceptsTab; }
            set { textBox.AcceptsTab = value; }
        }

        public bool IsReadOnly
        {
            get { return textBox.IsReadOnly; }
            set { textBox.IsReadOnly = value; }
        }

        public bool SpellCheck
        {
            get { return (bool)textBox.GetValue(wpf::System.Windows.Controls.SpellCheck.IsEnabledProperty); }
            set { textBox.SetValue(wpf::System.Windows.Controls.SpellCheck.IsEnabledProperty, value); }
        }

        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                textBox.TextAlignment = converter.Convert(textAlignment);
            }
        }

        private Brush foreground;
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                textBox.Foreground = converter.Convert(foreground);
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                textBox.FontFamily = converter.Convert(fontFamily);
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                textBox.FontSize = fontSize;
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                fontStyle = value;
                textBox.FontStyle = converter.Convert(fontStyle);
            }
        }

        private FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                fontStretch = value;
                textBox.FontStretch = converter.Convert(fontStretch);
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                fontWeight = value;
                textBox.FontWeight = converter.Convert(fontWeight);
            }
        }

        public bool IsHitTestVisible
        {
            get { return textBox.IsHitTestVisible; }
            set { textBox.IsHitTestVisible = value; }
        }

        private TextWrapping textWrapping;
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set
            {
                textWrapping = value;
                textBox.TextWrapping = converter.Convert(textWrapping);
            }
        }

        private ScrollBarVisibility horizontalScrollBarVisibility;
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return horizontalScrollBarVisibility; }
            set
            {
                horizontalScrollBarVisibility = value;
                textBox.SetValue(wpf::System.Windows.Controls.ScrollViewer.HorizontalScrollBarVisibilityProperty, converter.Convert(horizontalScrollBarVisibility));
            }
        }

        private ScrollBarVisibility verticalScrollBarVisibility;
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return verticalScrollBarVisibility; }
            set
            {
                verticalScrollBarVisibility = value;
                textBox.SetValue(wpf::System.Windows.Controls.ScrollViewer.VerticalScrollBarVisibilityProperty, converter.Convert(verticalScrollBarVisibility));
            }
        }

        private wpf::System.Windows.Controls.TextBox textBox;
        private IWpfValueConverter converter;

        public WpfTextBoxRenderElement(IWpfValueConverter converter)
        {
            this.converter = converter;

            textBox = new wpf.System.Windows.Controls.TextBox { BorderThickness = new wpf::System.Windows.Thickness(), Background = wpf::System.Windows.Media.Brushes.Transparent };

            textBox.TextChanged += (sender, e) => this.Text = textBox.Text;
            textBox.SelectionChanged += (sender, e) =>
            {
                this.CaretIndex = textBox.CaretIndex;
                this.SelectionStart = textBox.SelectionStart;
                this.SelectionLength = textBox.SelectionLength;
            };
        }

        public void Focus()
        {
            textBox.Focus();
        }

        public void ClearFocus()
        {
            wpf::System.Windows.Input.Keyboard.ClearFocus();
        }
    }
}
