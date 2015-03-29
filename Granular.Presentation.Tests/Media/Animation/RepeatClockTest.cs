using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class RepeatClockTest
    {
        [TestMethod]
        public void RepeatClockCutBeforeStartTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 0.1);
            Assert.AreEqual(TimeSpan.Zero, clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(-1));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.BeforeStarted, state.ProgressState);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.Zero, state.NextTick);

            state = clock.Tick(TimeSpan.Zero);
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.BeforeStarted, state.ProgressState);
            Assert.AreEqual(TimeSpan.Zero, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(1), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void RepeatClockCutActiveTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 0.3);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(3), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(3), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(-1));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.BeforeStarted, state.ProgressState);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.Active, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(3));
            Assert.AreEqual(0.5, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(3), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void RepeatClockCutAfterEndTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 0.5);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(4), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(5), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(-1));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.BeforeStarted, state.ProgressState);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.Active, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(4));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(5));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void RepeatClockSecondIterationCutBeforeStartTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 1.1);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(4), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(11), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(9));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(11));
            Assert.AreEqual(0, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void RepeatClockSecondIterationCutActiveTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 1.3);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(13), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(13), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(9));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(12), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(12.5));
            Assert.AreEqual(0.25, state.Progress);
            Assert.AreEqual(ClockProgressState.Active, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(12.5), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(12.5), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(13));
            Assert.AreEqual(0.5, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(13), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void RepeatClockSecondIterationCutAfterEndTest()
        {
            IClock clock = new RepeatClock(new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)), 1.5);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(14), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(15), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(9));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(12), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(13));
            Assert.AreEqual(0.5, state.Progress);
            Assert.AreEqual(ClockProgressState.Active, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(13), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(13), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(15));
            Assert.AreEqual(1, state.Progress);
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(14), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }
    }
}
