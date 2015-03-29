using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    internal class ColorAnimationOperations : IAnimationOperations<Color>
    {
        public static readonly ColorAnimationOperations Default = new ColorAnimationOperations();

        private ColorAnimationOperations()
        {
            //
        }

        public Color Zero { get { return Color.FromUInt32(0); } }

        public Color Add(Color value1, Color value2)
        {
            return value1 + value2;
        }

        public Color Subtract(Color value1, Color value2)
        {
            return value1 - value2;
        }

        public Color Scale(Color value, double factor)
        {
            return factor * value;
        }

        public Color Interpolate(Color value1, Color value2, double progress)
        {
            return (1 - progress) * value1 + progress * value2;
        }
    }

    public class ColorAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Color), typeof(ColorAnimation), new FrameworkPropertyMetadata());
        public Color To
        {
            get { return (Color)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Color), typeof(ColorAnimation), new FrameworkPropertyMetadata());
        public Color From
        {
            get { return (Color)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(Color), typeof(ColorAnimation), new FrameworkPropertyMetadata());
        public Color By
        {
            get { return (Color)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(ColorAnimation), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<Color> animationOperations;
        private bool isAccumulable;

        public ColorAnimation()
        {
            this.animationOperations = ColorAnimationOperations.Default;
            this.isAccumulable = true;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            double progress = EasingFunction != null ? EasingFunction.Ease(animationClock.CurrentState.Progress) : animationClock.CurrentState.Progress;

            Color baseValue = animationOperations.Zero;
            Color from;
            Color to;

            if (this.From != null)
            {
                if (this.To != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Color)defaultOriginValue;
                    }
                    from = this.From;
                    to = this.To;
                }
                else if (this.By != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Color)defaultOriginValue;
                    }
                    from = this.From;
                    to = animationOperations.Add(this.From, this.By);
                }
                else
                {
                    from = this.From;
                    to = (Color)defaultDestinationValue;
                }
            }
            else if (this.To != null)
            {
                from = (Color)defaultOriginValue;
                to = this.To;
            }
            else if (this.By != null)
            {
                if (isAccumulable)
                {
                    baseValue = (Color)defaultOriginValue;
                }
                from = animationOperations.Zero;
                to = this.By;
            }
            else
            {
                from = (Color)defaultOriginValue;
                to = (Color)defaultDestinationValue;
            }

            if (IsCumulative && isAccumulable)
            {
                baseValue = animationOperations.Add(baseValue, animationOperations.Scale(animationOperations.Subtract(to, from), Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return animationOperations.Add(baseValue, animationOperations.Interpolate(from, to, progress));
        }
    }

    [ContentProperty("KeyFrames")]
    public class ColorAnimationUsingKeyFrames : AnimationTimeline
    {
        public FreezableCollection<ColorKeyFrame> KeyFrames { get; private set; }

        private IAnimationOperations<Color> animationOperations;
        private bool isAccumulable;

        public ColorAnimationUsingKeyFrames()
        {
            this.animationOperations = ColorAnimationOperations.Default;
            this.isAccumulable = true;

            KeyFrames = new FreezableCollection<ColorKeyFrame>();
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

            Color value;

            if (index == KeyFrames.Count)
            {
                value = KeyFrames[KeyFrames.Count - 1].Value;
            }
            else
            {
                Color baseValue;

                if (index == 0)
                {
                    baseValue = IsAdditive && isAccumulable ? animationOperations.Zero : (Color)defaultOriginValue;
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
                value = animationOperations.Add(value, (Color)defaultOriginValue);
            }

            if (IsCumulative && isAccumulable)
            {
                value = animationOperations.Add(value, animationOperations.Scale(KeyFrames[KeyFrames.Count - 1].Value, Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return value;
        }

        private TimeSpan GetKeyFrameTime(ColorKeyFrame keyFrame, TimeSpan keyFramesDuration)
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

    public abstract class ColorKeyFrame : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Color), typeof(ColorKeyFrame), new FrameworkPropertyMetadata());
        public Color Value
        {
            get { return (Color)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(ColorKeyFrame), new FrameworkPropertyMetadata());
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public abstract Color InterpolateValue(Color baseValue, double keyFrameProgress);
    }

    public class DiscreteColorKeyFrame : ColorKeyFrame
    {
        public override Color InterpolateValue(Color baseValue, double keyFrameProgress)
        {
            return keyFrameProgress < 1 ? baseValue : Value;
        }
    }

    public class LinearColorKeyFrame : ColorKeyFrame
    {
        public override Color InterpolateValue(Color baseValue, double keyFrameProgress)
        {
            return ColorAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }

    public class EasingColorKeyFrame : ColorKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingColorKeyFrame), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public override Color InterpolateValue(Color baseValue, double keyFrameProgress)
        {
            if (EasingFunction != null)
            {
                keyFrameProgress = EasingFunction.Ease(keyFrameProgress);
            }

            return ColorAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }
}
