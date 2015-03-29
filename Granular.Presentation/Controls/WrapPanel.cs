using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public class WrapPanel : Panel
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel), new FrameworkPropertyMetadata());
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!this.Children.Any())
            {
                return Size.Zero;
            }

            foreach (UIElement child in Children)
            {
                MeasureChild(child, GetMainLength(availableSize), GetCrossLength(availableSize));
            }

            IEnumerable<IEnumerable<UIElement>> groups = GetElementGroups(availableSize);

            double mainLength = groups.Select(group => group.Select(child => GetMainLength(child.DesiredSize)).Sum()).Max();
            double crossLength = groups.Select(group => group.Select(child => GetCrossLength(child.DesiredSize)).Max()).Sum();

            return CreateSize(mainLength, crossLength);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!this.Children.Any())
            {
                return Size.Zero;
            }

            IEnumerable<IEnumerable<UIElement>> groups = GetElementGroups(finalSize);

            double maxMainLength = 0;
            double totalCrossLength = 0;

            foreach (IEnumerable<UIElement> group in groups)
            {
                double groupMainLength = 0;
                double groupMaxCrossLength = group.Select(child => GetCrossLength(child.DesiredSize)).Max();

                foreach (UIElement child in group)
                {
                    double childMainLength = GetMainLength(child.DesiredSize);

                    ArrangeChild(child, groupMainLength, totalCrossLength, childMainLength, groupMaxCrossLength);

                    groupMainLength += childMainLength;
                }

                maxMainLength = Math.Max(maxMainLength, groupMainLength);
                totalCrossLength += groupMaxCrossLength;
            }

            return CreateSize(maxMainLength, totalCrossLength);
        }

        // get groups of elements that should be arranged in the same row or column
        private IEnumerable<IEnumerable<UIElement>> GetElementGroups(Size size)
        {
            double mainLength = GetMainLength(size);

            List<IEnumerable<UIElement>> groups = new List<IEnumerable<UIElement>>();

            List<UIElement> currentGroup = new List<UIElement>();
            double currentGroupMainLength = 0;

            foreach (UIElement child in Children)
            {
                double childMainLength = GetMainLength(child.DesiredSize);

                if (currentGroupMainLength > 0 && currentGroupMainLength + childMainLength > mainLength)
                {
                    groups.Add(currentGroup);

                    // start a new group
                    currentGroup = new List<UIElement>();
                    currentGroupMainLength = 0;
                }

                currentGroupMainLength += childMainLength;
                currentGroup.Add(child);
            }

            groups.Add(currentGroup);
            return groups;
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
    }
}
