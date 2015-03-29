using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Granular.Presentation.Media.Animation.Tests
{
    public class TestTimeline : AnimationTimeline
    {
        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationTimelineClock animationClock)
        {
            return animationClock.CurrentState.Progress;
        }
    }

    [TestClass]
    public class TimelineClockTest
    {
        [TestMethod]
        public void TimelineClockBasicTest()
        {
            TestTimeline timeline = new TestTimeline();
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MinValue, clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.Zero, clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(TimeSpan.FromSeconds(0.2), clock.CurrentState.PreviousTick);
            Assert.AreEqual(TimeSpan.FromSeconds(0.2), clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(TimeSpan.FromSeconds(1), clock.CurrentState.PreviousTick);
            Assert.AreEqual(Granular.Compatibility.TimeSpan.MaxValue, clock.CurrentState.NextTick);
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(1, clock.CurrentState.Progress);
        }

        [TestMethod]
        public void TimelineClockReverseTest()
        {
            TestTimeline timeline = new TestTimeline { AutoReverse = true };
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
            Assert.AreEqual(0, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.8, clock.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(1.8));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);

            clock.Tick(TimeSpan.FromSeconds(2.2));
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
        }

        [TestMethod]
        public void TimelineClockRepeatCountTest()
        {
            TestTimeline timeline = new TestTimeline { RepeatBehavior = RepeatBehavior.FromRepeatCount(2.4) };
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
            Assert.AreEqual(0, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);
            Assert.AreEqual(0.2, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(1.2, Math.Round(clock.CurrentState.Iteration, 2));

            clock.Tick(TimeSpan.FromSeconds(2.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);
            Assert.AreEqual(2.2, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(2.6));
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.4, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(2.4, Math.Round(clock.CurrentState.Iteration, 2));
        }

        [TestMethod]
        public void TimelineClockRepeatTimeSpanTest()
        {
            TestTimeline timeline = new TestTimeline { RepeatBehavior = RepeatBehavior.FromTimeSpan(TimeSpan.FromSeconds(2.4)) };
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
            Assert.AreEqual(0, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);
            Assert.AreEqual(0.2, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(1.2, Math.Round(clock.CurrentState.Iteration, 2));

            clock.Tick(TimeSpan.FromSeconds(2.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);
            Assert.AreEqual(2.2, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(2.6));
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.4, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(2.4, Math.Round(clock.CurrentState.Iteration, 2));
        }

        [TestMethod]
        public void TimelineClockReverseRepeatTest()
        {
            TestTimeline timeline = new TestTimeline { RepeatBehavior = RepeatBehavior.FromRepeatCount(1.2), AutoReverse = true };
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Tick(TimeSpan.FromSeconds(-0.2));
            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
            Assert.AreEqual(0, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);
            Assert.AreEqual(0.1, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(1.2));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.8, clock.CurrentState.Progress);
            Assert.AreEqual(0.6, clock.CurrentState.Iteration);

            clock.Tick(TimeSpan.FromSeconds(2.4));
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.4, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(1.2, Math.Round(clock.CurrentState.Iteration, 2));

            clock.Tick(TimeSpan.FromSeconds(2.6));
            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.4, Math.Round(clock.CurrentState.Progress, 2));
            Assert.AreEqual(1.2, Math.Round(clock.CurrentState.Iteration, 2));
        }

        [TestMethod]
        public void TimelineClockControlBasicTest()
        {
            TestRootClock rootClock = new TestRootClock();
            TestTimeline timeline = new TestTimeline();
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            rootClock.Tick(TimeSpan.FromSeconds(10));

            Assert.AreEqual(ClockProgressState.BeforeStarted, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);
            Assert.AreEqual(0, clock.CurrentState.Iteration);

            clock.Begin(rootClock);

            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0, clock.CurrentState.Progress);

            rootClock.Tick(TimeSpan.FromSeconds(10.1));

            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.1, clock.CurrentState.Progress);

            clock.Pause();
            rootClock.Tick(TimeSpan.FromSeconds(10.2));

            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.1, clock.CurrentState.Progress);

            clock.Resume();
            rootClock.Tick(TimeSpan.FromSeconds(10.3));

            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.2, clock.CurrentState.Progress);

            clock.Stop();
            rootClock.Tick(TimeSpan.FromSeconds(10.4));

            Assert.AreEqual(ClockProgressState.AfterEnded, clock.CurrentState.ProgressState);
            Assert.AreEqual(1, clock.CurrentState.Progress);
        }

        [TestMethod]
        public void TimelineClockControlSeekTest()
        {
            TestRootClock rootClock = new TestRootClock();
            TestTimeline timeline = new TestTimeline();
            TimelineClock clock = (TimelineClock)timeline.CreateClock();

            clock.Begin(rootClock);
            rootClock.Tick(TimeSpan.FromSeconds(0.1));
            Assert.AreEqual(ClockProgressState.Active, clock.CurrentState.ProgressState);
            Assert.AreEqual(0.1, clock.CurrentState.Progress);

            clock.SeekOffset(TimeSpan.FromSeconds(0.1));
            rootClock.Tick(TimeSpan.FromSeconds(0.2));
            Assert.AreEqual(0.1, clock.CurrentState.Progress);

            clock.Seek(TimeSpan.FromSeconds(0.9));
            rootClock.Tick(TimeSpan.FromSeconds(0.3));
            Assert.AreEqual(1, clock.CurrentState.Progress);
        }
    }
}
