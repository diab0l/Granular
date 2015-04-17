using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Controls
{
    public enum ExpandDirection
    {
        Down,
        Up,
        Left,
        Right
    }

    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    [TemplateVisualState(VisualStates.ExpansionStates, VisualStates.ExpandedState)]
    [TemplateVisualState(VisualStates.ExpansionStates, VisualStates.CollapsedState)]
    public class Expander : HeaderedContentControl
    {
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Expander));
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Expander));
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        public static readonly DependencyProperty ExpandDirectionProperty = DependencyProperty.Register("ExpandDirection", typeof(ExpandDirection), typeof(Expander), new FrameworkPropertyMetadata());
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), typeof(Expander), new ControlPropertyMetadata(affectsVisualState: true, propertyChangedCallback: (sender, e) => ((Expander)sender).OnIsExpandedChanged(e)));
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public Expander()
        {
            //
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                RaiseEvent(new RoutedEventArgs(ExpandedEvent, this));
            }
            else
            {
                RaiseEvent(new RoutedEventArgs(CollapsedEvent, this));
            }
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, GetCommonState(), useTransitions);
            VisualStateManager.GoToState(this, GetExpansionState(), useTransitions);
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

        private string GetExpansionState()
        {
            return IsExpanded ? VisualStates.ExpandedState : VisualStates.CollapsedState;
        }
    }
}
