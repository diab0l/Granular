using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows.Input
{
    public static class KeyboardNavigationTarget
    {
        private static readonly IStopComparerProvider TabStopComparerProvider = new StopComparerProvider(currentStop => TabStopComparer.Default);
        private static readonly IStopComparerProvider LeftBoundStopComparerProvider = new StopComparerProvider(currentStop => new LeftBoundStopComparer(currentStop));
        private static readonly IStopComparerProvider RightBoundStopComparerProvider = new StopComparerProvider(currentStop => new RightBoundStopComparer(currentStop));
        private static readonly IStopComparerProvider TopBoundStopComparerProvider = new StopComparerProvider(currentStop => new TopBoundStopComparer(currentStop));
        private static readonly IStopComparerProvider BottomBoundStopComparerProvider = new StopComparerProvider(currentStop => new BottomBoundStopComparer(currentStop));

        private class Stop
        {
            public Visual Element { get; private set; }
            public int TabIndex { get; private set; }

            public Stop(Visual element) :
                this(element, KeyboardNavigation.GetTabIndex(element))
            {
                //
            }

            public Stop(Visual element, int tabIndex)
            {
                this.Element = element;
                this.TabIndex = tabIndex;
            }

            public override string ToString()
            {
                return String.Format("Stop({0}, {1})", Element, TabIndex);
            }
        }

        private interface IStopComparerProvider
        {
            IComparer<Stop> CreateComparer(Stop currentStop);
        }

        private class StopComparerProvider : IStopComparerProvider
        {
            private Func<Stop, IComparer<Stop>> createComparer;

            public StopComparerProvider(Func<Stop, IComparer<Stop>> createComparer)
            {
                this.createComparer = createComparer;
            }

            public IComparer<Stop> CreateComparer(Stop currentStop)
            {
                return createComparer(currentStop);
            }
        }

        private class TabStopComparer : IComparer<Stop>
        {
            public static readonly TabStopComparer Default = new TabStopComparer();

            private TabStopComparer()
            {
                //
            }

            public int Compare(Stop x, Stop y)
            {
                return x.TabIndex.CompareTo(y.TabIndex);
            }
        }

        private class RightBoundStopComparer : IComparer<Stop>
        {
            private Point currentPosition;

            public RightBoundStopComparer(Stop currentStop)
            {
                currentPosition = KeyboardNavigationTarget.GetRightBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(currentStop.Element));
            }

            public int Compare(Stop x, Stop y)
            {
                Point relativePosition1 = KeyboardNavigationTarget.GetLeftBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(x.Element)) - currentPosition;
                Point relativePosition2 = KeyboardNavigationTarget.GetLeftBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(y.Element)) - currentPosition;

                double distance1 = relativePosition1.X < 0 ? -relativePosition1.GetLengthSqr() : relativePosition1.GetLengthSqr();
                double distance2 = relativePosition2.X < 0 ? -relativePosition2.GetLengthSqr() : relativePosition2.GetLengthSqr();

                return distance1.CompareTo(distance2);
            }
        }

        private class LeftBoundStopComparer : IComparer<Stop>
        {
            private Point currentPosition;

            public LeftBoundStopComparer(Stop currentStop)
            {
                currentPosition = KeyboardNavigationTarget.GetLeftBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(currentStop.Element));
            }

            public int Compare(Stop x, Stop y)
            {
                Point relativePosition1 = KeyboardNavigationTarget.GetRightBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(x.Element)) - currentPosition;
                Point relativePosition2 = KeyboardNavigationTarget.GetRightBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(y.Element)) - currentPosition;

                double distance1 = relativePosition1.X <= 0 ? -relativePosition1.GetLengthSqr() : relativePosition1.GetLengthSqr();
                double distance2 = relativePosition2.X <= 0 ? -relativePosition2.GetLengthSqr() : relativePosition2.GetLengthSqr();

                return distance1.CompareTo(distance2);
            }
        }

        private class TopBoundStopComparer : IComparer<Stop>
        {
            private Point currentPosition;

            public TopBoundStopComparer(Stop currentStop)
            {
                currentPosition = KeyboardNavigationTarget.GetTopBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(currentStop.Element));
            }

            public int Compare(Stop x, Stop y)
            {
                Point relativePosition1 = KeyboardNavigationTarget.GetBottomBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(x.Element)) - currentPosition;
                Point relativePosition2 = KeyboardNavigationTarget.GetBottomBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(y.Element)) - currentPosition;

                double distance1 = relativePosition1.Y <= 0 ? -relativePosition1.GetLengthSqr() : relativePosition1.GetLengthSqr();
                double distance2 = relativePosition2.Y <= 0 ? -relativePosition2.GetLengthSqr() : relativePosition2.GetLengthSqr();

                return distance1.CompareTo(distance2);
            }
        }

        private class BottomBoundStopComparer : IComparer<Stop>
        {
            private Point currentPosition;

            public BottomBoundStopComparer(Stop currentStop)
            {
                currentPosition = KeyboardNavigationTarget.GetBottomBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(currentStop.Element));
            }

            public int Compare(Stop x, Stop y)
            {
                Point relativePosition1 = KeyboardNavigationTarget.GetTopBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(x.Element)) - currentPosition;
                Point relativePosition2 = KeyboardNavigationTarget.GetTopBoundPosition(KeyboardNavigationTarget.GetAbsoluteBounds(y.Element)) - currentPosition;

                double distance1 = relativePosition1.Y < 0 ? -relativePosition1.GetLengthSqr() : relativePosition1.GetLengthSqr();
                double distance2 = relativePosition2.Y < 0 ? -relativePosition2.GetLengthSqr() : relativePosition2.GetLengthSqr();

                return distance1.CompareTo(distance2);
            }
        }

        private interface INavigation
        {
            // find contained targets (scope or one of its children)
            Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider);
            Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider);
            Visual FindFirstTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider);
            Visual FindLastTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider);

            // get translated / filtered stops, must contain currentElement stop if it's a descendant of scope
            IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty);
        }

        private abstract class BaseNavigation : INavigation
        {
            // forward the find request to the parent or find a contained target (scope or one of its children)

            public virtual Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return scope.VisualParent != null ?
                    KeyboardNavigationTarget.FindNextTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) :
                    KeyboardNavigationTarget.FindNextContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public virtual Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return scope.VisualParent != null ?
                    KeyboardNavigationTarget.FindPreviousTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) :
                    KeyboardNavigationTarget.FindPreviousContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public virtual Visual FindFirstTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return scope.VisualParent != null ?
                    KeyboardNavigationTarget.FindFirstTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) :
                    KeyboardNavigationTarget.FindFirstContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public virtual Visual FindLastTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return scope.VisualParent != null ?
                    KeyboardNavigationTarget.FindLastTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) :
                    KeyboardNavigationTarget.FindLastContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public virtual IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
            {
                return KeyboardNavigationTarget.GetContainedStops(scope, currentElement, navigationModeProperty);
            }
        }

        private class ContinueNavigation : BaseNavigation
        {
            public static readonly ContinueNavigation Default = new ContinueNavigation();

            private ContinueNavigation()
            {
                //
            }
        }

        private class OnceNavigation : BaseNavigation
        {
            public static readonly OnceNavigation Default = new OnceNavigation();

            private OnceNavigation()
            {
                //
            }

            public override Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                // forward the request to the parent
                return scope.VisualParent != null ? KeyboardNavigationTarget.FindNextTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) : null;
            }

            public override Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                // forward the request to the parent
                return scope.VisualParent != null ? KeyboardNavigationTarget.FindPreviousTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) : null;
            }

            public override IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
            {
                if (IsStop((UIElement)scope) || scope == currentElement)
                {
                    yield return new Stop(scope);
                }

                VisualWeakReference navigationFocusElementReference = KeyboardNavigation.GetNavigationFocusElement(scope);
                Visual navigationFocusElement = navigationFocusElementReference != null ? navigationFocusElementReference.Visual : null;

                Stop[] stops = scope.VisualChildren.SelectMany(child => KeyboardNavigationTarget.GetGlobalStops(child, currentElement, navigationModeProperty)).ToArray();

                if (stops.Any())
                {
                    stops = stops.Where(stop => stop.Element == currentElement || stop.Element == navigationFocusElement).DefaultIfEmpty(stops.First()).ToArray();

                    foreach (Stop stop in stops)
                    {
                        yield return stop;
                    }
                }
            }
        }

        private class CycleNavigation : BaseNavigation
        {
            public static readonly CycleNavigation Default = new CycleNavigation();

            private CycleNavigation()
            {
                //
            }

            public override Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindNextContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ?? KeyboardNavigationTarget.FindFirstContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public override Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindPreviousContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ?? KeyboardNavigationTarget.FindLastContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public override Visual FindFirstTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindFirstContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public override Visual FindLastTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindLastContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }
        }

        private class NoneNavigation : BaseNavigation
        {
            public static readonly NoneNavigation Default = new NoneNavigation();

            private NoneNavigation()
            {
                //
            }

            public override Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                // forward the request to the parent
                return scope.VisualParent != null ? KeyboardNavigationTarget.FindNextTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) : null;
            }

            public override Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                // forward the request to the parent
                return scope.VisualParent != null ? KeyboardNavigationTarget.FindPreviousTarget(scope.VisualParent, currentStop, navigationModeProperty, stopComparerProvider) : null;
            }

            public override IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
            {
                if (IsStop((UIElement)scope) || scope == currentElement)
                {
                    yield return new Stop(scope);
                }

                // add currentElement stop if it's a descendant of scope
                if (scope.IsAncestorOf(currentElement))
                {
                    IEnumerable<Stop> childrenStop = scope.VisualChildren.SelectMany(child => KeyboardNavigationTarget.GetGlobalStops(child, currentElement, navigationModeProperty));
                    yield return childrenStop.First(childStop => childStop.Element == currentElement);
                }
            }
        }

        private class ContainedNavigation : BaseNavigation
        {
            public static readonly ContainedNavigation Default = new ContainedNavigation();

            private ContainedNavigation()
            {
                //
            }

            public override Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindNextContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ?? currentStop.Element; // stay at the edge
            }

            public override Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindPreviousContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ?? currentStop.Element; // stay at the edge
            }

            public override Visual FindFirstTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindFirstContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }

            public override Visual FindLastTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindLastContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
            }
        }

        private class LocalNavigation : BaseNavigation
        {
            public static readonly LocalNavigation Default = new LocalNavigation();

            private LocalNavigation()
            {
                //
            }

            public override Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindNextContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ??
                    (scope.VisualParent != null ? KeyboardNavigationTarget.FindNextTarget(scope.VisualParent, new Stop(currentStop.Element, KeyboardNavigation.GetTabIndex(scope)), navigationModeProperty, stopComparerProvider) : null); // translate currentStop and forward request to parent
            }

            public override Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
            {
                return KeyboardNavigationTarget.FindPreviousContainedTarget(scope, currentStop, navigationModeProperty, stopComparerProvider) ??
                    (scope.VisualParent != null ? KeyboardNavigationTarget.FindPreviousTarget(scope.VisualParent, new Stop(currentStop.Element, KeyboardNavigation.GetTabIndex(scope)), navigationModeProperty, stopComparerProvider) : null); // translate currentStop and forward request to parent
            }

            public override IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
            {
                int scopeTabIndex = KeyboardNavigation.GetTabIndex(scope);

                if (IsStop((UIElement)scope) || scope == currentElement)
                {
                    yield return new Stop(scope, scopeTabIndex);
                }

                // translate stops to have scope tab index instead of local tab index, local order is kept
                foreach (Stop stop in scope.VisualChildren.SelectMany(child => KeyboardNavigationTarget.GetGlobalStops(child, currentElement, navigationModeProperty)).OrderBy(childStop => childStop.TabIndex))
                {
                    yield return new Stop(stop.Element, scopeTabIndex);
                }
            }
        }

        public static Visual FindTarget(Visual currentElement, FocusNavigationDirection direction, DependencyProperty navigationModeProperty)
        {
            Stop currentStop = new Stop(currentElement);

            switch (direction)
            {
                case FocusNavigationDirection.Next: return FindNextTarget(currentStop.Element, currentStop, navigationModeProperty, TabStopComparerProvider);
                case FocusNavigationDirection.Previous: return FindPreviousTarget(currentStop.Element, currentStop, navigationModeProperty, TabStopComparerProvider);
                case FocusNavigationDirection.First: return FindFirstTarget(currentStop.Element, currentStop, navigationModeProperty, TabStopComparerProvider);
                case FocusNavigationDirection.Last: return FindLastTarget(currentStop.Element, currentStop, navigationModeProperty, TabStopComparerProvider);
                case FocusNavigationDirection.Left: return FindPreviousTarget(currentStop.Element, currentStop, navigationModeProperty, LeftBoundStopComparerProvider);
                case FocusNavigationDirection.Right: return FindNextTarget(currentStop.Element, currentStop, navigationModeProperty, RightBoundStopComparerProvider);
                case FocusNavigationDirection.Up: return FindPreviousTarget(currentStop.Element, currentStop, navigationModeProperty, TopBoundStopComparerProvider);
                case FocusNavigationDirection.Down: return FindNextTarget(currentStop.Element, currentStop, navigationModeProperty, BottomBoundStopComparerProvider);
            }

            throw new Granular.Exception("Unexpected navigation direction \"{0}\"", direction);
        }

        private static INavigation GetNavigation(Visual scope, DependencyProperty navigationModeProperty)
        {
            KeyboardNavigationMode scopeNavigationMode = (KeyboardNavigationMode)scope.GetValue(navigationModeProperty);

            switch (scopeNavigationMode)
            {
                case KeyboardNavigationMode.Continue: return ContinueNavigation.Default;
                case KeyboardNavigationMode.Once: return OnceNavigation.Default;
                case KeyboardNavigationMode.Cycle: return CycleNavigation.Default;
                case KeyboardNavigationMode.None: return NoneNavigation.Default;
                case KeyboardNavigationMode.Contained: return ContainedNavigation.Default;
                case KeyboardNavigationMode.Local: return LocalNavigation.Default;
            }

            throw new Granular.Exception("Unexpected KeyboardNavigationMode \"{0}\"", scopeNavigationMode);
        }

        private static Visual FindNextTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            return GetNavigation(scope, navigationModeProperty).FindNextTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
        }

        private static Visual FindPreviousTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            return GetNavigation(scope, navigationModeProperty).FindPreviousTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
        }

        private static Visual FindFirstTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            return GetNavigation(scope, navigationModeProperty).FindFirstTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
        }

        private static Visual FindLastTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            return GetNavigation(scope, navigationModeProperty).FindLastTarget(scope, currentStop, navigationModeProperty, stopComparerProvider);
        }

        private static Visual FindNextContainedTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            IComparer<Stop> stopComparer = stopComparerProvider.CreateComparer(currentStop);

            bool passedCurrentStop = false;

            Stop targetStop = null;

            foreach (Stop stop in GetContainedStops(scope, currentStop.Element, navigationModeProperty))
            {
                if (stop.Element == currentStop.Element)
                {
                    passedCurrentStop = true;
                    continue;
                }

                int compareResult = stopComparer.Compare(currentStop, stop);
                if ((compareResult < 0 || compareResult == 0 && passedCurrentStop) && // select stops with priority higher than currentStop, or the same priority after currentStop
                    (targetStop == null || stopComparer.Compare(targetStop, stop) > 0))
                {
                    targetStop = stop;
                }
            }

            return targetStop != null ? targetStop.Element : null;
        }

        private static Visual FindPreviousContainedTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            IComparer<Stop> stopComparer = stopComparerProvider.CreateComparer(currentStop);

            bool passedCurrentStop = false;

            Stop targetStop = null;

            foreach (Stop stop in GetContainedStops(scope, currentStop.Element, navigationModeProperty))
            {
                if (stop.Element == currentStop.Element)
                {
                    passedCurrentStop = true;
                    continue;
                }

                int compareResult = stopComparer.Compare(currentStop, stop);
                if ((compareResult > 0 || compareResult == 0 && !passedCurrentStop) && // select stops with priority lower than currentStop, or the same priority before currentStop
                    (targetStop == null || stopComparer.Compare(targetStop, stop) <= 0))
                {
                    targetStop = stop;
                }
            }

            return targetStop != null ? targetStop.Element : null;
        }

        private static Visual FindFirstContainedTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            IComparer<Stop> stopComparer = stopComparerProvider.CreateComparer(currentStop);

            Stop targetStop = null;

            foreach (Stop stop in GetContainedStops(scope, currentStop.Element, navigationModeProperty))
            {
                if (targetStop == null || stopComparer.Compare(targetStop, stop) > 0)
                {
                    targetStop = stop;
                }
            }

            return targetStop != null ? targetStop.Element : null;
        }

        private static Visual FindLastContainedTarget(Visual scope, Stop currentStop, DependencyProperty navigationModeProperty, IStopComparerProvider stopComparerProvider)
        {
            IComparer<Stop> stopComparer = stopComparerProvider.CreateComparer(currentStop);

            Stop targetStop = null;

            foreach (Stop stop in GetContainedStops(scope, currentStop.Element, navigationModeProperty))
            {
                if (targetStop == null || stopComparer.Compare(targetStop, stop) <= 0)
                {
                    targetStop = stop;
                }
            }

            return targetStop != null ? targetStop.Element : null;
        }

        private static bool IsStop(UIElement element)
        {
            return KeyboardNavigation.GetIsTabStop(element) && element.IsVisible && element.IsEnabled && element.Focusable;
        }

        private static IEnumerable<Stop> GetGlobalStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
        {
            return GetNavigation(scope, navigationModeProperty).GetGlobalStops(scope, currentElement, navigationModeProperty);
        }

        // get scope stop and scope children global stops
        private static IEnumerable<Stop> GetContainedStops(Visual scope, Visual currentElement, DependencyProperty navigationModeProperty)
        {
            if (IsStop((UIElement)scope) || scope == currentElement)
            {
                yield return new Stop(scope);
            }

            foreach (Stop stop in scope.VisualChildren.SelectMany(child => GetGlobalStops(child, currentElement, navigationModeProperty)))
            {
                yield return stop;
            }
        }

        private static Rect GetAbsoluteBounds(Visual element)
        {
            return new Rect(element.PointToRoot(Point.Zero), element.VisualSize);
        }

        private static Point GetLeftBoundPosition(Rect bounds)
        {
            return bounds.Location + new Point(0, bounds.Height / 2);
        }

        private static Point GetRightBoundPosition(Rect bounds)
        {
            return bounds.Location + new Point(bounds.Width, bounds.Height / 2);
        }

        private static Point GetTopBoundPosition(Rect bounds)
        {
            return bounds.Location + new Point(bounds.Width / 2, 0);
        }

        private static Point GetBottomBoundPosition(Rect bounds)
        {
            return bounds.Location + new Point(bounds.Width / 2, bounds.Height);
        }
    }
}
