using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    [ContentProperty("KeyFrames")]
    public class KeyFramesAnimationTimeline<T> : AnimationTimeline
    {
        public FreezableCollection<KeyFrame<T>> KeyFrames { get; private set; }

        private IAnimationOperations<T> animationOperations;
        private bool isAccumulable;

        public KeyFramesAnimationTimeline(IAnimationOperations<T> animationOperations, bool isAccumulable)
        {
            this.animationOperations = animationOperations;
            this.isAccumulable = isAccumulable;

            KeyFrames = new FreezableCollection<KeyFrame<T>>();
            KeyFrames.SetInheritanceParent(this);
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

            T value;

            if (index == KeyFrames.Count)
            {
                value = KeyFrames[KeyFrames.Count - 1].Value;
            }
            else
            {
                T baseValue;

                if (index == 0)
                {
                    baseValue = IsAdditive && isAccumulable ? animationOperations.Zero : (T)defaultOriginValue;
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
                value = animationOperations.Add(value, (T)defaultOriginValue);
            }

            if (IsCumulative && isAccumulable)
            {
                value = animationOperations.Add(value, animationOperations.Scale(KeyFrames[KeyFrames.Count - 1].Value, Math.Floor(animationClock.CurrentState.Iteration)));
            }

            return value;
        }

        private TimeSpan GetKeyFrameTime(KeyFrame<T> keyFrame, TimeSpan keyFramesDuration)
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
}
