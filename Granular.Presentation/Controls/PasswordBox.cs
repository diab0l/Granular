using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Input;

namespace System.Windows.Controls
{
    [TemplatePart("PART_ContentHost", typeof(FrameworkElement))]
    public sealed class PasswordBox : Control
    {
        public static readonly RoutedEvent PasswordChangedEvent = EventManager.RegisterRoutedEvent("PasswordChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PasswordBox));
        public event RoutedEventHandler PasswordChanged
        {
            add { AddHandler(PasswordChangedEvent, value); }
            remove { RemoveHandler(PasswordChangedEvent, value); }
        }

        public static readonly DependencyProperty MaxLengthProperty = TextBox.MaxLengthProperty.AddOwner(typeof(PasswordBox), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((PasswordBox)sender).textBoxView.MaxLength = (int)e.NewValue));
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                if (password == value)
                {
                    return;
                }

                password = value;
                textBoxView.Text = password;
                RaiseEvent(new RoutedEventArgs(PasswordChangedEvent, this));
            }
        }

        private TextBoxView textBoxView;
        private IDisposable passwordBoxViewKeyboardFocus;
        private Decorator contentHost;

        static PasswordBox()
        {
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(PasswordBox), new ControlPropertyMetadata(inherits: true, affectsVisualState: true, propertyChangedCallback: (sender, e) => ((PasswordBox)sender).textBoxView.IsReadOnly = !((PasswordBox)sender).IsEnabled));
            UIElement.IsMouseOverProperty.OverrideMetadata(typeof(PasswordBox), new ControlPropertyMetadata(affectsVisualState: true));
            UIElement.IsFocusedProperty.OverrideMetadata(typeof(PasswordBox), new ControlPropertyMetadata(affectsVisualState: true));
        }

        public PasswordBox()
        {
            textBoxView = new TextBoxView { IsPassword = true };
            textBoxView.TextChanged += (sender, e) => this.Password = textBoxView.Text;
            textBoxView.GotKeyboardFocus += (sender, e) => Focus();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null)
            {
                contentHost = null;
            }
            else
            {
                contentHost = Template.FindName("PART_ContentHost", this) as Decorator;
                contentHost.Child = textBoxView;
            }
        }

        public void SelectAll()
        {
            textBoxView.SelectionStart = 0;
            textBoxView.SelectionLength = Password.Length;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            passwordBoxViewKeyboardFocus = Keyboard.Focus(textBoxView);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (passwordBoxViewKeyboardFocus != null)
            {
                passwordBoxViewKeyboardFocus.Dispose();
                passwordBoxViewKeyboardFocus = null;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Focus();
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, GetCommonState(), useTransitions);
            VisualStateManager.GoToState(this, GetFocusState(), useTransitions);
        }

        private string GetCommonState()
        {
            if (!IsEnabled)
            {
                return VisualStates.DisabledState;
            }

            if (IsMouseOver)
            {
                return VisualStates.MouseOverState;
            }

            return VisualStates.NormalState;
        }

        private string GetFocusState()
        {
            return IsFocused ? VisualStates.FocusedState : VisualStates.UnfocusedState;
        }
    }
}
