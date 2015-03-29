using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls.Primitives
{
    public enum PlacementMode
    {
        Absolute,
        Relative,
        Bottom,
        Center,
        Right,
        AbsolutePoint,
        RelativePoint,
        Mouse,
        MousePoint,
        Left,
        Top,
        //Custom
    }

    // an implementation of the "Popup Placement Behavior" described in MSDN
    public static class Placement
    {
        private abstract class PlacementBase
        {
            public Point GetPosition(Rect placementTargetBounds, Rect placementRectangle, Rect mouseBounds, Point offset, Size popupSize, Rect containerBounds)
            {
                Rect targetObjectRect = !placementTargetBounds.IsEmpty ? placementTargetBounds : containerBounds;

                Rect targetArea = GetTargetArea(targetObjectRect, placementRectangle, mouseBounds, containerBounds);

                Point targetOrigin = GetTargetOrigin(targetArea);
                Point popupAlignmentPoint = GetPopupAlignmentPoint(popupSize);

                // calculate initial position
                Point position = GetPosition(targetOrigin, popupAlignmentPoint, offset);

                // check edges overflow and get alternative origin and alignment points
                if (position.X < containerBounds.Left)
                {
                    targetOrigin = GetLeftEdgeTargetOrigin(targetArea, targetOrigin);
                    popupAlignmentPoint = GetLeftEdgePopupAlignmentPoint(popupSize, popupAlignmentPoint);
                }

                if (position.Y < containerBounds.Top)
                {
                    targetOrigin = GetTopEdgeTargetOrigin(targetArea, targetOrigin);
                    popupAlignmentPoint = GetTopEdgePopupAlignmentPoint(popupSize, popupAlignmentPoint);
                }

                if (position.X + popupSize.Width > containerBounds.Right)
                {
                    targetOrigin = GetRightEdgeTargetOrigin(targetArea, targetOrigin);
                    popupAlignmentPoint = GetRightEdgePopupAlignmentPoint(popupSize, popupAlignmentPoint);
                }

                if (position.Y + popupSize.Height > containerBounds.Bottom)
                {
                    targetOrigin = GetBottomEdgeTargetOrigin(targetArea, targetOrigin);
                    popupAlignmentPoint = GetBottomEdgePopupAlignmentPoint(popupSize, popupAlignmentPoint);
                }

                // recalculate position with the alternative origin and alignment points
                position = GetPosition(targetOrigin, popupAlignmentPoint, offset);

                // after the recalculation, overflow can occur from opposite edges if the containerBounds is too small, so apply bounds
                return position.Bounds(containerBounds.GetTopLeft(), containerBounds.GetBottomRight() - popupSize.GetBottomRight());
            }

            protected virtual Rect GetTargetArea(Rect targetObjectRect, Rect placementRectangle, Rect mouseBounds, Rect containerBounds)
            {
                return !placementRectangle.IsEmpty ? placementRectangle.AddOffset(targetObjectRect.Location) : targetObjectRect;
            }

            protected virtual Point GetTargetOrigin(Rect targetArea)
            {
                return targetArea.GetTopLeft();
            }

            protected virtual Point GetPopupAlignmentPoint(Size popupSize)
            {
                return popupSize.GetTopLeft();
            }

            protected virtual Point GetLeftEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return currentTargetOrigin;
            }

            protected virtual Point GetRightEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return currentTargetOrigin;
            }

            protected virtual Point GetTopEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return currentTargetOrigin;
            }

            protected virtual Point GetBottomEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return currentTargetOrigin;
            }

            protected virtual Point GetLeftEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return currentAlignmentPoint;
            }

            protected virtual Point GetRightEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return currentAlignmentPoint;
            }

            protected virtual Point GetTopEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return currentAlignmentPoint;
            }

            protected virtual Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return currentAlignmentPoint;
            }

            private static Point GetPosition(Point targetOrigin, Point popupAlignmentPoint, Point offset)
            {
                return new Point(targetOrigin.X + offset.X - popupAlignmentPoint.X, targetOrigin.Y + offset.Y - popupAlignmentPoint.Y);
            }
        }

        private class AbsolutePlacement : PlacementBase
        {
            public static readonly AbsolutePlacement Default = new AbsolutePlacement();

            protected override Rect GetTargetArea(Rect targetObjectRect, Rect placementRectangle, Rect mouseBounds, Rect containerBounds)
            {
                return !placementRectangle.IsEmpty ? placementRectangle.AddOffset(containerBounds.Location) : containerBounds;
            }
        }

        private class RelativePlacement : PlacementBase
        {
            public static readonly RelativePlacement Default = new RelativePlacement();
        }

        private class BottomPlacement : PlacementBase
        {
            public static readonly BottomPlacement Default = new BottomPlacement();

            protected override Point GetTargetOrigin(Rect targetArea)
            {
 	             return targetArea.GetBottomLeft();
            }

            protected override Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, popupSize.Height);
            }

            protected override Point GetBottomEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return new Point(currentTargetOrigin.X, targetArea.Top);
            }
        }

        private class CenterPlacement : PlacementBase
        {
            public static readonly CenterPlacement Default = new CenterPlacement();

            protected override Point GetTargetOrigin(Rect targetArea)
            {
                return new Point(targetArea.Left + targetArea.Width / 2, targetArea.Top + targetArea.Height / 2);
            }

            protected override Point GetPopupAlignmentPoint(Size popupSize)
            {
                return new Point(popupSize.Width / 2, popupSize.Height / 2);
            }
        }

        private class RightPlacement : PlacementBase
        {
            public static readonly RightPlacement Default = new RightPlacement();

            protected override Point GetTargetOrigin(Rect targetArea)
            {
                return targetArea.GetTopRight();
            }

            protected override Point GetRightEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return new Point(targetArea.Left, currentTargetOrigin.Y);
            }

            protected override Point GetRightEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(popupSize.Width, currentAlignmentPoint.Y);
            }
        }

        private class AbsolutePointPlacement : PlacementBase
        {
            public static readonly AbsolutePointPlacement Default = new AbsolutePointPlacement();

            protected override Rect GetTargetArea(Rect targetObjectRect, Rect placementRectangle, Rect mouseBounds, Rect containerBounds)
            {
                return !placementRectangle.IsEmpty ? placementRectangle.AddOffset(containerBounds.Location) : containerBounds;
            }

            protected override Point GetRightEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(popupSize.Width, currentAlignmentPoint.Y);
            }

            protected override Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, popupSize.Height);
            }
        }

        private class RelativePointPlacement : PlacementBase
        {
            public static readonly RelativePointPlacement Default = new RelativePointPlacement();

            protected override Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, popupSize.Height);
            }

            protected override Point GetRightEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(popupSize.Width, currentAlignmentPoint.Y);
            }
        }

        private class MousePlacement : PlacementBase
        {
            public static readonly MousePlacement Default = new MousePlacement();

            protected override Rect GetTargetArea(Rect targetObjectRect, Rect placementRectangle, Rect mouseBounds, Rect containerBounds)
            {
                return mouseBounds;
            }

            protected override Point GetTargetOrigin(Rect targetArea)
            {
                return targetArea.GetBottomLeft();
            }

            protected override Point GetBottomEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return new Point(currentTargetOrigin.X, targetArea.Top);
            }

            protected override Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, popupSize.Height);
            }
        }

        private class MousePointPlacement : PlacementBase
        {
            public static readonly MousePointPlacement Default = new MousePointPlacement();

            protected override Rect GetTargetArea(Rect targetObjectRect, Rect placementRectangle, Rect mouseBounds, Rect containerBounds)
            {
                return mouseBounds;
            }

            protected override Point GetBottomEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, popupSize.Height);
            }

            protected override Point GetRightEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(popupSize.Width, currentAlignmentPoint.Y);
            }
        }

        private class LeftPlacement : PlacementBase
        {
            public static readonly LeftPlacement Default = new LeftPlacement();

            protected override Point GetPopupAlignmentPoint(Size popupSize)
            {
                return popupSize.GetTopRight();
            }

            protected override Point GetLeftEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return new Point(targetArea.Right, currentTargetOrigin.Y);
            }

            protected override Point GetLeftEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(0, currentAlignmentPoint.Y);
            }
        }

        private class TopPlacement : PlacementBase
        {
            public static readonly TopPlacement Default = new TopPlacement();

            protected override Point GetPopupAlignmentPoint(Size popupSize)
            {
                return popupSize.GetBottomLeft();
            }

            protected override Point GetTopEdgeTargetOrigin(Rect targetArea, Point currentTargetOrigin)
            {
                return new Point(currentTargetOrigin.X, targetArea.Bottom);
            }

            protected override Point GetTopEdgePopupAlignmentPoint(Size popupSize, Point currentAlignmentPoint)
            {
                return new Point(currentAlignmentPoint.X, 0);
            }
        }

        public static Point GetPosition(PlacementMode placementMode, Rect placementTargetRect, Rect placementRectangle, Rect mouseBounds, Point offset, Size popupSize, Rect containerBounds)
        {
            return GetPlacement(placementMode).GetPosition(placementTargetRect, placementRectangle, mouseBounds, offset, popupSize, containerBounds);
        }

        private static PlacementBase GetPlacement(PlacementMode placementMode)
        {
            switch (placementMode)
            {
                case PlacementMode.Absolute: return AbsolutePlacement.Default;
                case PlacementMode.Relative: return RelativePlacement.Default;
                case PlacementMode.Bottom: return BottomPlacement.Default;
                case PlacementMode.Center: return CenterPlacement.Default;
                case PlacementMode.Right: return RightPlacement.Default;
                case PlacementMode.AbsolutePoint: return AbsolutePointPlacement.Default;
                case PlacementMode.RelativePoint: return RelativePointPlacement.Default;
                case PlacementMode.Mouse: return MousePlacement.Default;
                case PlacementMode.MousePoint: return MousePointPlacement.Default;
                case PlacementMode.Left: return LeftPlacement.Default;
                case PlacementMode.Top: return TopPlacement.Default;
            }

            throw new Granular.Exception("Unexpected PlacementMode \"{0}\"", placementMode);
        }

        private static Point GetTopLeft(this Size size)
        {
            return Point.Zero;
        }

        private static Point GetTopRight(this Size size)
        {
            return new Point(size.Width, 0);
        }

        private static Point GetBottomLeft(this Size size)
        {
            return new Point(0, size.Height);
        }

        private static Point GetBottomRight(this Size size)
        {
            return new Point(size.Width, size.Height);
        }
    }
}
