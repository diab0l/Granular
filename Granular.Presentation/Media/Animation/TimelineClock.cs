using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media.Animation
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class TimelineClock : IClock
    {
        public event EventHandler Invalidated;

        public Timeline Timeline { get; private set; }

        public TimeSpan FirstTick { get { return beginTime + clock.FirstTick; } }
        public TimeSpan LastTick { get { return clock.LastTick == Granular.Compatibility.TimeSpan.MaxValue ? Granular.Compatibility.TimeSpan.MaxValue : beginTime + clock.LastTick; } }
        public TimeSpan Duration { get { return clock.Duration; } }

        public ClockState CurrentState { get; private set; }

        public bool IsFilling { get { return CurrentState.ProgressState == ClockProgressState.AfterEnded && currentFillBehavior == FillBehavior.HoldEnd; } }

        private IClock clock;
        private TimeSpan beginTime;
        private TimeSpan pauseOffset;
        private IRootClock rootClock;
        private FillBehavior currentFillBehavior;

        public TimelineClock(IClock baseClock, Timeline timeline)
        {
            this.clock = CreateClock(baseClock, timeline);
            this.Timeline = timeline;
            this.CurrentState = ClockState.Empty;
        }

        public ClockState Tick(TimeSpan time)
        {
            ClockState state = clock.Tick(time - beginTime);

            TimeSpan previousTick = state.PreviousTick == Granular.Compatibility.TimeSpan.MinValue ? Granular.Compatibility.TimeSpan.MinValue : beginTime + state.PreviousTick;
            TimeSpan nextTick = state.NextTick == Granular.Compatibility.TimeSpan.MaxValue ? Granular.Compatibility.TimeSpan.MaxValue : beginTime + state.NextTick;

            this.CurrentState = new ClockState(state.ProgressState, state.Progress, state.Iteration, previousTick, nextTick);

            Invalidated.Raise(this);

            return CurrentState;
        }

        private static IClock CreateClock(IClock baseClock, Timeline timeline)
        {
            if (timeline.Duration.HasTimeSpan)
            {
                baseClock = new DurationClock(baseClock, timeline.Duration.TimeSpan);
            }

            if (timeline.AutoReverse)
            {
                baseClock = new ReverseClock(baseClock);
            }

            if (timeline.RepeatBehavior != RepeatBehavior.OneTime)
            {
                double iterationsCount = timeline.RepeatBehavior.Count.DefaultIfNaN((double)timeline.RepeatBehavior.Duration.Ticks / baseClock.Duration.Ticks);
                baseClock = new RepeatClock(baseClock, iterationsCount);
            }

            if (timeline.BeginTime != TimeSpan.Zero)
            {
                baseClock = new OffsetClock(baseClock, timeline.BeginTime);
            }

            return baseClock;
        }

        public void Begin(IRootClock rootClock)
        {
            this.rootClock = rootClock;
            VerifyRootClock();
            beginTime = rootClock.Time;
            currentFillBehavior = Timeline.FillBehavior;
            rootClock.AddClock(this);
            Tick(rootClock.Time);
        }

        public void Pause()
        {
            VerifyRootClock();
            pauseOffset = rootClock.Time - beginTime;
            rootClock.RemoveClock(this);
            Tick(rootClock.Time);
        }

        public void Remove()
        {
            VerifyRootClock();
            rootClock.RemoveClock(this);
        }

        public void Resume()
        {
            VerifyRootClock();
            beginTime = rootClock.Time - pauseOffset;
            rootClock.AddClock(this);
            Tick(rootClock.Time);
        }

        public void Seek(TimeSpan time)
        {
            VerifyRootClock();
            beginTime = rootClock.Time - time;
            Tick(rootClock.Time);
        }

        public void SeekOffset(TimeSpan offset)
        {
            VerifyRootClock();
            beginTime += offset;
            Tick(rootClock.Time);
        }

        public void SkipToFill()
        {
            VerifyRootClock();
            beginTime = rootClock.Time - clock.Duration;
            currentFillBehavior = FillBehavior.HoldEnd;
            Tick(rootClock.Time);
        }

        public void Stop()
        {
            VerifyRootClock();
            beginTime = clock.Duration == Granular.Compatibility.TimeSpan.MaxValue ? TimeSpan.Zero : rootClock.Time - clock.Duration;
            currentFillBehavior = FillBehavior.Stop;
            rootClock.RemoveClock(this);
            Tick(rootClock.Time);
        }

        private void VerifyRootClock()
        {
            if (rootClock == null)
            {
                throw new Granular.Exception("RootClock was not provided");
            }
        }
    }
}
