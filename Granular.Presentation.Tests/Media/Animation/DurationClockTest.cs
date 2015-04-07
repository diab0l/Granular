using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class DurationClockTest
    {
        [TestMethod]
        public void DurationClockBasicTest()
        {
            IClock clock = new DurationClock(new TestClock(TimeSpan.FromSeconds(2)), TimeSpan.FromSeconds(1));
            Assert.AreEqual(TimeSpan.Zero, clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.Zero);
            Assert.AreEqual(TimeSpan.Zero, state.PreviousTick);
            Assert.AreEqual(TimeSpan.Zero, state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(1));
            Assert.AreEqual(TimeSpan.FromSeconds(1), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void DurationOffsetClockTest()
        {
            IClock clock = new DurationClock(new TestClock(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2)), TimeSpan.FromSeconds(1));
            Assert.AreEqual(TimeSpan.Zero, clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.Zero);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(TimeSpan.FromSeconds(1), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void DurationProgressZeroDurationTest()
        {
            IClock clock = new DurationClock(new TestClock(TimeSpan.FromSeconds(0)), TimeSpan.FromSeconds(0));
            Assert.AreEqual(TimeSpan.FromSeconds(0), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(0), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(0), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(-1));
            Assert.AreEqual(ClockProgressState.BeforeStarted, state.ProgressState);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(0), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(ClockProgressState.AfterEnded, state.ProgressState);
            Assert.AreEqual(TimeSpan.FromSeconds(0), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }
    }
}
