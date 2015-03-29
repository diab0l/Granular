using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public interface IAnimationOperations<T>
    {
        T Zero { get; }
        T Add(T value1, T value2);
        T Subtract(T value1, T value2);
        T Scale(T value, double factor);
        T Interpolate(T value1, T value2, double progress);
    }

    public class TransitionAnimationTimeline<T> : AnimationTimeline
    {
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(T), typeof(TransitionAnimationTimeline<T>), new FrameworkPropertyMetadata());
        public T To
        {
            get { return (T)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(T), typeof(TransitionAnimationTimeline<T>), new FrameworkPropertyMetadata());
        public T From
        {
            get { return (T)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(T), typeof(TransitionAnimationTimeline<T>), new FrameworkPropertyMetadata());
        public T By
        {
            get { return (T)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(TransitionAnimationTimeline<T>), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<T> animationOperations;
        private bool isAccumulable;

        public TransitionAnimationTimeline(IAnimationOperations<T> animationOperations, bool isAccumulable)
        {
            this.animationOperations = animationOperations;
            this.isAccumulable = isAccumulable;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            double progress = EasingFunction != null ? EasingFunction.Ease(animationClock.CurrentState.Progress) : animationClock.CurrentState.Progress;

            T baseValue = animationOperations.Zero;
            T from;
            T to;

            if (this.From != null)
            {
                if (this.To != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (T)defaultOriginValue;
                    }
                    from = this.From;
                    to = this.To;
                }
                else if (this.By != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (T)defaultOriginValue;
                    }
                    from = this.From;
                    to = animationOperations.Add(this.From, this.By);
                }
                else
                {
                    from = this.From;
                    to = (T)defaultDestinationValue;
                }
            }
            else if (this.To != null)
            {
                from = (T)defaultOriginValue;
                to = this.To;
            }
            else if (this.By != null)
            {
                if (isAccumulable)
                {
                    baseValue = (T)defaultOriginValue;
                }
                from = animationOperations.Zero;
                to = this.By;
            }
            else
            {
                from = (T)defaultOriginValue;
                to = (T)defaultDestinationValue;
            }

            if (IsCumulative && isAccumulable)
            {
                baseValue = animationOperations.Add(baseValue, animationOperations.Scale(animationOperations.Subtract(to, from), Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return animationOperations.Add(baseValue, animationOperations.Interpolate(from, to, progress));
        }
    }
}
