using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Granular.Extensions;

namespace System.Windows.Controls
{
    public enum ScrollBarVisibility
    {
        Disabled,
        Auto,
        Hidden,
        Visible
    }

    [TemplatePart("PART_HorizontalScrollBar", typeof(ScrollBar))]
    [TemplatePart("PART_VerticalScrollBar", typeof(ScrollBar))]
    [TemplatePart("PART_ScrollContentPresenter", typeof(ScrollContentPresenter))]
    public class ScrollViewer : ContentControl
    {
        public static readonly RoutedEvent ScrollChangedEvent = EventManager.RegisterRoutedEvent("ScrollChanged", RoutingStrategy.Bubble, typeof(ScrollChangedEventHandler), typeof(ScrollViewer));
        public event ScrollChangedEventHandler ScrollChanged
        {
            add { AddHandler(ScrollChangedEvent, value); }
            remove { RemoveHandler(ScrollChangedEvent, value); }
        }

        public static readonly DependencyProperty CanContentScrollProperty = DependencyProperty.RegisterAttached("CanContentScroll", typeof(bool), typeof(ScrollViewer), new FrameworkPropertyMetadata());

        public static bool GetCanContentScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanContentScrollProperty);
        }

        public static void SetCanContentScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(CanContentScrollProperty, value);
        }

        private static readonly DependencyPropertyKey ComputedHorizontalScrollBarVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ComputedHorizontalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ComputedHorizontalScrollBarVisibilityProperty = ComputedHorizontalScrollBarVisibilityPropertyKey.DependencyProperty;
        public Visibility ComputedHorizontalScrollBarVisibility
        {
            get { return (Visibility)GetValue(ComputedHorizontalScrollBarVisibilityPropertyKey); }
            private set { SetValue(ComputedHorizontalScrollBarVisibilityPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ComputedVerticalScrollBarVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ComputedVerticalScrollBarVisibility", typeof(Visibility), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ComputedVerticalScrollBarVisibilityProperty = ComputedVerticalScrollBarVisibilityPropertyKey.DependencyProperty;
        public Visibility ComputedVerticalScrollBarVisibility
        {
            get { return (Visibility)GetValue(ComputedVerticalScrollBarVisibilityPropertyKey); }
            private set { SetValue(ComputedVerticalScrollBarVisibilityPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ComputedScrollBarsVisibilityPropertyKey = DependencyProperty.RegisterReadOnly("ComputedScrollBarsVisibility", typeof(Visibility), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ComputedScrollBarsVisibilityProperty = ComputedScrollBarsVisibilityPropertyKey.DependencyProperty;
        public Visibility ComputedScrollBarsVisibility
        {
            get { return (Visibility)GetValue(ComputedScrollBarsVisibilityPropertyKey); }
            private set { SetValue(ComputedScrollBarsVisibilityPropertyKey, value); }
        }

        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible, affectsMeasure: true, propertyChangedCallback: (sender, e) => ((ScrollViewer)sender).SetContentCanScroll()));
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ScrollViewer), new FrameworkPropertyMetadata(ScrollBarVisibility.Visible, affectsMeasure: true, propertyChangedCallback: (sender, e) => ((ScrollViewer)sender).SetContentCanScroll()));
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata(affectsArrange: true, propertyChangedCallback: (sender, e) => ((ScrollViewer)sender).SetOffsets()));
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata(affectsArrange: true, propertyChangedCallback: (sender, e) => ((ScrollViewer)sender).SetOffsets()));
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        private static readonly DependencyPropertyKey ExtentWidthPropertyKey = DependencyProperty.RegisterReadOnly("ExtentWidth", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ExtentWidthProperty = ExtentWidthPropertyKey.DependencyProperty;
        public double ExtentWidth
        {
            get { return (double)GetValue(ExtentWidthPropertyKey); }
            private set { SetValue(ExtentWidthPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ExtentHeightPropertyKey = DependencyProperty.RegisterReadOnly("ExtentHeight", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ExtentHeightProperty = ExtentHeightPropertyKey.DependencyProperty;
        public double ExtentHeight
        {
            get { return (double)GetValue(ExtentHeightPropertyKey); }
            private set { SetValue(ExtentHeightPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ViewportWidthPropertyKey = DependencyProperty.RegisterReadOnly("ViewportWidth", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ViewportWidthProperty = ViewportWidthPropertyKey.DependencyProperty;
        public double ViewportWidth
        {
            get { return (double)GetValue(ViewportWidthPropertyKey); }
            private set { SetValue(ViewportWidthPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ViewportHeightPropertyKey = DependencyProperty.RegisterReadOnly("ViewportHeight", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ViewportHeightProperty = ViewportHeightPropertyKey.DependencyProperty;
        public double ViewportHeight
        {
            get { return (double)GetValue(ViewportHeightPropertyKey); }
            private set { SetValue(ViewportHeightPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ScrollableWidthPropertyKey = DependencyProperty.RegisterReadOnly("ScrollableWidth", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ScrollableWidthProperty = ScrollableWidthPropertyKey.DependencyProperty;
        public double ScrollableWidth
        {
            get { return (double)GetValue(ScrollableWidthPropertyKey); }
            private set { SetValue(ScrollableWidthPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ScrollableHeightPropertyKey = DependencyProperty.RegisterReadOnly("ScrollableHeight", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ScrollableHeightProperty = ScrollableHeightPropertyKey.DependencyProperty;
        public double ScrollableHeight
        {
            get { return (double)GetValue(ScrollableHeightPropertyKey); }
            private set { SetValue(ScrollableHeightPropertyKey, value); }
        }

        private ScrollBar horizontalScrollBar;
        private ScrollBar HorizontalScrollBar
        {
            get { return horizontalScrollBar; }
            set
            {
                if (horizontalScrollBar == value)
                {
                    return;
                }

                if (horizontalScrollBar != null)
                {
                    horizontalScrollBar.ValueChanged -= OnScrollBarValueChanged;
                }

                horizontalScrollBar = value;

                if (horizontalScrollBar != null)
                {
                    horizontalScrollBar.ValueChanged += OnScrollBarValueChanged;
                }
            }
        }

        private ScrollBar verticalScrollBar;
        private ScrollBar VerticalScrollBar
        {
            get { return verticalScrollBar; }
            set
            {
                if (verticalScrollBar == value)
                {
                    return;
                }

                if (verticalScrollBar != null)
                {
                    verticalScrollBar.ValueChanged -= OnScrollBarValueChanged;
                }

                verticalScrollBar = value;

                if (verticalScrollBar != null)
                {
                    verticalScrollBar.ValueChanged += OnScrollBarValueChanged;
                }
            }
        }

        private ScrollContentPresenter scrollContentPresenter;

        static ScrollViewer()
        {
            Control.IsTabStopProperty.OverrideMetadata(typeof(ScrollViewer), new FrameworkPropertyMetadata(false));
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                scrollContentPresenter = Template.FindName("PART_ScrollContentPresenter", this) as ScrollContentPresenter;
                HorizontalScrollBar = Template.FindName("PART_HorizontalScrollBar", this) as ScrollBar;
                VerticalScrollBar = Template.FindName("PART_VerticalScrollBar", this) as ScrollBar;
            }
            else
            {
                scrollContentPresenter = null;
                HorizontalScrollBar = null;
                VerticalScrollBar = null;
            }

            SetContentCanScroll();
            SetScrollInfoSizes();
        }

        private void OnScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            HorizontalOffset = HorizontalScrollBar.Value;
            VerticalOffset = VerticalScrollBar.Value;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (scrollContentPresenter != null)
            {
                VerticalOffset = (VerticalOffset - Math.Sign(e.Delta) * ScrollInfoExtensions.MouseWheelDelta).Bounds(0, ScrollableHeight);
                e.Handled = true;
            }
        }

        private void SetOffsets()
        {
            if (scrollContentPresenter != null)
            {
                scrollContentPresenter.Offset = new Point(HorizontalOffset, VerticalOffset);
            }

            if (HorizontalScrollBar != null)
            {
                HorizontalScrollBar.Value = HorizontalOffset;
            }

            if (VerticalScrollBar != null)
            {
                VerticalScrollBar.Value = VerticalOffset;
            }
        }

        private void SetContentCanScroll()
        {
            if (scrollContentPresenter != null)
            {
                scrollContentPresenter.CanHorizontallyScroll = HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                scrollContentPresenter.CanVerticallyScroll = VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
            }
        }

        private void SetScrollInfoSizes()
        {
            if (scrollContentPresenter != null)
            {
                ExtentWidth = scrollContentPresenter.ExtentSize.Width;
                ExtentHeight = scrollContentPresenter.ExtentSize.Height;
                ViewportWidth = scrollContentPresenter.ViewportSize.Width;
                ViewportHeight = scrollContentPresenter.ViewportSize.Height;
            }
            else
            {
                ExtentWidth = 0;
                ExtentHeight = 0;
                ViewportWidth = 0;
                ViewportHeight = 0;
            }

            ScrollableWidth = (ExtentWidth - ViewportWidth).Max(0);
            ScrollableHeight = (ExtentHeight - ViewportHeight).Max(0);

            HorizontalOffset = HorizontalOffset.Min(ScrollableWidth);
            VerticalOffset = VerticalOffset.Min(ScrollableHeight);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (TemplateChild == null)
            {
                return Size.Zero;
            }

            ComputedHorizontalScrollBarVisibility = GetScrollBarVisibility(HorizontalScrollBarVisibility, false);
            ComputedVerticalScrollBarVisibility = GetScrollBarVisibility(VerticalScrollBarVisibility, false);
            ComputedScrollBarsVisibility = ComputedHorizontalScrollBarVisibility == Visibility.Visible && ComputedVerticalScrollBarVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;

            // 3 passes, each pass can cause an overflow (and add a scrollbar which invalidates the measure), starting with no overlaps
            for (int measurePass = 0; measurePass < 3; measurePass++)
            {
                // computed visibilities can invalidate the ScrollBars measure, invalidate their path so they will be re-measured through TemplateChild
                InvalidateElementMeasurePath(this, HorizontalScrollBar);
                InvalidateElementMeasurePath(this, VerticalScrollBar);

                TemplateChild.Measure(availableSize);

                Visibility measuredHorizontalScrollBarVisibility = GetScrollBarVisibility(HorizontalScrollBarVisibility, scrollContentPresenter != null && scrollContentPresenter.ViewportSize.Width < scrollContentPresenter.ExtentSize.Width);
                Visibility measuredVerticalScrollBarVisibility = GetScrollBarVisibility(VerticalScrollBarVisibility, scrollContentPresenter != null && scrollContentPresenter.ViewportSize.Height < scrollContentPresenter.ExtentSize.Height);

                if (ComputedHorizontalScrollBarVisibility == measuredHorizontalScrollBarVisibility &&
                    ComputedVerticalScrollBarVisibility == measuredVerticalScrollBarVisibility)
                {
                    break;
                }

                ComputedHorizontalScrollBarVisibility = measuredHorizontalScrollBarVisibility;
                ComputedVerticalScrollBarVisibility = measuredVerticalScrollBarVisibility;
                ComputedScrollBarsVisibility = ComputedHorizontalScrollBarVisibility == Visibility.Visible && ComputedVerticalScrollBarVisibility == Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            }

            SetScrollInfoSizes();

            return TemplateChild.DesiredSize;
        }

        private static void InvalidateElementMeasurePath(UIElement root, UIElement element)
        {
            if (element == null)
            {
                return;
            }

            while (element != root)
            {
                element.InvalidateMeasure();
                element = (UIElement)element.VisualParent;
            }
        }

        private static Visibility GetScrollBarVisibility(ScrollBarVisibility scrollBarVisibility, bool isOverflowed)
        {
            switch (scrollBarVisibility)
            {
                case ScrollBarVisibility.Disabled: return Visibility.Collapsed;
                case ScrollBarVisibility.Auto: return isOverflowed ? Visibility.Visible : Visibility.Collapsed;
                case ScrollBarVisibility.Hidden: return Visibility.Collapsed;
                case ScrollBarVisibility.Visible: return Visibility.Visible;
            }

            throw new Granular.Exception("Unexpected ScrollBarVisibility \"{0}\"", scrollBarVisibility);
        }
    }
}
