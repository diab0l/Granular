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
        public void SequentialClockTicksTest()
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

        [TestMethod]
        public void SequentialClockProgressTest()
        {
            TestClock clock1 = new TestClock(TimeSpan.FromSeconds(1));
            TestClock clock2 = new TestClock(TimeSpan.FromSeconds(0));
            TestClock clock3 = new TestClock(TimeSpan.FromSeconds(2));

            IClock clock = new SequentialClock(new[] { clock1, clock2, clock3 });

            Assert.AreEqual(TimeSpan.FromSeconds(0), clock.FirstTick);
            Assert.AreEqual(TimeSpan.FromSeconds(3), clock.LastTick);
            Assert.AreEqual(TimeSpan.FromSeconds(3), clock.Duration);

            clock.Tick(TimeSpan.FromSeconds(-1));

            Assert.AreEqual(ClockProgressState.BeforeStarted, clock1.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock2.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock3.CurrentState.ProgressState);
            Assert.AreEqual(0, clock1.CurrentState.Progress);
            Assert.AreEqual(0, clock2.CurrentState.Progress);
            Assert.AreEqual(0, clock3.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(0));

            Assert.AreEqual(ClockProgressState.Active, clock1.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock2.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock3.CurrentState.ProgressState);
            Assert.AreEqual(0, clock1.CurrentState.Progress);
            Assert.AreEqual(0, clock2.CurrentState.Progress);
            Assert.AreEqual(0, clock3.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(1));

            Assert.AreEqual(ClockProgressState.AfterEnded, clock1.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.AfterEnded, clock2.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.Active, clock3.CurrentState.ProgressState);
            Assert.AreEqual(1, clock1.CurrentState.Progress);
            Assert.AreEqual(1, clock2.CurrentState.Progress);
            Assert.AreEqual(0, clock3.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(3));

            Assert.AreEqual(ClockProgressState.AfterEnded, clock1.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.AfterEnded, clock2.CurrentState.ProgressState);
            Assert.AreEqual(ClockProgressState.AfterEnded, clock3.CurrentState.ProgressState);
            Assert.AreEqual(1, clock1.CurrentState.Progress);
            Assert.AreEqual(1, clock2.CurrentState.Progress);
            Assert.AreEqual(1, clock3.CurrentState.Progress);
        }
    }
}
