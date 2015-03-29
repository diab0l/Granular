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
            ClockProgressState progressState = ClockProgressState.Active;

            if (time < TimeSpan.Zero)
            {
                time = TimeSpan.Zero;
                progressState = ClockProgressState.BeforeStarted;
            }

            if (time >= Duration)
            {
                time = Duration;
                progressState = ClockProgressState.AfterEnded;
            }

            ClockState state = clock.Tick(time);

            TimeSpan previousTick = state.PreviousTick > TimeSpan.Zero ? state.PreviousTick : Granular.Compatibility.TimeSpan.MinValue;
            TimeSpan nextTick = state.NextTick < Duration ? state.NextTick : Granular.Compatibility.TimeSpan.MaxValue;

            if (progressState == ClockProgressState.Active)
            {
                progressState = state.ProgressState;
            }

            return new ClockState(progressState, state.Progress, state.Iteration, previousTick, nextTick);
        }
    }
}
