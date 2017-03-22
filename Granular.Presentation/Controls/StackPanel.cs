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

        private bool IsNormalFlow { get { return FlowDirection == FlowDirection.LeftToRight || FlowDirection == FlowDirection.TopDown; } }

        private double measuredCrossLength;

        protected override Size MeasureOverride(Size availableSize)
        {
            double availableCrossLength = GetCrossLength(availableSize);
            Size measureSize = CreateSize(Orientation, Double.PositiveInfinity, availableCrossLength);

            double mainLength = 0;
            double crossLength = 0;

            foreach (FrameworkElement child in Children)
            {
                child.Measure(measureSize);

                mainLength += GetMainLength(child.DesiredSize);
                crossLength = Math.Max(crossLength, GetCrossLength(child.DesiredSize));
            }

            measuredCrossLength = availableCrossLength;

            return CreateSize(Orientation, mainLength, crossLength);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double panelMainLength = Children.Select(child => GetMainLength(child.DesiredSize)).Sum();
            double panelCrossLength = GetCrossLength(finalSize);

            if (measuredCrossLength != panelCrossLength)
            {
                Size measureSize = CreateSize(Orientation, Double.PositiveInfinity, panelCrossLength);

                foreach (FrameworkElement child in Children)
                {
                    child.Measure(measureSize);
                }

                measuredCrossLength = panelCrossLength;
            }

            double childrenMainLength = 0;
            foreach (UIElement child in Children)
            {
                double childMainLength = GetMainLength(child.DesiredSize);
                double childMainStart = IsNormalFlow ? childrenMainLength : panelMainLength - childrenMainLength - childMainLength;

                child.Arrange(CreateRect(Orientation, childMainStart, 0, childMainLength, panelCrossLength));

                childrenMainLength += childMainLength;
            }

            return CreateSize(Orientation, GetMainLength(finalSize), panelCrossLength);
        }

        private double GetMainLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Width : size.Height;
        }

        private double GetCrossLength(Size size)
        {
            return Orientation == Orientation.Horizontal ? size.Height : size.Width;
        }

        private static Size CreateSize(Orientation orientation, double mainLength, double crossLength)
        {
            return orientation == Orientation.Horizontal ?
                new Size(mainLength, crossLength) :
                new Size(crossLength, mainLength);
        }

        private static Rect CreateRect(Orientation orientation, double mainStart, double crossStart, double mainLength, double crossLength)
        {
            return orientation == Orientation.Horizontal ?
                new Rect(mainStart, crossStart, mainLength, crossLength) :
                new Rect(crossStart, mainStart, crossLength, mainLength);
        }
    }
}
