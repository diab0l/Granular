using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.FocusedState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.UnfocusedState)]
    [TemplatePart("PART_ContentHost", typeof(Decorator))]
    public abstract class TextBoxBase : Control
    {
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(TextBoxBase));
        public event RoutedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Direct, typeof(TextChangedEventHandler), typeof(TextBoxBase));
        public event TextChangedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(TextBoxBase), new FrameworkPropertyMetadata());
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public static readonly DependencyProperty AcceptsTabProperty = DependencyProperty.Register("AcceptsTab", typeof(bool), typeof(TextBoxBase), new FrameworkPropertyMetadata());
        public bool AcceptsTab
        {
            get { return (bool)GetValue(AcceptsTabProperty); }
            set { SetValue(AcceptsTabProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextBoxBase), new FrameworkPropertyMetadata());
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public Decorator ContentHost { get; private set; }

        static TextBoxBase()
        {
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(TextBoxBase), new ControlPropertyMetadata(inherits: true, affectsVisualState: true));
            UIElement.IsMouseOverProperty.OverrideMetadata(typeof(TextBoxBase), new ControlPropertyMetadata(affectsVisualState: true));
            UIElement.IsFocusedProperty.OverrideMetadata(typeof(TextBoxBase), new ControlPropertyMetadata(affectsVisualState: true));
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template == null)
            {
                ContentHost = null;
            }
            else
            {
                ContentHost = Template.FindName("PART_ContentHost", this) as Decorator;
                ContentHost.Child = GetTextBoxContent();
            }
        }

        protected abstract FrameworkElement GetTextBoxContent();

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
