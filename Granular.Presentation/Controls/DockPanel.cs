using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public enum Dock
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public class DockPanel : Panel
    {
        public static readonly DependencyProperty DockProperty = DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockPanel), new FrameworkPropertyMetadata(propertyChangedCallback: OnDockChanged));

        public static Dock GetDock(DependencyObject obj)
        {
            return (Dock)obj.GetValue(DockProperty);
        }

        public static void SetDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(DockProperty, value);
        }

        public static readonly DependencyProperty LastChildFillProperty = DependencyProperty.Register("LastChildFill", typeof(bool), typeof(DockPanel), new FrameworkPropertyMetadata(defaultValue: true, affectsMeasure: true));
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double remainingWidth = availableSize.Width;
            double remainingHeight = availableSize.Height;

            foreach (UIElement child in Children)
            {
                child.Measure(new Size(remainingWidth, remainingHeight).Max(Size.Zero));

                if (GetDockOrientation(GetDock(child)) == Orientation.Horizontal)
                {
                    remainingWidth -= child.DesiredSize.Width;
                }
                else
                {
                    remainingHeight -= child.DesiredSize.Height;
                }
            }

            double childrenWidth = 0;
            double childrenHeight = 0;

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                UIElement child = Children[i];

                if (GetDockOrientation(GetDock(child)) == Orientation.Horizontal)
                {
                    childrenWidth += child.DesiredSize.Width;
                    childrenHeight = Math.Max(childrenHeight, child.DesiredSize.Height);
                }
                else
                {
                    childrenHeight += child.DesiredSize.Height;
                    childrenWidth = Math.Max(childrenWidth, child.DesiredSize.Width);
                }
            }

            return new Size(childrenWidth, childrenHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double remainingWidth = finalSize.Width;
            double remainingHeight = finalSize.Height;

            double left = 0;
            double top = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                Dock childDock = GetDock(child);
                Orientation childDockOrientation = GetDockOrientation(childDock);

                bool childFill = LastChildFill && i == Children.Count - 1;

                double cellWidth = childDockOrientation == Orientation.Vertical || childFill ? remainingWidth : child.DesiredSize.Width;
                double cellHeight = childDockOrientation == Orientation.Horizontal || childFill ? remainingHeight : child.DesiredSize.Height;

                double cellLeft = childDock == Dock.Right ? remainingWidth - cellWidth : 0;
                double cellTop = childDock == Dock.Bottom ? remainingHeight - cellHeight : 0;

                child.Arrange(new Rect(left + cellLeft, top + cellTop, cellWidth, cellHeight));

                if (childDockOrientation == Orientation.Horizontal)
                {
                    remainingWidth -= cellWidth;

                    if (childDock == Dock.Left)
                    {
                        left += cellWidth;
                    }
                }
                else
                {
                    remainingHeight -= cellHeight;

                    if (childDock == Dock.Top)
                    {
                        top += cellHeight;
                    }
                }
            }

            return finalSize;
        }

        private static Orientation GetDockOrientation(Dock dock)
        {
            return dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;
        }

        private static void OnDockChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is Visual && ((Visual)dependencyObject).VisualParent is DockPanel)
            {
                ((DockPanel)((Visual)dependencyObject).VisualParent).InvalidateArrange();
            }
        }
    }
}
