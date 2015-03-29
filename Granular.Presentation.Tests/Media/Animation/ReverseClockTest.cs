using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class ReverseClockTest
    {
        [TestMethod]
        public void ReverseClockBasicTest()
        {
            IClock clock = new ReverseClock(new TestClock(TimeSpan.FromSeconds(2)));
            Assert.AreEqual(TimeSpan.Zero, clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(4), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(4), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(-1));
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.Zero, state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(5));
            Assert.AreEqual(TimeSpan.FromSeconds(4), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }

        [TestMethod]
        public void ReverseOffsetClockTest()
        {
            IClock clock = new ReverseClock(new TestClock(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(7), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(8), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.FromSeconds(0));
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(1), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(2));
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(6), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(4));
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(6), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(6));
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(6), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(8));
            Assert.AreEqual(TimeSpan.FromSeconds(7), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }
    }
}
