using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public enum HandoffBehavior
    {
        SnapshotAndReplace,
        Compose
    }

    public class Storyboard : ParallelTimeline
    {
        private sealed class TargetKey
        {
            public IAnimatable Target { get; private set; }
            public DependencyProperty TargetProperty { get; private set; }

            public TargetKey(IAnimatable target, DependencyProperty targetProperty)
            {
                this.Target = target;
                this.TargetProperty = targetProperty;
            }

            public override bool Equals(object obj)
            {
                TargetKey other = obj as TargetKey;

                return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                    Object.Equals(this.Target, other.Target) &&
                    Object.Equals(this.TargetProperty, other.TargetProperty);
            }

            public override int GetHashCode()
            {
                return Target.GetHashCode() ^ TargetProperty.GetHashCode();
            }
        }

        public static readonly DependencyProperty TargetNameProperty = DependencyProperty.RegisterAttached("TargetName", typeof(string), typeof(Storyboard), new FrameworkPropertyMetadata());

        public static string GetTargetName(DependencyObject obj)
        {
            return (string)obj.GetValue(TargetNameProperty);
        }

        public static void SetTargetName(DependencyObject obj, string value)
        {
            obj.SetValue(TargetNameProperty, value);
        }

        public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached("Target", typeof(DependencyObject), typeof(Storyboard), new FrameworkPropertyMetadata());

        public static DependencyObject GetTarget(DependencyObject obj)
        {
            return (DependencyObject)obj.GetValue(TargetProperty);
        }

        public static void SetTarget(DependencyObject obj, DependencyObject value)
        {
            obj.SetValue(TargetProperty, value);
        }

        public static readonly DependencyProperty TargetPropertyProperty = DependencyProperty.RegisterAttached("TargetProperty", typeof(PropertyPath), typeof(Storyboard), new FrameworkPropertyMetadata());

        public static PropertyPath GetTargetProperty(DependencyObject obj)
        {
            return (PropertyPath)obj.GetValue(TargetPropertyProperty);
        }

        public static void SetTargetProperty(DependencyObject obj, PropertyPath value)
        {
            obj.SetValue(TargetPropertyProperty, value);
        }

        private Dictionary<FrameworkElement, TimelineClock> clocks;

        public Storyboard()
        {
            clocks = new Dictionary<FrameworkElement, TimelineClock>();
        }

        public void Begin(FrameworkElement containingObject, INameScope nameScope = null, HandoffBehavior handoffBehavior = HandoffBehavior.SnapshotAndReplace, object layerOwner = null)
        {
            Stop(containingObject);

            TimelineClock clock = CreateClock();
            clock.Begin(((IAnimatable)containingObject).RootClock);

            clocks[containingObject] = clock;

            ListDictionary<TargetKey, AnimationTimelineClock> targets = GetAnimationClocksTargets(clock, containingObject, nameScope ?? NameScope.GetContainingNameScope(containingObject));
            foreach (TargetKey target in targets.GetKeys())
            {
                target.Target.ApplyAnimationClocks(target.TargetProperty, targets.GetValues(target), handoffBehavior, layerOwner);
            }
        }

        public void Remove(FrameworkElement containingObject, INameScope nameScope = null, object layerOwner = null)
        {
            Stop(containingObject);

            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                ListDictionary<TargetKey, AnimationTimelineClock> targets = GetAnimationClocksTargets(clock, containingObject, nameScope ?? NameScope.GetContainingNameScope(containingObject));
                foreach (TargetKey target in targets.GetKeys())
                {
                    target.Target.RemoveAnimationClocks(target.TargetProperty, targets.GetValues(target), layerOwner);
                }
                clocks.Remove(containingObject);
            }
        }

        public void Pause(FrameworkElement containingObject)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.Pause();
            }
        }

        public void Resume(FrameworkElement containingObject)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.Resume();
            }
        }

        public void Seek(FrameworkElement containingObject, TimeSpan time)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.Seek(time);
            }
        }

        public void SeekOffset(FrameworkElement containingObject, TimeSpan offset)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.SeekOffset(offset);
            }
        }

        public void SkipToFill(FrameworkElement containingObject)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.SkipToFill();
            }
        }

        public void Stop(FrameworkElement containingObject)
        {
            TimelineClock clock;
            if (clocks.TryGetValue(containingObject, out clock))
            {
                clock.Stop();
            }
        }

        // collect inner animations clocks that define Storyboard.Target, Storyboard.TargetName, or Storyboard.TargetProperty
        private ListDictionary<TargetKey, AnimationTimelineClock> GetAnimationClocksTargets(TimelineClock clock, FrameworkElement containingObject, INameScope nameScope)
        {
            ListDictionary<TargetKey, AnimationTimelineClock> targets = new ListDictionary<TargetKey, AnimationTimelineClock>();
            GetAnimationClocksTargets(clock, containingObject, nameScope, ref targets);
            return targets;
        }

        private void GetAnimationClocksTargets(TimelineClock clock, FrameworkElement containingObject, INameScope nameScope, ref ListDictionary<TargetKey, AnimationTimelineClock> targets)
        {
            AnimationTimelineClock animationClock = clock as AnimationTimelineClock;
            if (animationClock != null)
            {
                TargetKey target = GetClockTarget(animationClock, containingObject, nameScope);
                if (target != null)
                {
                    targets.Add(target, animationClock);
                }
            }

            TimelineGroupClock clockGroup = clock as TimelineGroupClock;
            if (clockGroup != null)
            {
                foreach (TimelineClock child in clockGroup.Children)
                {
                    GetAnimationClocksTargets(child, containingObject, nameScope, ref targets);
                }
            }
        }

        private TargetKey GetClockTarget(AnimationTimelineClock clock, FrameworkElement containingObject, INameScope nameScope)
        {
            DependencyObject root = GetTarget(clock.Timeline);

            if (root == null)
            {
                string targetName = GetTargetName(clock.Timeline);

                if (targetName.IsNullOrEmpty())
                {
                    root = containingObject;
                }
                else
                {
                    root = nameScope.FindName(targetName) as DependencyObject;

                    if (root == null)
                    {
                        throw new Granular.Exception("Can't find Storyboard.TargetName \"{0}\" for element \"{1}\"", targetName, containingObject);
                    }
                }
            }

            PropertyPath propertyPath = GetTargetProperty(clock.Timeline);

            DependencyObject target;
            DependencyProperty targetProperty;
            return TryGetPropertyPathTarget(root, propertyPath, out target, out targetProperty) && target is IAnimatable ? new TargetKey((IAnimatable)target, targetProperty) : null;
        }

        private static bool TryGetPropertyPathTarget(DependencyObject root, PropertyPath propertyPath, out DependencyObject target, out DependencyProperty targetProperty)
        {
            object baseValue;
            target = propertyPath.Elements.Count() > 1 && propertyPath.GetBasePropertyPath().TryGetValue(root, out baseValue) ? baseValue as DependencyObject : root;

            if (target != null && !propertyPath.IsEmpty)
            {
                return propertyPath.Elements.Last().TryGetDependencyProperty(target.GetType(), out targetProperty);
            }

            target = null;
            targetProperty = null;
            return false;
        }
    }
}
