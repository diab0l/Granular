using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public interface IAnimatable
    {
        IRootClock RootClock { get; }
        void SetAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner);
        void AddAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner);
        void RemoveAnimationClocks(DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, object layerOwner);
    }

    public class Animatable : Freezable, IAnimatable
    {
        public IRootClock RootClock { get; private set; }

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

        protected override void OnInheritanceParentChanged(DependencyObject oldInheritanceParent, DependencyObject newInheritanceParent)
        {
            base.OnInheritanceParentChanged(oldInheritanceParent, newInheritanceParent);

            RootClock = newInheritanceParent is IAnimatable ? ((IAnimatable)newInheritanceParent).RootClock : null;
        }
    }

    public static class AnimatableExtensions
    {
        public static void ClearAnimationClocks(this IAnimatable animatable, DependencyProperty dependencyProperty, object layerOwner = null)
        {
            animatable.SetAnimationClocks(dependencyProperty, new AnimationTimelineClock[0], layerOwner);
        }

        public static void ApplyAnimationClock(this IAnimatable animatable, DependencyProperty dependencyProperty, AnimationTimelineClock animationClock, HandoffBehavior handoffBehavior = HandoffBehavior.SnapshotAndReplace, object layerOwner = null)
        {
            IEnumerable<AnimationTimelineClock> animationClocks = animationClock != null ? new[] { animationClock } : new AnimationTimelineClock[0];
            animatable.ApplyAnimationClocks(dependencyProperty, animationClocks, handoffBehavior, layerOwner);
        }

        public static void ApplyAnimationClocks(this IAnimatable animatable, DependencyProperty dependencyProperty, IEnumerable<AnimationTimelineClock> animationClocks, HandoffBehavior handoffBehavior = HandoffBehavior.SnapshotAndReplace, object layerOwner = null)
        {
            if (handoffBehavior == HandoffBehavior.SnapshotAndReplace)
            {
                animatable.SetAnimationClocks(dependencyProperty, animationClocks, layerOwner);
            }
            else
            {
                animatable.AddAnimationClocks(dependencyProperty, animationClocks, layerOwner);
            }
        }

        public static void BeginAnimation(this IAnimatable animatable, DependencyProperty dependencyProperty, AnimationTimeline animation, HandoffBehavior handoffBehavior = HandoffBehavior.SnapshotAndReplace, object layerOwner = null)
        {
            AnimationTimelineClock animationClock = (AnimationTimelineClock)animation.CreateClock();
            animatable.ApplyAnimationClock(dependencyProperty, animationClock, handoffBehavior, layerOwner);
            animationClock.Begin(animatable.RootClock);
        }
    }
}
