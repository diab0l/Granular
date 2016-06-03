using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    internal class ThicknessAnimationOperations : IAnimationOperations<Thickness>
    {
        public static readonly ThicknessAnimationOperations Default = new ThicknessAnimationOperations();

        private ThicknessAnimationOperations()
        {
            //
        }

        public Thickness Zero { get { return Thickness.Zero; } }

        public Thickness Add(Thickness value1, Thickness value2)
        {
            return value1 + value2;
        }

        public Thickness Subtract(Thickness value1, Thickness value2)
        {
            return value1 - value2;
        }

        public Thickness Scale(Thickness value, double factor)
        {
            return factor * value;
        }

        public Thickness Interpolate(Thickness value1, Thickness value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class ThicknessAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Thickness), typeof(ThicknessAnimation), new FrameworkPropertyMetadata());
        public Thickness To
        {
            get { return (Thickness)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Thickness), typeof(ThicknessAnimation), new FrameworkPropertyMetadata());
        public Thickness From
        {
            get { return (Thickness)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(Thickness), typeof(ThicknessAnimation), new FrameworkPropertyMetadata());
        public Thickness By
        {
            get { return (Thickness)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(ThicknessAnimation), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<Thickness> animationOperations;
        private bool isAccumulable;

        public ThicknessAnimation()
        {
            this.animationOperations = ThicknessAnimationOperations.Default;
            this.isAccumulable = true;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            double progress = EasingFunction != null ? EasingFunction.Ease(animationClock.CurrentState.Progress) : animationClock.CurrentState.Progress;

            Thickness baseValue = animationOperations.Zero;
            Thickness from;
            Thickness to;

            if (this.From != null)
            {
                if (this.To != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Thickness)defaultOriginValue;
                    }
                    from = this.From;
                    to = this.To;
                }
                else if (this.By != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Thickness)defaultOriginValue;
                    }
                    from = this.From;
                    to = animationOperations.Add(this.From, this.By);
                }
                else
                {
                    from = this.From;
                    to = (Thickness)defaultDestinationValue;
                }
            }
            else if (this.To != null)
            {
                from = (Thickness)defaultOriginValue;
                to = this.To;
            }
            else if (this.By != null)
            {
                if (isAccumulable)
                {
                    baseValue = (Thickness)defaultOriginValue;
                }
                from = animationOperations.Zero;
                to = this.By;
            }
            else
            {
                from = (Thickness)defaultOriginValue;
                to = (Thickness)defaultDestinationValue;
            }

            if (IsCumulative && isAccumulable)
            {
                baseValue = animationOperations.Add(baseValue, animationOperations.Scale(animationOperations.Subtract(to, from), Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return animationOperations.Add(baseValue, animationOperations.Interpolate(from, to, progress));
        }
    }

    [ContentProperty("KeyFrames")]
    public class ThicknessAnimationUsingKeyFrames : AnimationTimeline
    {
        public FreezableCollection<ThicknessKeyFrame> KeyFrames { get; private set; }

        private IAnimationOperations<Thickness> animationOperations;
        private bool isAccumulable;

        public ThicknessAnimationUsingKeyFrames()
        {
            this.animationOperations = ThicknessAnimationOperations.Default;
            this.isAccumulable = true;

            KeyFrames = new FreezableCollection<ThicknessKeyFrame>();
            KeyFrames.TrySetContextParent(this);
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            if (KeyFrames.Count == 0)
            {
                return defaultDestinationValue;
            }

            TimeSpan duration = Duration.HasTimeSpan ? Duration.TimeSpan : GetKeyFramesDuration();

            TimeSpan time = duration.Scale(animationClock.CurrentState.Progress);

            int index = GetKeyFrameIndexAtTime(time, duration);

            Thickness value;

            if (index == KeyFrames.Count)
            {
                value = KeyFrames[KeyFrames.Count - 1].Value;
            }
            else
            {
                Thickness baseValue;

                if (index == 0)
                {
                    baseValue = IsAdditive && isAccumulable ? animationOperations.Zero : (Thickness)defaultOriginValue;
                }
                else
                {
                    baseValue = KeyFrames[index - 1].Value;
                }

                TimeSpan segmentStart = index == 0 ? TimeSpan.Zero : GetKeyFrameTime(KeyFrames[index - 1], duration);
                TimeSpan segmentEnd = GetKeyFrameTime(KeyFrames[index], duration);

                double progress = segmentEnd == segmentStart ? 1 : (double)(time - segmentStart).Ticks / (segmentEnd - segmentStart).Ticks;

                value = KeyFrames[index].InterpolateValue(baseValue, progress);
            }

            if (IsAdditive && isAccumulable)
            {
                value = animationOperations.Add(value, (Thickness)defaultOriginValue);
            }

            if (IsCumulative && isAccumulable)
            {
                value = animationOperations.Add(value, animationOperations.Scale(KeyFrames[KeyFrames.Count - 1].Value, Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return value;
        }

        private TimeSpan GetKeyFrameTime(ThicknessKeyFrame keyFrame, TimeSpan keyFramesDuration)
        {
            if (keyFrame.KeyTime.HasTimeSpan)
            {
                return keyFrame.KeyTime.TimeSpan;
            }

            if (keyFrame.KeyTime.HasPercent)
            {
                return keyFramesDuration.Scale(keyFrame.KeyTime.Percent);
            }

            throw new Granular.Exception("KeyTime of type \"{0}\" is not supported", keyFrame.KeyTime.Type);
        }

        private int GetKeyFrameIndexAtTime(TimeSpan time, TimeSpan keyFramesDuration)
        {
            return KeyFrames.IndexOf(KeyFrames.LastOrDefault(keyFrame => GetKeyFrameTime(keyFrame, keyFramesDuration) < time)) + 1;
        }

        private TimeSpan GetKeyFramesDuration()
        {
            return KeyFrames.Where(keyFrame => keyFrame.KeyTime.HasTimeSpan).Select(keyFrame => keyFrame.KeyTime.TimeSpan).DefaultIfEmpty(TimeSpan.FromSeconds(1)).Max();
        }
    }

    public abstract class ThicknessKeyFrame : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Thickness), typeof(ThicknessKeyFrame), new FrameworkPropertyMetadata());
        public Thickness Value
        {
            get { return (Thickness)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ThicknessKeyFrame), new FrameworkPropertyMetadata());
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public abstract Thickness InterpolateValue(Thickness baseValue, double keyFrameProgress);
    }

    public class DiscreteThicknessKeyFrame : ThicknessKeyFrame
    {
        public override Thickness InterpolateValue(Thickness baseValue, double keyFrameProgress)
        {
            return keyFrameProgress < 1 ? baseValue : Value;
        }
    }

    public class LinearThicknessKeyFrame : ThicknessKeyFrame
    {
        public override Thickness InterpolateValue(Thickness baseValue, double keyFrameProgress)
        {
            return ThicknessAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }

    public class EasingThicknessKeyFrame : ThicknessKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingThicknessKeyFrame), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public override Thickness InterpolateValue(Thickness baseValue, double keyFrameProgress)
        {
            if (EasingFunction != null)
            {
                keyFrameProgress = EasingFunction.Ease(keyFrameProgress);
            }

            return ThicknessAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }
}
