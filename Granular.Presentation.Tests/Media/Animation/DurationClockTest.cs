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
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
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
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }
    }
}
