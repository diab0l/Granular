using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Controls
{
    [ContentProperty("Text")]
    public class TextBox : TextBoxBase
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).OnTextChanged(e)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty CaretIndexProperty = DependencyProperty.Register("CaretIndex", typeof(int), typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.CaretIndex = (int)e.NewValue));
        public int CaretIndex
        {
            get { return (int)GetValue(CaretIndexProperty); }
            set { SetValue(CaretIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.SelectionStart = (int)e.NewValue));
        public int SelectionStart
        {
            get { return (int)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register("SelectionLength", typeof(int), typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.SelectionLength = (int)e.NewValue));
        public int SelectionLength
        {
            get { return (int)GetValue(SelectionLengthProperty); }
            set { SetValue(SelectionLengthProperty, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.MaxLength = (int)e.NewValue));
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public static readonly DependencyProperty TextAlignmentProperty = TextBlock.TextAlignmentProperty.AddOwner(typeof(TextBox), new FrameworkPropertyMetadata(inherits: true));
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
        }

        public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(typeof(TextBox), new FrameworkPropertyMetadata(inherits: true));
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(TextBoxBase), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden, propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.HorizontalScrollBarVisibility = (ScrollBarVisibility)e.NewValue));
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(TextBoxBase), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden, propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.VerticalScrollBarVisibility = (ScrollBarVisibility)e.NewValue));
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        public int LineCount { get; private set; }

        private TextBoxView textBoxView;

        static TextBox()
        {
            TextBoxBase.AcceptsReturnProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.AcceptsReturn = (bool)e.NewValue));
            TextBoxBase.AcceptsTabProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.AcceptsTab = (bool)e.NewValue));
            TextBoxBase.IsReadOnlyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.IsReadOnly = ((TextBox)sender).IsReadOnly));
            SpellCheck.IsEnabledProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((TextBox)sender).textBoxView.SpellCheck = (bool)e.NewValue));
            FrameworkElement.CursorProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(Cursors.IBeam));
        }

        public TextBox()
        {
            textBoxView = new TextBoxView();
            textBoxView.HorizontalScrollBarVisibility = HorizontalScrollBarVisibility;
            textBoxView.VerticalScrollBarVisibility = VerticalScrollBarVisibility;
            textBoxView.SpellCheck = (bool)GetValue(SpellCheck.IsEnabledProperty);
            textBoxView.TextChanged += (sender, e) => this.Text = textBoxView.Text;
            textBoxView.CaretIndexChanged += (sender, e) => this.CaretIndex = textBoxView.CaretIndex;
            textBoxView.SelectionStartChanged += (sender, e) => this.SelectionStart = textBoxView.SelectionStart;
            textBoxView.SelectionLengthChanged += (sender, e) => this.SelectionLength = textBoxView.SelectionLength;
        }

        protected override FrameworkElement GetTextBoxContent()
        {
            return textBoxView;
        }

        public void Clear()
        {
            Text = String.Empty;
        }

        public void Select(int start, int length)
        {
            textBoxView.SelectionStart = start;
            textBoxView.SelectionLength = length;
        }

        public int GetCharacterIndexFromLineIndex(int lineIndex)
        {
            return Text.DefaultIfNullOrEmpty().GetCharacterIndexFromLineIndex(lineIndex);
        }

        public int GetLineIndexFromCharacterIndex(int charIndex)
        {
            return Text.DefaultIfNullOrEmpty().GetLineIndexFromCharacterIndex(charIndex);
        }

        public int GetLineLength(int lineIndex)
        {
            return Text.DefaultIfNullOrEmpty().GetLineLength(lineIndex);
        }

        public string GetLineText(int lineIndex)
        {
            return Text.DefaultIfNullOrEmpty().GetLineText(lineIndex);
        }

        public void ScrollToLine(int lineIndex)
        {
            int characterIndex = GetCharacterIndexFromLineIndex(lineIndex);

            if (characterIndex != -1)
            {
                Select(characterIndex, characterIndex);
            }
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            textBoxView.Text = Text;
            RaiseEvent(new TextChangedEventArgs(TextChangedEvent, this));
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            textBoxView.FocusRenderElement();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            textBoxView.ClearFocusRenderElement();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !AcceptsReturn || e.Key == Key.Tab && !AcceptsTab)
            {
                return;
            }

            textBoxView.ProcessRenderElementKeyEvent(e);
        }
    }
}
