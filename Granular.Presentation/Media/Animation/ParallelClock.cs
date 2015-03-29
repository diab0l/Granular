using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public class ParallelClock : IClock
    {
        public TimeSpan FirstTick { get; private set; }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        private IEnumerable<IClock> clocks;

        public ParallelClock(IEnumerable<IClock> clocks)
        {
            this.clocks = clocks;

            FirstTick = clocks.Select(clock => clock.FirstTick).DefaultIfEmpty(TimeSpan.Zero).Min();
            LastTick = clocks.Select(clock => clock.LastTick).DefaultIfEmpty(TimeSpan.Zero).Max();
            Duration = clocks.Select(clock => clock.Duration).DefaultIfEmpty(TimeSpan.Zero).Max();
        }

        public ClockState Tick(TimeSpan time)
        {
            IEnumerable<ClockState> states = clocks.Select(clock => clock.Tick(time)).ToArray();

            TimeSpan nextTick;
            TimeSpan previousTick;
            ClockProgressState progressState;

            nextTick = states.Select(state => state.NextTick).DefaultIfEmpty(TimeSpan.Zero).Min();
            previousTick = states.Select(state => state.PreviousTick).DefaultIfEmpty(TimeSpan.Zero).Max();

            if (states.All(state => state.ProgressState == ClockProgressState.BeforeStarted))
            {
                progressState = ClockProgressState.BeforeStarted;
            }
            else if (states.All(state => state.ProgressState == ClockProgressState.AfterEnded))
            {
                progressState = ClockProgressState.AfterEnded;
            }
            else
            {
                progressState = ClockProgressState.Active;
            }

            return new ClockState(progressState, 0, 0, previousTick, nextTick);
        }
    }
}
