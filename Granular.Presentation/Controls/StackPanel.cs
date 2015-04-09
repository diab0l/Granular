using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public enum Orientation
    {
        Horizontal,
        Vertical
    }

    public class StackPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel), new FrameworkPropertyMetadata(Orientation.Vertical));
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty FlowDirectionProperty = DependencyProperty.Register("FlowDirection", typeof(FlowDirection), typeof(StackPanel), new FrameworkPropertyMetadata(FlowDirection.TopDown));
        public FlowDirection FlowDirection
        {
            get { return (FlowDirection)GetValue(FlowDirectionProperty); }
            set { SetValue(FlowDirectionProperty, value); }
        }

        private bool IsNormalFlow { get { return FlowDirection == Controls.FlowDirection.LeftToRight || FlowDirection == Controls.FlowDirection.TopDown; } }

        protected override Size MeasureOverride(Size availableSize)
        {
            double mainLength = 0;
            double crossLength = 0;

            foreach (FrameworkElement child in Children)
            {
                MeasureChild(child, Double.PositiveInfinity, GetCrossLength(availableSize));

                mainLength += GetMainLength(child.DesiredSize);
                crossLength = Math.Max(crossLength, GetCrossLength(child.DesiredSize));
            }

            return CreateSize(mainLength, crossLength);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double panelMainLength = Children.Select(child => GetMainLength(child.DesiredSize)).Sum();
            double panelCrossLength = GetCrossLength(finalSize);

            double childrenMainLength = 0;
            foreach (UIElement child in Children)
            {
                double childMainLength = GetMainLength(child.DesiredSize);
                double childMainStart = IsNormalFlow ? childrenMainLength : panelMainLength - childrenMainLength - childMainLength;

                ArrangeChild(child, childMainStart, 0, childMainLength, panelCrossLength);

                childrenMainLength += childMainLength;
            }

            return CreateSize(GetMainLength(finalSize), panelCrossLength);
        }


        private Size CreateSize(double mainLength, double crossLength)
        {
            if (Orientation == Orientation.Horizontal)
            {
                return new Size(mainLength, crossLength);
            }

            return new Size(crossLength, mainLength);
        }

        private double GetMainLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetCrossLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Height : size.Width;
        }

        private void MeasureChild(UIElement child, double availableMainLength, double availableCrossLength)
        {
            child.Measure(Orientation == Orientation.Horizontal ?
                new Size(availableMainLength, availableCrossLength) :
                new Size(availableCrossLength, availableMainLength));
        }

        private void ArrangeChild(UIElement child, double finalMainStart, double finalCrossStart, double finalMainLength, double finalCrossLength)
        {
            child.Arrange(Orientation == Orientation.Horizontal ?
                new Rect(finalMainStart, finalCrossStart, finalMainLength, finalCrossLength) :
                new Rect(finalCrossStart, finalMainStart, finalCrossLength, finalMainLength));
        }
    }
}
