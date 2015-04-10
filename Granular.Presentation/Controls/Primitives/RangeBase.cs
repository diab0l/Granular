using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.FocusedState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.UnfocusedState)]
    public abstract class RangeBase : Control
    {
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeBase));
        public event RoutedPropertyChangedEventHandler<double> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(coerceValueCallback: CoerceValueRange, bindsTwoWayByDefault: true, propertyChangedCallback: (sender, e) => ((RangeBase)sender).OnValueChanged(e)));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((RangeBase)sender).OnMinimumChanged(e)));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((RangeBase)sender).OnMaximumChanged(e)));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register("SmallChange", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata());
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register("LargeChange", typeof(double), typeof(RangeBase), new FrameworkPropertyMetadata());
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        private static object CoerceValueRange(DependencyObject dependencyObject, object value)
        {
            return ((double)value).Bounds(((RangeBase)dependencyObject).Minimum, ((RangeBase)dependencyObject).Maximum);
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, GetCommonState(), useTransitions);
            VisualStateManager.GoToState(this, GetFocusState(), useTransitions);
        }

        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<double>(ValueChangedEvent, this, (double)e.OldValue, (double)e.NewValue));
        }

        protected virtual void OnMinimumChanged(DependencyPropertyChangedEventArgs e)
        {
            //
        }

        protected virtual void OnMaximumChanged(DependencyPropertyChangedEventArgs e)
        {
            //
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
            return IsKeyboardFocused ? VisualStates.FocusedState : VisualStates.UnfocusedState;
        }
    }
}
