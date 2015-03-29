using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    public interface IScrollInfo
    {
        // the scrolled content size (usually larger than ViewportSize)
        Size ExtentSize { get; }

        // the available size (usually smaller than ExtentSize)
        Size ViewportSize { get; }

        bool CanHorizontallyScroll { get; set; }
        bool CanVerticallyScroll { get; set; }

        // scroll offset between 0 and Max(0, ExtentSize - ViewportSize)
        Point Offset { get; set; }
    }

    public static class ScrollInfoExtensions
    {
        public const double ScrollLineDelta = 16;
        public const double MouseWheelDelta = 48;

        public static void LineUp(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(0, ScrollLineDelta);
        }

        public static void LineDown(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(0, ScrollLineDelta);
        }

        public static void LineLeft(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(ScrollLineDelta, 0);
        }

        public static void LineRight(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(ScrollLineDelta, 0);
        }

        public static void PageUp(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(0, scrollInfo.ViewportSize.Height);
        }

        public static void PageDown(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(0, scrollInfo.ViewportSize.Height);
        }

        public static void PageLeft(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(scrollInfo.ViewportSize.Width, 0);
        }

        public static void PageRight(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(scrollInfo.ViewportSize.Width, 0);
        }

        public static void MouseWheelUp(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(0, MouseWheelDelta);
        }

        public static void MouseWheelDown(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(0, MouseWheelDelta);
        }

        public static void MouseWheelLeft(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset -= new Point(MouseWheelDelta, 0);
        }

        public static void MouseWheelRight(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset += new Point(MouseWheelDelta, 0);
        }

        public static void ScrollToHome(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset = Point.Zero;
        }

        public static void ScrollToEnd(this IScrollInfo scrollInfo)
        {
            scrollInfo.Offset = new Point(0, scrollInfo.GetScrollableSize().Height);
        }

        public static void ScrollToHorizontalOffset(this IScrollInfo scrollInfo, double offset)
        {
            scrollInfo.Offset = new Point(offset.Bounds(0, scrollInfo.GetScrollableSize().Width), scrollInfo.Offset.Y);
        }

        public static void ScrollToVerticalOffset(this IScrollInfo scrollInfo, double offset)
        {
            scrollInfo.Offset = new Point(scrollInfo.Offset.X, offset.Bounds(0, scrollInfo.GetScrollableSize().Height));
        }

        public static Size GetScrollableSize(this IScrollInfo scrollInfo)
        {
            return (scrollInfo.ExtentSize - scrollInfo.ViewportSize).Max(Size.Zero);
        }
    }
}
