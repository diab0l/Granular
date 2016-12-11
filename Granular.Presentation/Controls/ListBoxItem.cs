using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls
{
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    [TemplateVisualState(VisualStates.SelectionStates, VisualStates.SelectedState)]
    [TemplateVisualState(VisualStates.SelectionStates, VisualStates.SelectedUnfocusedState)]
    [TemplateVisualState(VisualStates.SelectionStates, VisualStates.UnselectedState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.FocusedState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.UnfocusedState)]
    public class ListBoxItem : ContentControl
    {
        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(ListBoxItem));
        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        public static readonly RoutedEvent UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(ListBoxItem));
        public event RoutedEventHandler Unselected
        {
            add { AddHandler(UnselectedEvent, value); }
            remove { RemoveHandler(UnselectedEvent, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ListBoxItem), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((ListBoxItem)sender).OnIsSelectedChanged(e)));
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        static ListBoxItem()
        {
            UIElement.IsEnabledProperty.OverrideMetadata(typeof(ListBoxItem), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsVisualState));
            UIElement.IsMouseOverProperty.OverrideMetadata(typeof(ListBoxItem), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.AffectsVisualState));
            Selector.IsSelectionActiveProperty.OverrideMetadata(typeof(ListBoxItem), new FrameworkPropertyMetadata(FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsVisualState));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Focus();
        }

        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs((bool)e.NewValue == true ? SelectedEvent : UnselectedEvent, this));
        }

        protected override void UpdateVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, GetCommonState(), useTransitions);
            VisualStateManager.GoToState(this, GetSelectionState(), useTransitions);
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

        private string GetSelectionState()
        {
            if (!IsSelected)
            {
                return VisualStates.UnselectedState;
            }

            if (!Selector.GetIsSelectionActive(this))
            {
                return VisualStates.SelectedUnfocusedState;
            }

            return VisualStates.SelectedState;
        }

        private string GetFocusState()
        {
            return IsFocused ? VisualStates.FocusedState : VisualStates.UnfocusedState;
        }
    }
}
