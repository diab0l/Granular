using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    // set time interval to [0, duration]
    public class DurationClock : IClock
    {
        public TimeSpan FirstTick { get; private set; }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        private IClock clock;

        public DurationClock(IClock clock, TimeSpan duration)
        {
            this.clock = clock;
            this.Duration = duration;

            SetTickBounds();
        }

        private void SetTickBounds()
        {
            if (Duration < clock.FirstTick)
            {
                this.FirstTick = TimeSpan.Zero;
                this.LastTick = Duration;
            }
            else
            {
                this.FirstTick = clock.FirstTick;
                this.LastTick = Duration.Min(clock.LastTick);
            }
        }

        public ClockState Tick(TimeSpan time)
        {
            if (time < TimeSpan.Zero)
            {
                ClockState state = clock.Tick(TimeSpan.Zero);

                TimeSpan previousTick = Granular.Compatibility.TimeSpan.MinValue;
                TimeSpan nextTick = TimeSpan.Zero;

                return new ClockState(ClockProgressState.BeforeStarted, state.Progress, state.Iteration, previousTick, nextTick);
            }

            if (time >= Duration)
            {
                ClockState state = clock.Tick(Duration);

                TimeSpan previousTick = Duration;
                TimeSpan nextTick = Granular.Compatibility.TimeSpan.MaxValue;

                return new ClockState(ClockProgressState.AfterEnded, state.Progress, state.Iteration, previousTick, nextTick);
            }

            return clock.Tick(time);
        }
    }
}
