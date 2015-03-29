using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class SequentialClockTest
    {
        [TestMethod]
        public void SequentialClockBasicTest()
        {
            IClock clock = new SequentialClock(new [] {
                new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(6)),
                new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(7)),
                new TestClock(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8)), });

            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(28), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(30), clock.Duration);

            ClockState state = clock.Tick(TimeSpan.Zero);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(6));
            Assert.AreEqual(TimeSpan.FromSeconds(6), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(13), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(17));
            Assert.AreEqual(TimeSpan.FromSeconds(17), state.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(24), state.NextTick);

            state = clock.Tick(TimeSpan.FromSeconds(28));
            Assert.AreEqual(TimeSpan.FromSeconds(28), state.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, state.NextTick);
        }
    }
}
