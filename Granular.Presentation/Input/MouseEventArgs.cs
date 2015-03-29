using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Input
{
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);
    public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);

    public enum MouseButtonState
    {
        Released,
        Pressed
    }

    public enum MouseButton
    {
        Left,
        Middle,
        Right,
        XButton1,
        XButton2
    }

    public class MouseEventArgs : InputEventArgs
    {
        public MouseDevice MouseDevice { get; private set; }
        public MouseButtonState LeftButton { get; private set; }
        public MouseButtonState MiddleButton { get; private set; }
        public MouseButtonState RightButton { get; private set; }
        public MouseButtonState XButton1 { get; private set; }
        public MouseButtonState XButton2 { get; private set; }

        public Point AbsolutePosition { get; private set; }

        public MouseEventArgs(RoutedEvent routedEvent, object originalSource, MouseDevice mouseDevice, int timestamp, Point absolutePosition) :
            base(routedEvent, originalSource, mouseDevice, timestamp)
        {
            this.MouseDevice = mouseDevice;
            this.AbsolutePosition = absolutePosition;

            this.LeftButton = mouseDevice.GetButtonState(MouseButton.Left);
            this.MiddleButton = mouseDevice.GetButtonState(MouseButton.Middle);
            this.RightButton = mouseDevice.GetButtonState(MouseButton.Right);
            this.XButton1 = mouseDevice.GetButtonState(MouseButton.XButton1);
            this.XButton2 = mouseDevice.GetButtonState(MouseButton.XButton2);
        }

        public Point GetPosition(IInputElement relativeTo)
        {
            return relativeTo.GetRelativePosition(AbsolutePosition);
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is MouseEventHandler)
            {
                ((MouseEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }

    public class MouseButtonEventArgs : MouseEventArgs
    {
        public MouseButton ChangedButton { get; private set; }
        public MouseButtonState ButtonState { get; private set; }
        public int ClickCount { get; private set; }

        public MouseButtonEventArgs(RoutedEvent routedEvent, object originalSource, MouseDevice mouseDevice, int timestamp, Point absolutePosition, MouseButton changedButton, MouseButtonState buttonState, int clickCount) :
            base(routedEvent, originalSource, mouseDevice, timestamp, absolutePosition)
        {
            this.ChangedButton = changedButton;
            this.ButtonState = buttonState;
            this.ClickCount = clickCount;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is MouseButtonEventHandler)
            {
                ((MouseButtonEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }

    public class MouseWheelEventArgs : MouseEventArgs
    {
        public int Delta { get; private set; }

        public MouseWheelEventArgs(RoutedEvent routedEvent, object originalSource, MouseDevice mouseDevice, int timestamp, Point absolutePosition, int delta) :
            base(routedEvent, originalSource, mouseDevice, timestamp, absolutePosition)
        {
            this.Delta = delta;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is MouseWheelEventHandler)
            {
                ((MouseWheelEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
