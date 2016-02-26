using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace System.Windows.Controls.Primitives
{
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.NormalState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.MouseOverState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.PressedState)]
    [TemplateVisualState(VisualStates.CommonStates, VisualStates.DisabledState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.FocusedState)]
    [TemplateVisualState(VisualStates.FocusStates, VisualStates.UnfocusedState)]
    public class Thumb : Control
    {
        public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(DragStartedEventHandler), typeof(Thumb));
        public event DragStartedEventHandler DragStarted
        {
            add { AddHandler(DragStartedEvent, value); }
            remove { RemoveHandler(DragStartedEvent, value); }
        }

        public static readonly RoutedEvent DragDeltaEvent = EventManager.RegisterRoutedEvent("DragDelta", RoutingStrategy.Bubble, typeof(DragDeltaEventHandler), typeof(Thumb));
        public event DragDeltaEventHandler DragDelta
        {
            add { AddHandler(DragDeltaEvent, value); }
            remove { RemoveHandler(DragDeltaEvent, value); }
        }

        public static readonly RoutedEvent DragCompletedEvent = EventManager.RegisterRoutedEvent("DragCompleted", RoutingStrategy.Bubble, typeof(DragCompletedEventHandler), typeof(Thumb));
        public event DragCompletedEventHandler DragCompleted
        {
            add { AddHandler(DragCompletedEvent, value); }
            remove { RemoveHandler(DragCompletedEvent, value); }
        }

        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(bool), typeof(Thumb), new ControlPropertyMetadata(affectsVisualState: true));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingPropertyKey); }
            private set { SetValue(IsDraggingPropertyKey, value); }
        }

        private Point dragStartPosition;

        static Thumb()
        {
            FocusableProperty.OverrideMetadata(typeof(Thumb), new FrameworkPropertyMetadata(false));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == Input.MouseButton.Left)
            {
                e.MouseDevice.Capture(this);
                dragStartPosition = e.GetPosition((IInputElement)VisualParent);

                IsDragging = true;

                DragStartedEventArgs dragStartedEventArgs = new DragStartedEventArgs(DragStartedEvent, this);
                RaiseEvent(dragStartedEventArgs);

                e.Handled = dragStartedEventArgs.Handled;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsDragging)
            {
                DragDeltaEventArgs DragDeltaEventArgs = new DragDeltaEventArgs(DragDeltaEvent, this, e.GetPosition((IInputElement)VisualParent) - dragStartPosition);
                RaiseEvent(DragDeltaEventArgs);

                e.Handled = DragDeltaEventArgs.Handled;
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsDragging)
            {
                e.MouseDevice.ReleaseCapture();

                IsDragging = false;

                DragCompletedEventArgs dragCompletedEventArgs = new DragCompletedEventArgs(DragCompletedEvent, this, false);
                RaiseEvent(dragCompletedEventArgs);

                e.Handled = dragCompletedEventArgs.Handled;
            }
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

            if (IsDragging)
            {
                return VisualStates.PressedState;
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
