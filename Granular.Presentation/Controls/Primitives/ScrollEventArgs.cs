using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.Primitives
{
    public enum ScrollEventType
    {
        EndScroll,
        //First,
        LargeDecrement,
        LargeIncrement,
        //Last,
        SmallDecrement,
        SmallIncrement,
        //ThumbPosition,
        ThumbTrack
    }

    public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

    public class ScrollEventArgs : RoutedEventArgs
    {
        public double NewValue { get; private set; }
        public ScrollEventType ScrollEventType { get; private set; }

        public ScrollEventArgs(RoutedEvent routedEvent, object originalSource, ScrollEventType scrollEventType, double newValue) :
            base(routedEvent, originalSource)
        {
            this.ScrollEventType = scrollEventType;
            this.NewValue = newValue;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is ScrollEventHandler)
            {
                ((ScrollEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
