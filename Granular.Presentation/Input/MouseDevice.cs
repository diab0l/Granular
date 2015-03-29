using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Input
{
    public class RawEventArgs
    {
        public int Timestamp { get; private set; }

        public RawEventArgs(int timestamp)
        {
            this.Timestamp = timestamp;
        }
    }

    public class RawMouseEventArgs : RawEventArgs
    {
        public Point Position { get; private set; }

        public RawMouseEventArgs(Point position, int timestamp) :
            base(timestamp)
        {
            this.Position = position;
        }
    }

    public class RawMouseButtonEventArgs : RawMouseEventArgs
    {
        public MouseButton Button { get; private set; }
        public MouseButtonState ButtonState { get; private set; }

        public RawMouseButtonEventArgs(MouseButton button, MouseButtonState buttonState, Point position, int timestamp) :
            base(position, timestamp)
        {
            this.Button = button;
            this.ButtonState = buttonState;
        }
    }

    public class RawMouseWheelEventArgs : RawMouseEventArgs
    {
        public int Delta { get; private set; }

        public RawMouseWheelEventArgs(int delta, Point position, int timestamp) :
            base(position, timestamp)
        {
            this.Delta = delta;
        }
    }

    public class MouseDevice : IInputDevice, IDisposable
    {
        private IInputElement hitTarget;
        public IInputElement HitTarget
        {
            get { return hitTarget; }
            private set
            {
                this.hitTarget = value;
                SetTarget();
            }
        }

        private IInputElement captureTarget;
        public IInputElement CaptureTarget
        {
            get { return captureTarget; }
            private set
            {
                this.captureTarget = value;
                SetTarget();
            }
        }

        private bool isActive;
        private bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                SetTarget();
            }
        }

        private IInputElement target;
        public IInputElement Target
        {
            get { return target; }
            private set
            {
                if (target == value)
                {
                    return;
                }

                IInputElement oldTarget = target;
                this.target = value;

                OnTargetChanged(oldTarget, target);
            }
        }

        public Point Position { get; private set; }

        private IPresentationSource presentationSource;
        private HashSet<MouseButton> pressedButtons;

        public MouseDevice(IPresentationSource presentationSource)
        {
            this.presentationSource = presentationSource;
            presentationSource.HitTestInvalidated += OnHitTestInvalidated;

            pressedButtons = new HashSet<MouseButton>();
            Position = Point.Zero;
        }

        public void Dispose()
        {
            presentationSource.HitTestInvalidated -= OnHitTestInvalidated;
        }

        private void SetTarget()
        {
            Target = IsActive ? CaptureTarget ?? HitTarget : null;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IInputElement lastTarget = Target;

            IsActive = false;

            if (lastTarget != null)
            {
                int timestamp = presentationSource.GetTimestamp();

                foreach (MouseButton button in pressedButtons)
                {
                    lastTarget.RaiseEvents(
                        new MouseButtonEventArgs(Mouse.PreviewMouseUpEvent, lastTarget, this, timestamp, Position, button, MouseButtonState.Released, 1),
                        new MouseButtonEventArgs(Mouse.MouseUpEvent, lastTarget, this, timestamp, Position, button, MouseButtonState.Released, 1));
                }
            }

            pressedButtons.Clear();
        }

        public bool ProcessRawEvent(RawMouseEventArgs rawEventArgs)
        {
            if (!IsActive)
            {
                Activate();
            }

            Position = rawEventArgs.Position;
            HitTarget = presentationSource.HitTest(Position);

            if (Target == null)
            {
                return false;
            }

            if (rawEventArgs is RawMouseButtonEventArgs)
            {
                return ProcessRawMouseButtonEvent((RawMouseButtonEventArgs)rawEventArgs);
            }

            if (rawEventArgs is RawMouseWheelEventArgs)
            {
                return ProcessRawMouseWheelEvent((RawMouseWheelEventArgs)rawEventArgs);
            }

            return Target.RaiseEvents(
                new MouseEventArgs(Mouse.PreviewMouseMoveEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position),
                new MouseEventArgs(Mouse.MouseMoveEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position));
        }

        private bool ProcessRawMouseButtonEvent(RawMouseButtonEventArgs rawEventArgs)
        {
            bool isPressed = rawEventArgs.ButtonState == MouseButtonState.Pressed;

            if (isPressed)
            {
                pressedButtons.Add(rawEventArgs.Button);
            }
            else
            {
                pressedButtons.Remove(rawEventArgs.Button);
            }

            return Target.RaiseEvents(
                new MouseButtonEventArgs(isPressed ? Mouse.PreviewMouseDownEvent : Mouse.PreviewMouseUpEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position, rawEventArgs.Button, rawEventArgs.ButtonState, 1),
                new MouseButtonEventArgs(isPressed ? Mouse.MouseDownEvent : Mouse.MouseUpEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position, rawEventArgs.Button, rawEventArgs.ButtonState, 1));
        }

        private bool ProcessRawMouseWheelEvent(RawMouseWheelEventArgs rawEventArgs)
        {
            return Target.RaiseEvents(
                new MouseWheelEventArgs(Mouse.PreviewMouseWheelEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position, rawEventArgs.Delta),
                new MouseWheelEventArgs(Mouse.MouseWheelEvent, Target, this, rawEventArgs.Timestamp, rawEventArgs.Position, rawEventArgs.Delta));
        }

        private void OnHitTestInvalidated(object sender, EventArgs e)
        {
            HitTarget = presentationSource.HitTest(Position);
        }

        public MouseButtonState GetButtonState(MouseButton button)
        {
            return pressedButtons.Contains(button) ? MouseButtonState.Pressed : MouseButtonState.Released;
        }

        public void Capture(IInputElement element)
        {
            CaptureTarget = element;
        }

        public void ReleaseCapture()
        {
            CaptureTarget = null;
        }

        private void OnTargetChanged(IInputElement oldTarget, IInputElement newTarget)
        {
            IInputElement[] oldTargetPath = oldTarget != null ? oldTarget.GetPathFromRoot().ToArray() : new IInputElement[0];
            IInputElement[] newTargetPath = newTarget != null ? newTarget.GetPathFromRoot().ToArray() : new IInputElement[0];

            int splitIndex = 0;
            while (splitIndex < oldTargetPath.Length && splitIndex < newTargetPath.Length && oldTargetPath[splitIndex] == newTargetPath[splitIndex])
            {
                splitIndex++;
            }

            int timestamp = presentationSource.GetTimestamp();

            for (int i = oldTargetPath.Length - 1; i >= splitIndex; i--)
            {
                oldTargetPath[i].RaiseEvent(new MouseEventArgs(Mouse.MouseLeaveEvent, oldTargetPath[i], this, timestamp, Position));
            }

            for (int i = splitIndex; i < newTargetPath.Length; i++)
            {
                newTargetPath[i].RaiseEvent(new MouseEventArgs(Mouse.MouseEnterEvent, newTargetPath[i], this, timestamp, Position));
            }
        }
    }
}
