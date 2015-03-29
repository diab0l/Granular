using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    [TestClass]
    public class ParallelTimelineClockTest
    {
        [TestMethod]
        public void ParallelClockBasicTest()
        {
            ParallelTimeline timeline = new ParallelTimeline();
            timeline.Children.Add(new TestTimeline());
            timeline.Children.Add(new TestTimeline { BeginTime = TimeSpan.FromSeconds(2) });

            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.Zero, clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(TimeSpan.FromSeconds(0.2), clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(0.2), clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2), clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);

            clock.Tick(TimeSpan.FromSeconds(2.2));
            Assert.AreEqual(TimeSpan.FromSeconds(2.2), clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(2.2), clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);

            clock.Tick(TimeSpan.FromSeconds(3.2));
            Assert.AreEqual(TimeSpan.FromSeconds(3), clock.CurrentState.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
        }
    }
}
