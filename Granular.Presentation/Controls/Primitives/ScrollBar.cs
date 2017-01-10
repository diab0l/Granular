using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    [TemplatePart("PART_Track", typeof(Track))]
    [TemplatePart("PART_DecreaseButton", typeof(ButtonBase))]
    [TemplatePart("PART_IncreaseButton", typeof(ButtonBase))]
    public class ScrollBar : RangeBase
    {
        private int SnapBackThreshold = 150;

        public static readonly RoutedEvent ScrollEvent = EventManager.RegisterRoutedEvent("Scroll", RoutingStrategy.Bubble, typeof(ScrollEventHandler), typeof(ScrollBar));
        public event ScrollEventHandler Scroll
        {
            add { AddHandler(ScrollEvent, value); }
            remove { RemoveHandler(ScrollEvent, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ScrollBar), new FrameworkPropertyMetadata());
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(ScrollBar), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((ScrollBar)sender).OnViewportSizeChanged(e)));
        public double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        private Thumb thumb;
        private Thumb Thumb
        {
            get { return thumb; }
            set
            {
                if (thumb == value)
                {
                    return;
                }

                if (thumb != null)
                {
                    thumb.DragStarted -= OnThumbDragStarted;
                    thumb.DragDelta -= OnThumbDragDelta;
                    thumb.DragCompleted -= OnThumbDragCompleted;
                }

                thumb = value;

                if (thumb != null)
                {
                    thumb.DragStarted += OnThumbDragStarted;
                    thumb.DragDelta += OnThumbDragDelta;
                    thumb.DragCompleted += OnThumbDragCompleted;
                }
            }
        }

        private Track track;
        private Track Track
        {
            get { return track; }
            set
            {
                track = value;

                if (track != null)
                {
                    Thumb = track.Thumb;
                    DecreasePageButton = track.DecreaseRepeatButton;
                    IncreasePageButton = track.IncreaseRepeatButton;
                }
                else
                {
                    Thumb = null;
                    DecreasePageButton = null;
                    IncreasePageButton = null;
                }
            }
        }

        private ButtonBase decreasePageButton;
        private ButtonBase DecreasePageButton
        {
            get { return decreasePageButton; }
            set
            {
                if (decreasePageButton == value)
                {
                    return;
                }

                if (decreasePageButton != null)
                {
                    decreasePageButton.Click -= OnDecreasePageClicked;
                }

                decreasePageButton = value;

                if (decreasePageButton != null)
                {
                    decreasePageButton.Click += OnDecreasePageClicked;
                }
            }
        }

        private ButtonBase increasePageButton;
        private ButtonBase IncreasePageButton
        {
            get { return increasePageButton; }
            set
            {
                if (increasePageButton == value)
                {
                    return;
                }

                if (increasePageButton != null)
                {
                    increasePageButton.Click -= OnIncreasePageClicked;
                }

                increasePageButton = value;

                if (increasePageButton != null)
                {
                    increasePageButton.Click += OnIncreasePageClicked;
                }
            }
        }

        private ButtonBase decreaseLineButton;
        private ButtonBase DecreaseLineButton
        {
            get { return decreaseLineButton; }
            set
            {
                if (decreaseLineButton == value)
                {
                    return;
                }

                if (decreaseLineButton != null)
                {
                    decreaseLineButton.Click -= OnDecreaseLineClicked;
                }

                decreaseLineButton = value;

                if (decreaseLineButton != null)
                {
                    decreaseLineButton.Click += OnDecreaseLineClicked;
                }
            }
        }

        private ButtonBase increaseLineButton;
        private ButtonBase IncreaseLineButton
        {
            get { return increaseLineButton; }
            set
            {
                if (increaseLineButton == value)
                {
                    return;
                }

                if (increaseLineButton != null)
                {
                    increaseLineButton.Click -= OnIncreaseLineClicked;
                }

                increaseLineButton = value;

                if (increaseLineButton != null)
                {
                    increaseLineButton.Click += OnIncreaseLineClicked;
                }
            }
        }

        private double dragInitialValue;

        static ScrollBar()
        {
            UIElement.FocusableProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(false));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ScrollBar), new FrameworkPropertyMetadata(new StyleKey(typeof(ScrollBar))));
        }

        public ScrollBar()
        {
            //
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                DecreaseLineButton = Template.FindName("PART_DecreaseButton", this) as ButtonBase;
                IncreaseLineButton = Template.FindName("PART_IncreaseButton", this) as ButtonBase;
                Track = Template.FindName("PART_Track", this) as Track;
            }
            else
            {
                DecreaseLineButton = null;
                IncreaseLineButton = null;
                Track = null;
            }
        }

        private void OnDecreaseLineClicked(object sender, RoutedEventArgs e)
        {
            Value -= SmallChange;
            RaiseScrollEvent(ScrollEventType.SmallDecrement);
            e.Handled = true;
        }

        private void OnIncreaseLineClicked(object sender, RoutedEventArgs e)
        {
            Value += SmallChange;
            RaiseScrollEvent(ScrollEventType.SmallIncrement);
            e.Handled = true;
        }

        private void OnDecreasePageClicked(object sender, RoutedEventArgs e)
        {
            Value -= LargeChange;
            RaiseScrollEvent(ScrollEventType.LargeDecrement);
            e.Handled = true;
        }

        private void OnIncreasePageClicked(object sender, RoutedEventArgs e)
        {
            Value += LargeChange;
            RaiseScrollEvent(ScrollEventType.LargeIncrement);
            e.Handled = true;
        }

        private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            dragInitialValue = Value;
            e.Handled = true;
        }

        private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Value = Math.Abs(GetCrossLength(e.Delta)) > SnapBackThreshold ? dragInitialValue :
                (dragInitialValue + (Maximum - Minimum) * GetMainLength(e.Delta) / (GetMainLength(Track.RenderSize) - GetMainLength(Thumb.RenderSize))).Bounds(Minimum, Maximum);

            RaiseScrollEvent(ScrollEventType.ThumbTrack);
            e.Handled = true;
        }

        private void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (e.IsCanceled)
            {
                Value = dragInitialValue;
            }

            RaiseScrollEvent(ScrollEventType.EndScroll);
            e.Handled = true;
        }

        private void OnViewportSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            SmallChange = ScrollInfoExtensions.ScrollLineDelta;
            LargeChange = (ViewportSize - ScrollInfoExtensions.ScrollLineDelta).Max(ScrollInfoExtensions.ScrollLineDelta);
        }

        private double GetMainLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetMainLength(Point point)
        {
            return Orientation == Orientation.Horizontal ? point.X : point.Y;
        }

        private double GetCrossLength(Point point)
        {
            return Orientation == Orientation.Horizontal ? point.Y : point.X;
        }

        private void RaiseScrollEvent(ScrollEventType scrollEventType)
        {
            RaiseEvent(new ScrollEventArgs(ScrollEvent, this, scrollEventType, Value));
        }
    }
}
