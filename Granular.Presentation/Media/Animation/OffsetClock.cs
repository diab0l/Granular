using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    // shift time by an offset
    public class OffsetClock : IClock
    {
        public TimeSpan FirstTick { get { return offset + clock.FirstTick; } }
        public TimeSpan LastTick { get { return clock.LastTick == Granular.Compatibility.TimeSpan.MaxValue ? Granular.Compatibility.TimeSpan.MaxValue : offset + clock.LastTick; } }
        public TimeSpan Duration { get { return clock.Duration == Granular.Compatibility.TimeSpan.MaxValue ? Granular.Compatibility.TimeSpan.MaxValue : offset + clock.Duration; } }

        private IClock clock;
        private TimeSpan offset;

        public OffsetClock(IClock clock, TimeSpan offset)
        {
            this.clock = clock;
            this.offset = offset;
        }

        public ClockState Tick(TimeSpan time)
        {
            ClockState state = clock.Tick(time - offset);

            TimeSpan previousTick = state.PreviousTick == Granular.Compatibility.TimeSpan.MinValue ? Granular.Compatibility.TimeSpan.MinValue : offset + state.PreviousTick;
            TimeSpan nextTick = state.NextTick == Granular.Compatibility.TimeSpan.MaxValue ? Granular.Compatibility.TimeSpan.MaxValue : offset + state.NextTick;

            return new ClockState(state.ProgressState, state.Progress, state.Iteration, previousTick, nextTick);
        }
    }
}
