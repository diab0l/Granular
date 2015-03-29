using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class AnimationTimelineClock : TimelineClock
    {
        private AnimationTimeline timeline;

        public AnimationTimelineClock(AnimationTimeline timeline) :
            base(CreateAnimationClock(timeline), timeline)
        {
            this.timeline = timeline;
        }

        private static IClock CreateAnimationClock(AnimationTimeline timeline)
        {
            if (!timeline.Duration.HasTimeSpan)
            {
                throw new Granular.Exception("Can't resolve animation duration \"{0}\"", timeline.Duration);
            }

            return new AnimationClock(timeline.Duration.TimeSpan);
        }

        public object GetValue(object defaultOriginValue, object defaultDestinationValue)
        {
            return timeline.GetCurrentValue(defaultOriginValue, defaultDestinationValue, this);
        }
    }
}
