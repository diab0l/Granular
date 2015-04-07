using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.Primitives
{
    [TemplateVisualState(VisualStates.CheckStates, VisualStates.CheckedState)]
    [TemplateVisualState(VisualStates.CheckStates, VisualStates.UncheckedState)]
    [TemplateVisualState(VisualStates.CheckStates, VisualStates.IndeterminateState)]
    public class ToggleButton : ButtonBase
    {
        public static readonly RoutedEvent CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
        public event RoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        public static readonly RoutedEvent UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
        public event RoutedEventHandler Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        public static readonly RoutedEvent IndeterminateEvent = EventManager.RegisterRoutedEvent("Indeterminate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToggleButton));
        public event RoutedEventHandler Indeterminate
        {
            add { AddHandler(IndeterminateEvent, value); }
            remove { RemoveHandler(IndeterminateEvent, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(ToggleButton), new FrameworkPropertyMetadata(false, propertyChangedCallback: (sender, e) => ((ToggleButton)sender).OnIsCheckedChanged(e)));
        public bool? IsChecked
        {
            get { return (bool?)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsThreeStateProperty = DependencyProperty.Register("IsThreeState", typeof(bool), typeof(ToggleButton), new FrameworkPropertyMetadata());
        public bool IsThreeState
        {
            get { return (bool)GetValue(IsThreeStateProperty); }
            set { SetValue(IsThreeStateProperty, value); }
        }

        public static readonly DependencyProperty CheckOnClickProperty = DependencyProperty.Register("CheckOnClick", typeof(bool), typeof(ToggleButton), new FrameworkPropertyMetadata(true));
        public bool CheckOnClick
        {
            get { return (bool)GetValue(CheckOnClickProperty); }
            set { SetValue(CheckOnClickProperty, value); }
        }

        protected override void OnClick(RoutedEventArgs e)
        {
            if (CheckOnClick)
            {
                ToggleState();
            }
        }

        protected virtual void ToggleState()
        {
            IsChecked = GetToggledState(IsChecked, IsThreeState);
        }

        protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsChecked.HasValue && IsChecked.Value)
            {
                RaiseEvent(new RoutedEventArgs(CheckedEvent, this));
            }

            if (IsChecked.HasValue && !IsChecked.Value)
            {
                RaiseEvent(new RoutedEventArgs(UncheckedEvent, this));
            }
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            base.UpdateVisualState(useTransitions);

            VisualStateManager.GoToState(this, GetCheckState(), useTransitions);
        }

        private string GetCheckState()
        {
            if (IsChecked.HasValue)
            {
                return IsChecked.Value ? VisualStates.CheckedState : VisualStates.UncheckedState;
            }

            return VisualStates.IndeterminateState;
        }

        public static bool? GetToggledState(bool? currentState, bool isThreeState)
        {
            // false -> true [-> null] -> false

            if (currentState == false)
            {
                return true;
            }

            if (currentState == null || !isThreeState)
            {
                return false;
            }

            return null;
        }
    }
}
