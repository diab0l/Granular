using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    internal class DoubleAnimationOperations : IAnimationOperations<double?>
    {
        public static readonly DoubleAnimationOperations Default = new DoubleAnimationOperations();

        private DoubleAnimationOperations()
        {
            //
        }

        public double? Zero { get { return 0; } }

        public double? Add(double? value1, double? value2)
        {
            return value1.Value + value2.Value;
        }

        public double? Subtract(double? value1, double? value2)
        {
            return value1.Value - value2.Value;
        }

        public double? Scale(double? value, double factor)
        {
            return factor * value.Value;
        }

        public double? Interpolate(double? value1, double? value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class DoubleAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(double?), typeof(DoubleAnimation), new FrameworkPropertyMetadata());
        public double? To
        {
            get { return (double?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(double?), typeof(DoubleAnimation), new FrameworkPropertyMetadata());
        public double? From
        {
            get { return (double?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(double?), typeof(DoubleAnimation), new FrameworkPropertyMetadata());
        public double? By
        {
            get { return (double?)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(DoubleAnimation), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<double?> animationOperations;
        private bool isAccumulable;

        public DoubleAnimation()
        {
            this.animationOperations = DoubleAnimationOperations.Default;
            this.isAccumulable = true;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            double progress = EasingFunction != null ? EasingFunction.Ease(animationClock.CurrentState.Progress) : animationClock.CurrentState.Progress;

            double? baseValue = animationOperations.Zero;
            double? from;
            double? to;

            if (this.From != null)
            {
                if (this.To != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (double?)defaultOriginValue;
                    }
                    from = this.From;
                    to = this.To;
                }
                else if (this.By != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (double?)defaultOriginValue;
                    }
                    from = this.From;
                    to = animationOperations.Add(this.From, this.By);
                }
                else
                {
                    from = this.From;
                    to = (double?)defaultDestinationValue;
                }
            }
            else if (this.To != null)
            {
                from = (double?)defaultOriginValue;
                to = this.To;
            }
            else if (this.By != null)
            {
                if (isAccumulable)
                {
                    baseValue = (double?)defaultOriginValue;
                }
                from = animationOperations.Zero;
                to = this.By;
            }
            else
            {
                from = (double?)defaultOriginValue;
                to = (double?)defaultDestinationValue;
            }

            if (IsCumulative && isAccumulable)
            {
                baseValue = animationOperations.Add(baseValue, animationOperations.Scale(animationOperations.Subtract(to, from), Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return animationOperations.Add(baseValue, animationOperations.Interpolate(from, to, progress));
        }
    }

    [ContentProperty("KeyFrames")]
    public class DoubleAnimationUsingKeyFrames : AnimationTimeline
    {
        public FreezableCollection<DoubleKeyFrame> KeyFrames { get; private set; }

        private IAnimationOperations<double?> animationOperations;
        private bool isAccumulable;

        public DoubleAnimationUsingKeyFrames()
        {
            this.animationOperations = DoubleAnimationOperations.Default;
            this.isAccumulable = true;

            KeyFrames = new FreezableCollection<DoubleKeyFrame>();
            KeyFrames.SetInheritanceParent(this);
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            if (KeyFrames.Count == 0)
            {
                return defaultDestinationValue;
            }

            TimeSpan duration = Duration.HasTimeSpan ? Duration.TimeSpan : GetKeyFramesDuration();

            TimeSpan time = animationClock.Duration.Scale(animationClock.CurrentState.Progress);

            int index = GetKeyFrameIndexAtTime(time, duration);

            double? value;

            if (index == KeyFrames.Count)
            {
                value = KeyFrames[KeyFrames.Count - 1].Value;
            }
            else
            {
                double? baseValue;

                if (index == 0)
                {
                    baseValue = IsAdditive && isAccumulable ? animationOperations.Zero : (double?)defaultOriginValue;
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
                value = animationOperations.Add(value, (double?)defaultOriginValue);
            }

            if (IsCumulative && isAccumulable)
            {
                value = animationOperations.Add(value, animationOperations.Scale(KeyFrames[KeyFrames.Count - 1].Value, Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return value;
        }

        private TimeSpan GetKeyFrameTime(DoubleKeyFrame keyFrame, TimeSpan keyFramesDuration)
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

    public abstract class DoubleKeyFrame : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double?), typeof(DoubleKeyFrame), new FrameworkPropertyMetadata());
        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(DoubleKeyFrame), new FrameworkPropertyMetadata());
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public abstract double? InterpolateValue(double? baseValue, double keyFrameProgress);
    }

    public class DiscreteDoubleKeyFrame : DoubleKeyFrame
    {
        public override double? InterpolateValue(double? baseValue, double keyFrameProgress)
        {
            return keyFrameProgress < 1 ? baseValue : Value;
        }
    }

    public class LinearDoubleKeyFrame : DoubleKeyFrame
    {
        public override double? InterpolateValue(double? baseValue, double keyFrameProgress)
        {
            return DoubleAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }

    public class EasingDoubleKeyFrame : DoubleKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingDoubleKeyFrame), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public override double? InterpolateValue(double? baseValue, double keyFrameProgress)
        {
            if (EasingFunction != null)
            {
                keyFrameProgress = EasingFunction.Ease(keyFrameProgress);
            }

            return DoubleAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }
}
