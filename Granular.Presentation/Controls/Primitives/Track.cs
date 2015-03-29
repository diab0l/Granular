using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Controls.Primitives
{
    public class Track : FrameworkElement
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Track), new FrameworkPropertyMetadata(affectsMeasure: true));
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(Track), new FrameworkPropertyMetadata(affectsMeasure: true));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(Track), new FrameworkPropertyMetadata(1.0, affectsMeasure: true));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Track), new FrameworkPropertyMetadata(affectsArrange: true, bindsTwoWayByDefault: true));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ViewportSizeProperty = DependencyProperty.Register("ViewportSize", typeof(double), typeof(Track), new FrameworkPropertyMetadata(Double.NaN, affectsMeasure: true));
        public double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        private RepeatButton decreaseRepeatButton;
        public RepeatButton DecreaseRepeatButton
        {
            get { return decreaseRepeatButton; }
            set
            {
                if (decreaseRepeatButton == value)
                {
                    return;
                }

                if (decreaseRepeatButton != null)
                {
                    RemoveVisualChild(decreaseRepeatButton);
                }

                decreaseRepeatButton = value;

                if (decreaseRepeatButton != null)
                {
                    AddVisualChild(decreaseRepeatButton);
                }
            }
        }

        private RepeatButton increaseRepeatButton;
        public RepeatButton IncreaseRepeatButton
        {
            get { return increaseRepeatButton; }
            set
            {
                if (increaseRepeatButton == value)
                {
                    return;
                }

                if (increaseRepeatButton != null)
                {
                    RemoveVisualChild(increaseRepeatButton);
                }

                increaseRepeatButton = value;

                if (increaseRepeatButton != null)
                {
                    AddVisualChild(increaseRepeatButton);
                }
            }
        }

        private Thumb thumb;
        public Thumb Thumb
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
                    RemoveVisualChild(thumb);
                }

                thumb = value;

                if (thumb != null)
                {
                    AddVisualChild(thumb);
                }
            }
        }

        public double ThumbMinLength { get; set; }

        public Track()
        {
            ThumbMinLength = 24;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Thumb != null)
            {
                Thumb.Measure(availableSize);
                return GetMainSize(0).Combine(Thumb.DesiredSize);
            }

            return Size.Zero;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double mainLength = GetMainLength(finalSize);
            double crossLength = GetCrossLength(finalSize);

            // the scrollable range (Maximum - Minimum) equals to (ExtentSize - ViewportSize)
            // the thumb ratio should be (ViewportSize / ExtentSize) = (ViewportSize / (ViewportSize + Maximum - Minimum))
            double thumbMainLength = (Double.IsNaN(ViewportSize) ? (Thumb == null ? 0 : GetMainLength(Thumb.DesiredSize)) : (mainLength * ViewportSize / (ViewportSize + Maximum - Minimum))).Bounds(ThumbMinLength, ThumbMinLength.Max(mainLength));

            // the decrease and increase buttons fill the remaining area (these buttons are usually transparent)
            double decreaseButtonMainLength = Maximum == Minimum ? 0 : (mainLength - thumbMainLength) * (Value.Min(Maximum) - Minimum) / (Maximum - Minimum);
            double increaseButtonMainLength = mainLength - thumbMainLength - decreaseButtonMainLength;

            if (DecreaseRepeatButton != null && IncreaseRepeatButton != null)
            {
                ArrangeChild(DecreaseRepeatButton, 0, 0, decreaseButtonMainLength, crossLength);
                ArrangeChild(IncreaseRepeatButton, decreaseButtonMainLength + thumbMainLength, 0, increaseButtonMainLength, crossLength);
            }

            if (Thumb != null)
            {
                ArrangeChild(Thumb, decreaseButtonMainLength, 0, thumbMainLength, crossLength);
            }

            return finalSize;
        }

        private double GetMainLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetCrossLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Height : size.Width;
        }

        private Size GetMainSize(double length)
        {
            return Orientation == Orientation.Horizontal ? Size.FromWidth(length) : Size.FromHeight(length);
        }

        private void ArrangeChild(UIElement child, double finalMainStart, double finalCrossStart, double finalMainLength, double finalCrossLength)
        {
            child.Arrange(Orientation == Orientation.Horizontal ?
                new Rect(finalMainStart, finalCrossStart, finalMainLength, finalCrossLength) :
                new Rect(finalCrossStart, finalMainStart, finalCrossLength, finalMainLength));
        }
    }
}
