using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Controls
{
    public class Canvas : Panel
    {
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(Double.NaN, affectsArrange: true));

        public static double GetLeft(DependencyObject obj)
        {
            return (double)obj.GetValue(LeftProperty);
        }

        public static void SetLeft(DependencyObject obj, double value)
        {
            obj.SetValue(LeftProperty, value);
        }

        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(Double.NaN, affectsArrange: true));

        public static double GetTop(DependencyObject obj)
        {
            return (double)obj.GetValue(TopProperty);
        }

        public static void SetTop(DependencyObject obj, double value)
        {
            obj.SetValue(TopProperty, value);
        }

        public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached("Right", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(Double.NaN, affectsArrange: true));

        public static double GetRight(DependencyObject obj)
        {
            return (double)obj.GetValue(RightProperty);
        }

        public static void SetRight(DependencyObject obj, double value)
        {
            obj.SetValue(RightProperty, value);
        }

        public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached("Bottom", typeof(double), typeof(Canvas), new FrameworkPropertyMetadata(Double.NaN, affectsArrange: true));

        public static double GetBottom(DependencyObject obj)
        {
            return (double)obj.GetValue(BottomProperty);
        }

        public static void SetBottom(DependencyObject obj, double value)
        {
            obj.SetValue(BottomProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(Size.Infinity);
            }

            return Size.Zero;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                // Canvas.Left and Canvas.Top have higher priority
                double x = GetLeft(child).DefaultIfNaN(finalSize.Width - child.DesiredSize.Width - GetRight(child)).DefaultIfNaN(0);
                double y = GetTop(child).DefaultIfNaN(finalSize.Height - child.DesiredSize.Height - GetBottom(child)).DefaultIfNaN(0);

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));
            }

            return finalSize;
        }
    }
}
