using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    internal class RectAnimationOperations : IAnimationOperations<Rect>
    {
        public static readonly RectAnimationOperations Default = new RectAnimationOperations();

        private RectAnimationOperations()
        {
            //
        }

        public Rect Zero { get { return Rect.Zero; } }

        public Rect Add(Rect value1, Rect value2)
        {
            return new Rect(value1.Location + value2.Location, value1.Size + value2.Size);
        }

        public Rect Subtract(Rect value1, Rect value2)
        {
            return new Rect(value1.Location - value2.Location, value1.Size - value2.Size);
        }

        public Rect Scale(Rect value, double factor)
        {
            return new Rect(factor * value.Location, factor * value.Size);
        }

        public Rect Interpolate(Rect value1, Rect value2, double progress)
        {
            return new Rect((1 - progress) * value1.Location + progress * value2.Location, (1 - progress) * value1.Size + progress * value2.Size);
        }
    }

    public class RectAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Rect), typeof(RectAnimation), new FrameworkPropertyMetadata());
        public Rect To
        {
            get { return (Rect)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Rect), typeof(RectAnimation), new FrameworkPropertyMetadata());
        public Rect From
        {
            get { return (Rect)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        public static readonly DependencyProperty ByProperty = DependencyProperty.Register("By", typeof(Rect), typeof(RectAnimation), new FrameworkPropertyMetadata());
        public Rect By
        {
            get { return (Rect)GetValue(ByProperty); }
            set { SetValue(ByProperty, value); }
        }

        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(RectAnimation), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<Rect> animationOperations;
        private bool isAccumulable;

        public RectAnimation()
        {
            this.animationOperations = RectAnimationOperations.Default;
            this.isAccumulable = true;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            double progress = EasingFunction != null ? EasingFunction.Ease(animationClock.CurrentState.Progress) : animationClock.CurrentState.Progress;

            Rect baseValue = animationOperations.Zero;
            Rect from;
            Rect to;

            if (this.From != null)
            {
                if (this.To != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Rect)defaultOriginValue;
                    }
                    from = this.From;
                    to = this.To;
                }
                else if (this.By != null)
                {
                    if (IsAdditive && isAccumulable)
                    {
                        baseValue = (Rect)defaultOriginValue;
                    }
                    from = this.From;
                    to = animationOperations.Add(this.From, this.By);
                }
                else
                {
                    from = this.From;
                    to = (Rect)defaultDestinationValue;
                }
            }
            else if (this.To != null)
            {
                from = (Rect)defaultOriginValue;
                to = this.To;
            }
            else if (this.By != null)
            {
                if (isAccumulable)
                {
                    baseValue = (Rect)defaultOriginValue;
                }
                from = animationOperations.Zero;
                to = this.By;
            }
            else
            {
                from = (Rect)defaultOriginValue;
                to = (Rect)defaultDestinationValue;
            }

            if (IsCumulative && isAccumulable)
            {
                baseValue = animationOperations.Add(baseValue, animationOperations.Scale(animationOperations.Subtract(to, from), Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return animationOperations.Add(baseValue, animationOperations.Interpolate(from, to, progress));
        }
    }

    [ContentProperty("KeyFrames")]
    public class RectAnimationUsingKeyFrames : AnimationTimeline
    {
        public FreezableCollection<RectKeyFrame> KeyFrames { get; private set; }

        private IAnimationOperations<Rect> animationOperations;
        private bool isAccumulable;

        public RectAnimationUsingKeyFrames()
        {
            this.animationOperations = RectAnimationOperations.Default;
            this.isAccumulable = true;

            KeyFrames = new FreezableCollection<RectKeyFrame>();
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

            Rect value;

            if (index == KeyFrames.Count)
            {
                value = KeyFrames[KeyFrames.Count - 1].Value;
            }
            else
            {
                Rect baseValue;

                if (index == 0)
                {
                    baseValue = IsAdditive && isAccumulable ? animationOperations.Zero : (Rect)defaultOriginValue;
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
                value = animationOperations.Add(value, (Rect)defaultOriginValue);
            }

            if (IsCumulative && isAccumulable)
            {
                value = animationOperations.Add(value, animationOperations.Scale(KeyFrames[KeyFrames.Count - 1].Value, Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return value;
        }

        private TimeSpan GetKeyFrameTime(RectKeyFrame keyFrame, TimeSpan keyFramesDuration)
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

    public abstract class RectKeyFrame : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(Rect), typeof(RectKeyFrame), new FrameworkPropertyMetadata());
        public Rect Value
        {
            get { return (Rect)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(RectKeyFrame), new FrameworkPropertyMetadata());
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public abstract Rect InterpolateValue(Rect baseValue, double keyFrameProgress);
    }

    public class DiscreteRectKeyFrame : RectKeyFrame
    {
        public override Rect InterpolateValue(Rect baseValue, double keyFrameProgress)
        {
            return keyFrameProgress < 1 ? baseValue : Value;
        }
    }

    public class LinearRectKeyFrame : RectKeyFrame
    {
        public override Rect InterpolateValue(Rect baseValue, double keyFrameProgress)
        {
            return RectAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }

    public class EasingRectKeyFrame : RectKeyFrame
    {
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingRectKeyFrame), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        public override Rect InterpolateValue(Rect baseValue, double keyFrameProgress)
        {
            if (EasingFunction != null)
            {
                keyFrameProgress = EasingFunction.Ease(keyFrameProgress);
            }

            return RectAnimationOperations.Default.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }
}
