using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    // reverse the time after inner clock's duration
    public class ReverseClock : IClock
    {
        public TimeSpan FirstTick { get { return clock.FirstTick; } }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        private IClock clock;

        public ReverseClock(IClock clock)
        {
            this.clock = clock;

            this.Duration = clock.Duration.Scale(2);
            this.LastTick = Duration - clock.FirstTick;
        }

        public ClockState Tick(TimeSpan time)
        {
            bool isReversed = time > clock.Duration;

            TimeSpan reversedTime = Duration - time;

            TimeSpan previousTick;
            TimeSpan nextTick;

            ClockState state;

            if (!isReversed)
            {
                state = clock.Tick(time);

                previousTick = state.PreviousTick;
                nextTick = time >= clock.LastTick ? Duration - clock.LastTick : state.NextTick;
            }
            else
            {
                state = clock.Tick(reversedTime);

                previousTick = reversedTime >= clock.LastTick ? clock.LastTick : Duration - state.NextTick;
                nextTick = state.PreviousTick == Granular.Compatibility.TimeSpan.MinValue ? Granular.Compatibility.TimeSpan.MaxValue : Duration - state.PreviousTick;
            }

            return new ClockState(state.ProgressState, state.Progress, state.Iteration, previousTick, nextTick);
        }
    }
}
