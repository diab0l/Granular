using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    public class SequentialClock : IClock
    {
        public TimeSpan FirstTick { get; private set; }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        private IEnumerable<IClock> clocks;

        public SequentialClock(IEnumerable<IClock> clocks)
        {
            this.clocks = clocks;

            if (!clocks.Any())
            {
                FirstTick = TimeSpan.Zero;
                LastTick = TimeSpan.Zero;
                Duration = TimeSpan.Zero;
            }
            else
            {
                FirstTick = clocks.First().FirstTick;
                LastTick = clocks.Take(clocks.Count() - 1).Select(clock => clock.Duration).Aggregate((t1, t2) => t1 + t2) + clocks.Last().LastTick;
                Duration = clocks.Select(clock => clock.Duration).Aggregate((t1, t2) => t1 + t2);
            }
        }

        public ClockState Tick(TimeSpan time)
        {
            ClockProgressState progressState;

            if (time < FirstTick)
            {
                progressState = ClockProgressState.BeforeStarted;
            }
            else if (time < LastTick)
            {
                progressState = ClockProgressState.Active;
            }
            else
            {
                progressState = ClockProgressState.AfterEnded;
            }

            TimeSpan previousTick = Granular.Compatibility.TimeSpan.MinValue;
            TimeSpan nextTick = Granular.Compatibility.TimeSpan.MaxValue;
            TimeSpan totalDuration = TimeSpan.Zero;

            foreach (IClock clock in clocks)
            {
                ClockState state = clock.Tick(time - totalDuration);

                if (state.PreviousTick != Granular.Compatibility.TimeSpan.MinValue)
                {
                    previousTick = previousTick.Max(state.PreviousTick + totalDuration);
                }

                if (state.NextTick != Granular.Compatibility.TimeSpan.MaxValue)
                {
                    nextTick = nextTick.Min(state.NextTick + totalDuration);
                }

                totalDuration += clock.Duration;
            }

            return new ClockState(progressState, 0, 0, previousTick, nextTick);
        }
    }
}
