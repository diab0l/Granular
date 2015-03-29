using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.Primitives
{
    public delegate void DragStartedEventHandler(object sender, DragStartedEventArgs e);
    public delegate void DragDeltaEventHandler(object sender, DragDeltaEventArgs e);
    public delegate void DragCompletedEventHandler(object sender, DragCompletedEventArgs e);

    public class DragStartedEventArgs : RoutedEventArgs
    {
        public DragStartedEventArgs(RoutedEvent routedEvent, object originalSource) :
            base(routedEvent, originalSource)
        {
            //
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is DragStartedEventHandler)
            {
                ((DragStartedEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }

    public class DragDeltaEventArgs : RoutedEventArgs
    {
        public Point Delta { get; private set; }

        public DragDeltaEventArgs(RoutedEvent routedEvent, object originalSource, Point delta) :
            base(routedEvent, originalSource)
        {
            this.Delta = delta;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is DragDeltaEventHandler)
            {
                ((DragDeltaEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }

    public class DragCompletedEventArgs : RoutedEventArgs
    {
        public bool IsCanceled { get; private set; }

        public DragCompletedEventArgs(RoutedEvent routedEvent, object originalSource, bool isCanceled) :
            base(routedEvent, originalSource)
        {
            this.IsCanceled = isCanceled;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is DragCompletedEventHandler)
            {
                ((DragCompletedEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
