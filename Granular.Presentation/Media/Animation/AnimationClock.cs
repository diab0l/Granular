using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public class AnimationClock : IClock
    {
        public TimeSpan FirstTick { get { return TimeSpan.Zero; } }
        public TimeSpan LastTick { get { return Duration; } }
        public TimeSpan Duration { get; private set; }

        public AnimationClock(TimeSpan duration)
        {
            this.Duration = duration;
        }

        public ClockState Tick(TimeSpan time)
        {
            TimeSpan previousTick;
            TimeSpan nextTick;
            ClockProgressState progressState;
            double progress;

            if (time < TimeSpan.Zero)
            {
                previousTick = Granular.Compatibility.TimeSpan.MinValue;
                nextTick = TimeSpan.Zero;

                progress = 0;
                progressState = ClockProgressState.BeforeStarted;
            }
            else if (time < Duration)
            {
                previousTick = time;
                nextTick = time;

                progress = time.Divide(Duration);
                progressState = ClockProgressState.Active;
            }
            else
            {
                previousTick = Duration;
                nextTick = Granular.Compatibility.TimeSpan.MaxValue;

                progress = 1;
                progressState = ClockProgressState.AfterEnded;
            }

            return new ClockState(progressState, progress, 0, previousTick, nextTick);
        }
    }
}
