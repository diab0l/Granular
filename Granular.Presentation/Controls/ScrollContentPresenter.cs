using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace System.Windows.Controls
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class ScrollContentPresenter : ContentPresenter, IScrollInfo, IAdornerLayerHost
    {
        private class InnerScrollInfo : IScrollInfo
        {
            public Size ExtentSize { get; set; }
            public Size ViewportSize { get; set; }

            public bool CanHorizontallyScroll { get; set; }
            public bool CanVerticallyScroll { get; set; }
            public Point Offset { get; set; }

            public InnerScrollInfo()
            {
                ExtentSize = Size.Zero;
                ViewportSize = Size.Zero;
                Offset = Point.Zero;
            }
        }

        public AdornerLayer AdornerLayer { get; private set; }

        public static readonly DependencyProperty CanContentScrollProperty = ScrollViewer.CanContentScrollProperty.AddOwner(typeof(ScrollContentPresenter), new FrameworkPropertyMetadata(affectsArrange: true, propertyChangedCallback: OnCanContentScrollChanged));
        public bool CanContentScroll
        {
            get { return (bool)GetValue(CanContentScrollProperty); }
            set { SetValue(CanContentScrollProperty, value); }
        }

        public Size ExtentSize { get { return delegateScrollInfo.ExtentSize; } }

        public Size ViewportSize { get { return delegateScrollInfo.ViewportSize; } }

        public bool CanHorizontallyScroll
        {
            get { return delegateScrollInfo.CanHorizontallyScroll; }
            set
            {
                if (delegateScrollInfo.CanHorizontallyScroll == value)
                {
                    return;
                }

                delegateScrollInfo.CanHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }

        public bool CanVerticallyScroll
        {
            get { return delegateScrollInfo.CanVerticallyScroll; }
            set
            {
                if (delegateScrollInfo.CanVerticallyScroll == value)
                {
                    return;
                }

                delegateScrollInfo.CanVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        public Point Offset
        {
            get { return delegateScrollInfo.Offset; }
            set
            {
                if (delegateScrollInfo.Offset == value)
                {
                    return;
                }

                delegateScrollInfo.Offset = value;
                InvalidateArrange();
            }
        }

        private InnerScrollInfo innerScrollInfo;
        private IScrollInfo delegateScrollInfo; // one of innerScrollInfo or Content's implementation (when CanContentScroll)

        public ScrollContentPresenter()
	    {
            innerScrollInfo = new InnerScrollInfo();
            delegateScrollInfo = innerScrollInfo;

            AdornerLayer = new AdornerLayer();
            AddVisualChild(AdornerLayer);
	    }

        protected override void OnTemplateChildChanged()
        {
            // move AdornerLayer to the top
            SetVisualChildIndex(AdornerLayer, VisualChildren.Count() - 1);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            AdornerLayer.Measure(availableSize);

            Size availableScrollSize = new Size(CanHorizontallyScroll ? Double.PositiveInfinity : availableSize.Width, CanVerticallyScroll ? Double.PositiveInfinity : availableSize.Height);
            Size measuredSize = base.MeasureOverride(availableScrollSize);

            innerScrollInfo.ExtentSize = measuredSize;
            innerScrollInfo.ViewportSize = availableSize;

            return measuredSize.Min(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            AdornerLayer.Arrange(new Rect(finalSize));

            if (TemplateChild != null)
            {
                if (delegateScrollInfo == innerScrollInfo)
                {
                    Size finalScrollSize = new Size(CanHorizontallyScroll ? Double.PositiveInfinity : finalSize.Width, CanVerticallyScroll ? Double.PositiveInfinity : finalSize.Height);

                    Size childFinalSize = TemplateChild.DesiredSize.Bounds(finalSize, finalScrollSize);
                    Point childOffset = Offset.Bounds(Point.Zero, (childFinalSize - ViewportSize).Max(Size.Zero).ToPoint());

                    TemplateChild.Arrange(new Rect(-childOffset, childFinalSize));
                }
                else
                {
                    // CanContentScroll and Content implements IScrollInfo
                    TemplateChild.Arrange(new Rect(finalSize));
                }
            }

            innerScrollInfo.ExtentSize = TemplateChild != null ? TemplateChild.VisualSize : Size.Zero;
            innerScrollInfo.ViewportSize = finalSize;

            return finalSize;
        }

        protected override void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnContentChanged(e);
            delegateScrollInfo = GetDelegateScrollInfo();
        }

        private IScrollInfo GetDelegateScrollInfo()
        {
            if (CanContentScroll)
            {
                if (Content is IScrollInfo)
                {
                    return (IScrollInfo)Content;
                }

                if (Content is ItemsPresenter && ((ItemsPresenter)Content).Panel is IScrollInfo)
                {
                    return (IScrollInfo)((ItemsPresenter)Content).Panel;
                }
            }

            return innerScrollInfo;
        }

        private void OnCanContentScrollChanged(DependencyPropertyChangedEventArgs e)
        {
 	        delegateScrollInfo = GetDelegateScrollInfo();
        }

        private static void OnCanContentScrollChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is ScrollContentPresenter)
            {
                ((ScrollContentPresenter)dependencyObject).OnCanContentScrollChanged(e);
            }
        }
    }
}
