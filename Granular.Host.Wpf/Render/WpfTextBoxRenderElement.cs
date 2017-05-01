extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfTextBoxRenderElement : ITextBoxRenderElement, IWpfRenderElement
    {
        private interface ITextContenttAdapter
        {
            wpf::System.Windows.Controls.Control Control { get; }

            event EventHandler TextChanged;
            string Text { get; set; }

            event EventHandler CaretIndexChanged;
            int CaretIndex { get; set; }

            event EventHandler SelectionStartChanged;
            int SelectionLength { get; set; }

            event EventHandler SelectionLengthChanged;
            int SelectionStart { get; set; }

            bool IsReadOnly { get; set; }
        }

        private class TextBoxAdapter : ITextContenttAdapter
        {
            public wpf::System.Windows.Controls.Control Control { get { return textBox; } }

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
                    }

                    TextChanged.Raise(this);
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

            public bool IsReadOnly
            {
                get { return textBox.IsReadOnly; }
                set { textBox.IsReadOnly = value; }
            }

            private wpf::System.Windows.Controls.TextBox textBox;

            public TextBoxAdapter()
            {
                textBox = new wpf.System.Windows.Controls.TextBox { BorderThickness = new wpf::System.Windows.Thickness(), Background = wpf::System.Windows.Media.Brushes.Transparent };
                textBox.TextChanged += (sender, e) => this.Text = textBox.Text;
                textBox.SelectionChanged += (sender, e) =>
                {
                    this.CaretIndex = textBox.CaretIndex;
                    this.SelectionStart = textBox.SelectionStart;
                    this.SelectionLength = textBox.SelectionLength;
                };
            }
        }

        private class PasswordBoxAdapter : ITextContenttAdapter
        {
            public wpf::System.Windows.Controls.Control Control { get { return passwordBox; } }

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

                    if (passwordBox.Password != text)
                    {
                        passwordBox.Password = text;
                    }

                    TextChanged.Raise(this);
                }
            }

            public event EventHandler CaretIndexChanged { add { } remove { } }
            public int CaretIndex { get; set; }

            public event EventHandler SelectionStartChanged { add { } remove { } }
            public int SelectionLength { get; set; }

            public event EventHandler SelectionLengthChanged { add { } remove { } }
            public int SelectionStart { get; set; }

            public bool IsReadOnly
            {
                get { return !passwordBox.IsEnabled; }
                set { passwordBox.IsEnabled = !value; }
            }

            private wpf::System.Windows.Controls.PasswordBox passwordBox;

            public PasswordBoxAdapter()
            {
                passwordBox = new wpf::System.Windows.Controls.PasswordBox
                {
                    Template = (wpf::System.Windows.Controls.ControlTemplate)wpf::System.Windows.Markup.XamlReader.Parse(@"
<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' TargetType='{x:Type PasswordBox}'>
    <ScrollViewer x:Name='PART_ContentHost' SnapsToDevicePixels='{TemplateBinding UIElement.SnapsToDevicePixels}'/>
</ControlTemplate>")
                };

                passwordBox.PasswordChanged += (sender, e) => this.Text = passwordBox.Password;
            }
        }

        public wpf::System.Windows.FrameworkElement WpfElement { get { return contentAdapter.Control; } }

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
                SetContentBounds();
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
                contentAdapter.Text = text;
                TextChanged.Raise(this);
            }
        }

        private int maxLength;
        public int MaxLength
        {
            get { return maxLength; }
            set
            {
                maxLength = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.MaxLengthProperty, maxLength);
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
                contentAdapter.CaretIndex = caretIndex;
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
                contentAdapter.SelectionStart = selectionStart;
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
                contentAdapter.SelectionLength = selectionLength;
                SelectionLengthChanged.Raise(this);
            }
        }

        public bool acceptsReturn;
        public bool AcceptsReturn
        {
            get { return acceptsReturn; }
            set
            {
                acceptsReturn = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.AcceptsReturnProperty, acceptsReturn);
            }
        }

        private bool acceptsTab;
        public bool AcceptsTab
        {
            get { return acceptsTab; }
            set
            {
                acceptsTab = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.AcceptsTabProperty, acceptsTab);
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

                isPassword = value;
                SetTextContentAdapter();
            }
        }

        private bool isReadOnly;
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                contentAdapter.IsReadOnly = isReadOnly;
            }
        }

        private bool spellCheck;
        public bool SpellCheck
        {
            get { return spellCheck; }
            set
            {
                spellCheck = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.SpellCheck.IsEnabledProperty, spellCheck);
            }
        }

        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Documents.Block.TextAlignmentProperty, converter.Convert(textAlignment));
            }
        }

        private Brush foreground;
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                contentAdapter.Control.Foreground = converter.Convert(foreground, factory);
            }
        }

        private FontFamily fontFamily;
        public FontFamily FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                contentAdapter.Control.FontFamily = converter.Convert(fontFamily);
            }
        }

        private double fontSize;
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                contentAdapter.Control.FontSize = fontSize;
            }
        }

        private FontStyle fontStyle;
        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set
            {
                fontStyle = value;
                contentAdapter.Control.FontStyle = converter.Convert(fontStyle);
            }
        }

        private FontStretch fontStretch;
        public FontStretch FontStretch
        {
            get { return fontStretch; }
            set
            {
                fontStretch = value;
                contentAdapter.Control.FontStretch = converter.Convert(fontStretch);
            }
        }

        private FontWeight fontWeight;
        public FontWeight FontWeight
        {
            get { return fontWeight; }
            set
            {
                fontWeight = value;
                contentAdapter.Control.FontWeight = converter.Convert(fontWeight);
            }
        }

        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                isHitTestVisible = value;
                contentAdapter.Control.IsHitTestVisible = isHitTestVisible;
            }
        }

        private TextWrapping textWrapping;
        public TextWrapping TextWrapping
        {
            get { return textWrapping; }
            set
            {
                textWrapping = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBlock.TextWrappingProperty, converter.Convert(textWrapping));
            }
        }

        private ScrollBarVisibility horizontalScrollBarVisibility;
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return horizontalScrollBarVisibility; }
            set
            {
                horizontalScrollBarVisibility = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.ScrollViewer.HorizontalScrollBarVisibilityProperty, converter.Convert(horizontalScrollBarVisibility));
            }
        }

        private ScrollBarVisibility verticalScrollBarVisibility;
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return verticalScrollBarVisibility; }
            set
            {
                verticalScrollBarVisibility = value;
                contentAdapter.Control.SetValue(wpf::System.Windows.Controls.ScrollViewer.VerticalScrollBarVisibilityProperty, converter.Convert(verticalScrollBarVisibility));
            }
        }

        private ITextContenttAdapter contentAdapter;
        private IRenderElementFactory factory;
        private WpfValueConverter converter;

        public WpfTextBoxRenderElement(IRenderElementFactory factory, WpfValueConverter converter)
        {
            this.factory = factory;
            this.converter = converter;
            bounds = Rect.Zero;
            fontFamily = FontFamily.Default;
            fontSize = 11;

            SetTextContentAdapter();
        }

        private void SetTextContentAdapter()
        {
            contentAdapter = IsPassword ? (ITextContenttAdapter)new PasswordBoxAdapter() : new TextBoxAdapter();

            contentAdapter.TextChanged += (sender, e) => this.Text = contentAdapter.Text;
            contentAdapter.CaretIndexChanged += (sender, e) => this.CaretIndex = contentAdapter.CaretIndex;
            contentAdapter.SelectionStartChanged += (sender, e) => this.SelectionStart = contentAdapter.SelectionStart;
            contentAdapter.SelectionLengthChanged += (sender, e) => this.SelectionLength = contentAdapter.SelectionLength;
            contentAdapter.Control.AddHandler(wpf::System.Windows.Input.Mouse.QueryCursorEvent, (wpf::System.Windows.Input.QueryCursorEventHandler)((sender, e) =>
            {
                e.Cursor = wpf::System.Windows.Input.Cursors.Arrow;
                e.Handled = false;
            }), true);

            SetContentBounds();
            contentAdapter.Text = Text;
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.MaxLengthProperty, MaxLength);
            contentAdapter.CaretIndex = CaretIndex;
            contentAdapter.SelectionStart = SelectionStart;
            contentAdapter.SelectionLength = SelectionLength;
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.AcceptsReturnProperty, AcceptsReturn);
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBox.AcceptsTabProperty, AcceptsTab);
            contentAdapter.IsReadOnly = IsReadOnly;
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.SpellCheck.IsEnabledProperty, SpellCheck);
            contentAdapter.Control.SetValue(wpf::System.Windows.Documents.Block.TextAlignmentProperty, converter.Convert(TextAlignment));
            contentAdapter.Control.Foreground = converter.Convert(Foreground, factory);
            contentAdapter.Control.FontFamily = converter.Convert(FontFamily);
            contentAdapter.Control.FontSize = FontSize;
            contentAdapter.Control.FontStyle = converter.Convert(FontStyle);
            contentAdapter.Control.FontStretch = converter.Convert(FontStretch);
            contentAdapter.Control.FontWeight = converter.Convert(FontWeight);
            contentAdapter.Control.IsHitTestVisible = IsHitTestVisible;
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.TextBlock.TextWrappingProperty, converter.Convert(TextWrapping));
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.ScrollViewer.HorizontalScrollBarVisibilityProperty, converter.Convert(HorizontalScrollBarVisibility));
            contentAdapter.Control.SetValue(wpf::System.Windows.Controls.ScrollViewer.VerticalScrollBarVisibilityProperty, converter.Convert(VerticalScrollBarVisibility));
        }

        public void Focus()
        {
            contentAdapter.Control.Focus();
        }

        public void ClearFocus()
        {
            wpf::System.Windows.Window window = wpf::System.Windows.Window.GetWindow(contentAdapter.Control);
            wpf::System.Windows.Input.FocusManager.SetFocusedElement(window, null);
            wpf::System.Windows.Input.Keyboard.Focus(window);
        }

        private void SetContentBounds()
        {
            wpf::System.Windows.Controls.Canvas.SetLeft(contentAdapter.Control, bounds.Left);
            wpf::System.Windows.Controls.Canvas.SetTop(contentAdapter.Control, bounds.Top);
            contentAdapter.Control.Width = bounds.Width;
            contentAdapter.Control.Height = bounds.Height;
        }

        public void ProcessKeyEvent(KeyEventArgs e)
        {
            wpf::System.Windows.Input.KeyEventArgs wpfKeyEventArgs = new wpf::System.Windows.Input.KeyEventArgs(wpf::System.Windows.Input.Keyboard.PrimaryDevice, wpf::System.Windows.PresentationSource.FromVisual(contentAdapter.Control), 0, converter.Convert(e.Key));
            wpfKeyEventArgs.RoutedEvent = e.KeyStates == KeyStates.Down ? wpf::System.Windows.Input.Keyboard.KeyDownEvent : wpf::System.Windows.Input.Keyboard.KeyUpEvent;
            contentAdapter.Control.RaiseEvent(wpfKeyEventArgs);
            e.Handled = wpfKeyEventArgs.Handled;
        }
    }
}
