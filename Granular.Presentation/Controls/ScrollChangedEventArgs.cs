using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public delegate void ScrollChangedEventHandler(object sender, ScrollChangedEventArgs e);

    public class ScrollChangedEventArgs : RoutedEventArgs
    {
        public Point Offset { get; private set; }
        public Point OffsetChange { get; private set; }
        public Size Extent { get; private set; }
        public Point ExtentChange { get; private set; }
        public Size Viewport { get; private set; }
        public Point ViewportChange { get; private set; }

        public ScrollChangedEventArgs(RoutedEvent routedEvent, object originalSource, Point offset, Point offsetChange, Size extent, Point extentChange, Size viewport, Point viewportChange) :
            base(routedEvent, originalSource)
        {
            this.Offset = offset;
            this.OffsetChange = offsetChange;
            this.Extent = extent;
            this.ExtentChange = extentChange;
            this.Viewport = viewport;
            this.ViewportChange = viewportChange;
        }

        public override void InvokeEventHandler(Delegate handler, object target)
        {
            if (handler is ScrollChangedEventHandler)
            {
                ((ScrollChangedEventHandler)handler)(target, this);
            }
            else
            {
                base.InvokeEventHandler(handler, target);
            }
        }
    }
}
