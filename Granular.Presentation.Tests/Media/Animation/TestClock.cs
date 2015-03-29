using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Granular.Extensions;

namespace Granular.Presentation.Media.Animation.Tests
{
    internal class TestClock : IClock
    {
        public TimeSpan FirstTick { get; private set; }
        public TimeSpan LastTick { get; private set; }
        public TimeSpan Duration { get; private set; }

        public TimeSpan PreviousTick { get; private set; }
        public TimeSpan NextTick { get; private set; }

        public ClockProgressState State { get; private set; }
        public double Progress { get; private set; }
        public double Iteration { get { return 0; } }

        public TestClock(TimeSpan duration) :
            this(duration, TimeSpan.Zero, duration)
        {
            //
        }

        public TestClock(TimeSpan duration, TimeSpan firstTick) :
            this(duration, firstTick, duration)
        {
            //
        }

        public TestClock(TimeSpan duration, TimeSpan firstTick, TimeSpan lastTick)
        {
            this.FirstTick = firstTick;
            this.LastTick = lastTick;
            this.Duration = duration;
        }

        public ClockState Tick(TimeSpan time)
        {
            if (time < FirstTick)
            {
                return new ClockState(ClockProgressState.BeforeStarted, 0, 0, Granular.Compatibility.TimeSpan.MinValue, FirstTick);
            }

            if (time < LastTick)
            {
                return new ClockState(ClockProgressState.Active, (time - FirstTick).Divide(LastTick - FirstTick), 0, time, time);
            }

            return new ClockState(ClockProgressState.AfterEnded, 1, 0, LastTick, Granular.Compatibility.TimeSpan.MaxValue);
        }
    }
}
