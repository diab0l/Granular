using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Granular;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows
{
    public enum Visibility
    {
        Visible,
        Hidden,
        Collapsed
    }

    public partial class UIElement : Visual, IAnimatable, IInputElement
    {
        public event EventHandler LayoutUpdated;

        private UIElement logicalParent;
        public UIElement LogicalParent
        {
            get { return logicalParent; }
            private set
            {
                if (logicalParent == value)
                {
                    return;
                }

                UIElement oldLogicalParent = logicalParent;
                logicalParent = value;
                SetInheritanceParent();
                OnLogicalParentChanged(oldLogicalParent, logicalParent);
            }
        }

        public bool IsMeasureValid { get; private set; }

        public bool IsArrangeValid { get; private set; }

        private Size desiredSize;
        public Size DesiredSize
        {
            get { return desiredSize; }
            set
            {
                if (desiredSize == value)
                {
                    return;
                }

                desiredSize = value;
                if (VisualParent != null)
                {
                    ((UIElement)VisualParent).InvalidateMeasure();
                }
            }
        }

        public Size RenderSize { get { return VisualSize; } }

        private List<object> logicalChildren;
        public IEnumerable<object> LogicalChildren { get; private set; }

        public static readonly DependencyProperty OpacityProperty = DependencyProperty.Register("Opacity", typeof(double), typeof(UIElement), new FrameworkPropertyMetadata(1.0, (sender, e) => (sender as UIElement).VisualOpacity = (double)e.NewValue));
        public double Opacity
        {
            get { return (double)GetValue(OpacityProperty); }
            set { SetValue(OpacityProperty, value); }
        }

        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register("Visibility", typeof(Visibility), typeof(UIElement), new FrameworkPropertyMetadata(Visibility.Visible, affectsMeasure: true, propertyChangedCallback: (sender, e) => ((UIElement)sender).OnVisibilityChanged(e)));
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        private static readonly DependencyPropertyKey IsVisiblePropertyKey = DependencyProperty.RegisterReadOnly("IsVisible", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(true, propertyChangedCallback: (sender, e) => ((UIElement)sender).OnIsVisibleChanged(e), coerceValueCallback: (sender, value) => ((UIElement)sender).CoerceIsVisible((bool)value)));
        public static readonly DependencyProperty IsVisibleProperty = IsVisiblePropertyKey.DependencyProperty;
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisiblePropertyKey); }
            private set { SetValue(IsVisiblePropertyKey, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register("IsEnabled", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(true, propertyChangedCallback: (sender, e) => ((UIElement)sender).OnIsEnabledChanged(e), coerceValueCallback: (sender, value) => ((UIElement)sender).CoerceInheritedValue(IsEnabledProperty, (bool)value)));
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(true, propertyChangedCallback: (sender, e) => ((UIElement)sender).OnIsHitTestVisibleChanged(e), coerceValueCallback: (sender, value) => ((UIElement)sender).CoerceInheritedValue(IsHitTestVisibleProperty, (bool)value)));
        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        public static readonly DependencyProperty ClipToBoundsProperty = DependencyProperty.Register("ClipToBounds", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(true, propertyChangedCallback: (sender, e) => ((UIElement)sender).VisualClipToBounds = (bool)e.NewValue));
        public bool ClipToBounds
        {
            get { return (bool)GetValue(ClipToBoundsProperty); }
            set { SetValue(ClipToBoundsProperty, value); }
        }

        public static readonly DependencyProperty FocusableProperty = DependencyProperty.Register("Focusable", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata(propertyChangedCallback: (sender, e) => ((UIElement)sender).OnFocusableChanged(e)));
        public bool Focusable
        {
            get { return (bool)GetValue(FocusableProperty); }
            set { SetValue(FocusableProperty, value); }
        }

        private static readonly DependencyPropertyKey IsMouseOverPropertyKey = DependencyProperty.RegisterReadOnly("IsMouseOver", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsMouseOverProperty = IsMouseOverPropertyKey.DependencyProperty;
        public bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverPropertyKey); }
            private set { SetValue(IsMouseOverPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsFocusedPropertyKey = DependencyProperty.RegisterReadOnly("IsFocused", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedPropertyKey); }
            private set { SetValue(IsFocusedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsKeyboardFocusedPropertyKey = DependencyProperty.RegisterReadOnly("IsKeyboardFocused", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsKeyboardFocusedProperty = IsKeyboardFocusedPropertyKey.DependencyProperty;
        public bool IsKeyboardFocused
        {
            get { return (bool)GetValue(IsKeyboardFocusedPropertyKey); }
            private set { SetValue(IsKeyboardFocusedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsKeyboardFocusWithinPropertyKey = DependencyProperty.RegisterReadOnly("IsKeyboardFocusWithin", typeof(bool), typeof(UIElement), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IsKeyboardFocusWithinProperty = IsKeyboardFocusWithinPropertyKey.DependencyProperty;
        public bool IsKeyboardFocusWithin
        {
            get { return (bool)GetValue(IsKeyboardFocusWithinPropertyKey); }
            private set { SetValue(IsKeyboardFocusWithinPropertyKey, value); }
        }

        private bool isRootElement;
        public bool IsRootElement
        {
            get { return isRootElement; }
            set
            {
                if (isRootElement == value)
                {
                    return;
                }

                isRootElement = value;
                CoerceValue(IsVisibleProperty);
            }
        }

        private AnimatableRootClock animatableRootClock;
        IRootClock IAnimatable.RootClock
        {
            get
            {
                if (animatableRootClock == null)
                {
                    animatableRootClock = new AnimatableRootClock(RootClock.Default, this.IsVisible);
                }

                return animatableRootClock;
            }
        }

        public Size PreviousAvailableSize { get; private set; }
        public Rect PreviousFinalRect { get; private set; }

        private int measureInvalidationDisabled;
        private ListDictionary<RoutedEvent, RoutedEventHandlerItem> routedEventHandlers;
        private CacheDictionary<RoutedEvent, IEnumerable<RoutedEventHandlerItem>> routedEventHandlersCache;
        private Size previousDesiredSize;
        private IDisposable focus;

        public UIElement()
        {
            logicalChildren = new List<object>();
            LogicalChildren = new ReadOnlyCollection<object>(logicalChildren);
            routedEventHandlers = new ListDictionary<RoutedEvent, RoutedEventHandlerItem>();
            routedEventHandlersCache = new CacheDictionary<RoutedEvent, IEnumerable<RoutedEventHandlerItem>>(ResolveRoutedEventHandlers);
            PreviousFinalRect = Rect.Empty;
            PreviousAvailableSize = Size.Empty;
            previousDesiredSize = Size.Empty;

            VisualClipToBounds = ClipToBounds;
            VisualIsHitTestVisible = IsHitTestVisible;
            VisualIsVisible = IsVisible;
            VisualOpacity = Opacity;
        }

        public void AddLogicalChild(object child)
        {
            UIElement childElement = child as UIElement;

            if (childElement != null)
            {
                if (childElement.LogicalParent == this)
                {
                    return;
                }

                if (childElement.LogicalParent != null)
                {
                    childElement.LogicalParent.RemoveLogicalChild(childElement);
                }

                childElement.LogicalParent = this;
            }

            logicalChildren.Add(child);
        }

        public void RemoveLogicalChild(object child)
        {
            UIElement childElement = child as UIElement;

            if (childElement != null)
            {
                if (childElement.LogicalParent != this)
                {
                    return;
                }

                childElement.LogicalParent = null;
            }

            logicalChildren.Remove(child);
        }

        public void AddHandler(RoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false)
        {
            routedEventHandlers.Add(routedEvent, new RoutedEventHandlerItem(handler, handledEventsToo));
            routedEventHandlersCache.Remove(routedEvent);
        }

        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler)
        {
            routedEventHandlers.Remove(routedEvent, routedEventHandlers.GetValues(routedEvent).FirstOrDefault(item => item.Handler == handler));
            routedEventHandlersCache.Remove(routedEvent);
        }

        public void RaiseEvent(RoutedEventArgs e)
        {
            EventRoute eventRoute = new EventRoute(e.RoutedEvent, GetEventRouteItems(e.RoutedEvent, this));
            e.Source = this;
            eventRoute.InvokeHandlers(e);
        }

        private IEnumerable<EventRouteItem> GetEventRouteItems(RoutedEvent routedEvent, UIElement originalSource)
        {
            object logicalSource = GetClosestLogicalChild(this, originalSource);

            IEnumerable<EventRouteItem> items = GetRoutedEventHandlers(routedEvent).Select(handler => new EventRouteItem(handler, originalSource, logicalSource, this));

            if (routedEvent.RoutingStrategy == RoutingStrategy.Bubble ||
                routedEvent.RoutingStrategy == RoutingStrategy.Tunnel)
            {
                UIElement parent = VisualParent as UIElement;

                if (parent != null)
                {
                    IEnumerable<EventRouteItem> parentItems = parent.GetEventRouteItems(routedEvent, this);

                    if (routedEvent.RoutingStrategy == RoutingStrategy.Bubble)
                    {
                        items = items.Concat(parentItems);
                    }
                    else
                    {
                        items = parentItems.Concat(items);
                    }
                }
            }

            return items.ToArray();
        }

        private IEnumerable<RoutedEventHandlerItem> GetRoutedEventHandlers(RoutedEvent routedEvent)
        {
            return routedEventHandlersCache.GetValue(routedEvent);
        }

        private IEnumerable<RoutedEventHandlerItem> ResolveRoutedEventHandlers(RoutedEvent routedEvent)
        {
            return EventManager.GetFlattenedClassHandlers(GetType(), routedEvent).
                Concat(GetRoutedEventHandlersOverride(routedEvent)).
                Concat(routedEventHandlers.GetValues(routedEvent)).
                ToArray();
        }

        protected IEnumerable<RoutedEventHandlerItem> GetRoutedEventHandlersOverride(RoutedEvent routedEvent)
        {
            return new RoutedEventHandlerItem[0];
        }

        public void UpdateLayout()
        {
            LayoutManager.Current.UpdateLayout();
        }

        public void Measure(Size availableSize)
        {
            using (Dispatcher.CurrentDispatcher.DisableProcessing())
            {
                using (DisableMeasureInvalidation())
                {
                    if (Visibility == Visibility.Collapsed)
                    {
                        LayoutManager.Current.RemoveMeasure(this);
                        DesiredSize = Size.Zero;
                        return;
                    }

                    if (IsMeasureValid && PreviousAvailableSize.IsClose(availableSize))
                    {
                        LayoutManager.Current.RemoveMeasure(this);
                        DesiredSize = previousDesiredSize;
                        return;
                    }

                    InvalidateArrange();

                    DesiredSize = MeasureCore(availableSize);

                    IsMeasureValid = true;

                    PreviousAvailableSize = availableSize;
                    previousDesiredSize = DesiredSize;
                    LayoutManager.Current.RemoveMeasure(this);
                }
            }
        }

        protected virtual Size MeasureCore(Size availableSize)
        {
            return Size.Empty;
        }

        public void InvalidateMeasure()
        {
            if (measureInvalidationDisabled > 0 || !IsMeasureValid && !PreviousAvailableSize.IsEmpty)
            {
                return;
            }

            IsMeasureValid = false;
            LayoutManager.Current.AddMeasure(this);
        }

        public void Arrange(Rect finalRect)
        {
            using (Dispatcher.CurrentDispatcher.DisableProcessing())
            {
                using (DisableMeasureInvalidation())
                {
                    if (Visibility == Visibility.Collapsed)
                    {
                        LayoutManager.Current.RemoveArrange(this);
                        return;
                    }

                    if (IsArrangeValid && finalRect.IsClose(PreviousFinalRect))
                    {
                        LayoutManager.Current.RemoveArrange(this);
                        return;
                    }

                    if (!IsMeasureValid || !PreviousAvailableSize.IsClose(finalRect.Size))
                    {
                        Measure(finalRect.Size);
                    }

                    ArrangeCore(finalRect);

                    PreviousFinalRect = finalRect;
                    IsArrangeValid = true;
                    LayoutManager.Current.RemoveArrange(this);
                    LayoutManager.Current.AddUpdatedElement(this);
                }
            }
        }

        protected virtual void ArrangeCore(Rect finalRect)
        {
            //
        }

        public void InvalidateArrange()
        {
            if (!IsArrangeValid && !PreviousFinalRect.IsEmpty)
            {
                return;
            }

            IsArrangeValid = false;
            LayoutManager.Current.AddArrange(this);
        }

        private IDisposable DisableMeasureInvalidation()
        {
            measureInvalidationDisabled++;
            return new Disposable(() => measureInvalidationDisabled--);
        }

        internal void RaiseLayoutUpdated()
        {
            OnLayoutUpdated();
            LayoutUpdated.Raise(this, EventArgs.Empty);
        }

        protected virtual void OnLayoutUpdated()
        {
            //
        }

        protected override void OnVisualParentChanged(Visual oldVisualParent, Visual newVisualParent)
        {
            SetInheritanceParent();

            CoerceValue(IsVisibleProperty);
            CoerceValue(IsEnabledProperty);
            CoerceValue(IsHitTestVisibleProperty);
        }

        protected virtual void OnLogicalParentChanged(UIElement oldLogicalParent, UIElement newLogicalParent)
        {
            //
        }

        private void SetInheritanceParent()
        {
            SetInheritanceParent(LogicalParent ?? VisualParent);
        }

        public void SetAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner)
        {
            AnimationExpression animationExpression = GetInitializedAnimationExpression(dependencyProperty);
            animationExpression.SetClocks(animationClocks, layerOwner);
        }

        public void AddAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner)
        {
            AnimationExpression animationExpression = GetInitializedAnimationExpression(dependencyProperty);
            animationExpression.AddClocks(animationClocks, layerOwner);
        }

        public void RemoveAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner)
        {
            AnimationExpression animationExpression = GetInitializedAnimationExpression(dependencyProperty);
            animationExpression.RemoveClocks(animationClocks, layerOwner);
        }

        private AnimationExpression GetInitializedAnimationExpression(DependencyProperty dependencyProperty)
        {
            IDependencyPropertyValueEntry entry = GetValueEntry(dependencyProperty);
            AnimationExpression animationExpression = entry.GetAnimationValue(false) as AnimationExpression;

            if (animationExpression == null)
            {
                animationExpression = new AnimationExpression(this, dependencyProperty);

                entry.SetAnimationValue(animationExpression);
            }

            return animationExpression;
        }

        public Visual HitTest(Point position)
        {
            if (!IsHitTestVisible || !IsEnabled || !IsVisible || VisualClipToBounds && !VisualBounds.Contains(position))
            {
                return null;
            }

            Point relativePosition = position - VisualOffset;

            for (int i = VisualChildren.Count - 1; i >= 0; i--)
            {
                Visual childHit = ((UIElement)VisualChildren[i]).HitTest(relativePosition);

                if (childHit != null)
                {
                    return childHit;
                }
            }

            return HitTestOverride(relativePosition) ? this : null;
        }

        protected virtual bool HitTestOverride(Point position)
        {
            return false;
        }

        public void Focus()
        {
            if (!IsFocused && Focusable)
            {
                focus = Disposable.Combine(Keyboard.Focus(this), FocusManager.Focus(this));
            }
        }

        public void ClearFocus()
        {
            if (focus != null)
            {
                focus.Dispose();
                focus = null;
            }
        }

        public void SetAnimatableRootClock(AnimatableRootClock animatableRootClock)
        {
            if (this.animatableRootClock != null)
            {
                throw new Granular.Exception("AnimatableRootClock was already initialized");
            }

            this.animatableRootClock = animatableRootClock;
        }

        private void ForceDefaultValueInheritance(DependencyPropertyChangedEventArgs e)
        {
            // clear modified value if it's equal to a default value that should be inherited
            if (Granular.Compatibility.EqualityComparer.Default.Equals(e.NewValue, e.Property.GetMetadata(GetType()).DefaultValue) && !GetValueSource(e.Property).IsExpression)
            {
                ClearValue(e.Property);
            }
        }

        private void OnVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Visibility != Visibility.Collapsed)
            {
                if (!IsMeasureValid)
                {
                    LayoutManager.Current.AddMeasure(this);
                }

                if (!IsArrangeValid)
                {
                    LayoutManager.Current.AddArrange(this);
                }
            }

            IsVisible = Visibility == Visibility.Visible;
            VisualIsVisible = IsVisible;
        }

        private void OnIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            ClearFocus();

            if (animatableRootClock != null)
            {
                // add or remove animation clocks from the global root clock
                animatableRootClock.IsConnected = IsVisible;
            }

            VisualIsVisible = IsVisible;

            CoerceChildrenInheritedValue(IsVisibleProperty);
        }

        private void OnIsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            ClearFocus();

            CoerceChildrenInheritedValue(IsEnabledProperty);
        }

        private void OnIsHitTestVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            VisualIsHitTestVisible = IsHitTestVisible;

            CoerceChildrenInheritedValue(IsHitTestVisibleProperty);
        }

        private bool CoerceIsVisible(bool value)
        {
            return value && (VisualParent != null ? ((UIElement)VisualParent).IsVisible : IsRootElement);
        }

        private bool CoerceInheritedValue(DependencyProperty dependencyProperty, bool value)
        {
            return VisualParent != null ? value && (bool)VisualParent.GetValue(dependencyProperty) : value;
        }

        private void CoerceChildrenInheritedValue(DependencyProperty dependencyProperty)
        {
            foreach (Visual child in VisualChildren)
            {
                child.CoerceValue(dependencyProperty);
            }
        }

        private void OnFocusableChanged(DependencyPropertyChangedEventArgs e)
        {
            ClearFocus();
        }

        IEnumerable<IInputElement> IInputElement.GetPathFromRoot()
        {
            List<IInputElement> path = new List<IInputElement>();

            Visual element = this;

            while (element != null)
            {
                path.Add((IInputElement)element);
                element = element.VisualParent;
            }

            path.Reverse();

            return path;
        }

        Point IInputElement.GetRelativePosition(Point absolutePosition)
        {
            Visual element = this;

            while (element != null)
            {
                absolutePosition -= element.VisualOffset;
                element = element.VisualParent;
            }

            return absolutePosition;
        }

        public static void AddHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false)
        {
            if (element is UIElement)
            {
                ((UIElement)element).AddHandler(routedEvent, handler, handledEventsToo);
            }
        }

        public static void RemoveHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            if (element is UIElement)
            {
                ((UIElement)element).RemoveHandler(routedEvent, handler);
            }
        }

        private static object GetClosestLogicalChild(UIElement rootElement, UIElement childElement)
        {
            while (childElement != rootElement && childElement != null)
            {
                UIElement element = childElement;

                while (element != null)
                {
                    if (element == rootElement)
                    {
                        return childElement;
                    }

                    element = element.LogicalParent;
                }

                childElement = childElement.VisualParent as UIElement;
            }

            return childElement;
        }
    }
}
