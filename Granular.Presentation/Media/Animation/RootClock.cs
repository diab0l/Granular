using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;
using System.Windows.Threading;

namespace System.Windows.Media.Animation
{
    public interface IRootClock
    {
        TimeSpan Time { get; }
        void AddClock(IClock clock);
        void RemoveClock(IClock clock);
    }

    public class RootClock : IRootClock
    {
        private class ClockSchedule
        {
            public IClock Clock { get; private set; }
            public TimeSpan NextTick { get; private set; }

            public ClockSchedule(IClock clock)
            {
                this.Clock = clock;
                this.NextTick = clock.FirstTick;
            }

            public void Tick(TimeSpan time)
            {
                ClockState state = Clock.Tick(time);
                NextTick = state.NextTick;
            }
        }

        private static readonly TimeSpan TickFrequency = TimeSpan.FromMilliseconds(20);

        public static readonly RootClock Default = new RootClock();

        public TimeSpan Time { get { return Granular.Compatibility.TimeSpan.Subtract(DateTime.Now, startTime); } }

        private DateTime startTime;

        private IDisposable scheduledTick;
        private TimeSpan scheduledTickTime;
        private TimeSpan lastTickTime;

        private List<ClockSchedule> clocksSchedule;

        private RootClock()
        {
            clocksSchedule = new List<ClockSchedule>();

            startTime = DateTime.Now;
            lastTickTime = Granular.Compatibility.TimeSpan.MinValue;
        }

        public void AddClock(IClock clock)
        {
            if (clocksSchedule.Any(clockSchedule => clockSchedule.Clock == clock) || clock.FirstTick == Granular.Compatibility.TimeSpan.MaxValue)
            {
                return;
            }

            clocksSchedule.Add(new ClockSchedule(clock));

            ScheduleTick(clock.FirstTick);
        }

        private void ScheduleTick(TimeSpan tickTime)
        {
            // keep TickFrequency interval between ticks
            tickTime = tickTime.Max(lastTickTime + TickFrequency);

            if (scheduledTick != null)
            {
                if (scheduledTickTime <= tickTime)
                {
                    // earlier tick is already scheduled
                    return;
                }

                scheduledTick.Dispose();
            }

            scheduledTick = ApplicationHost.Current.TaskScheduler.ScheduleTask((tickTime - Time).Max(TimeSpan.Zero), () => Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, () => Tick()));
            scheduledTickTime = tickTime;
        }

        public void RemoveClock(IClock clock)
        {
            clocksSchedule.Remove(clocksSchedule.FirstOrDefault(clockSchedule => clockSchedule.Clock == clock));
        }

        public void Tick()
        {
            TimeSpan tickTime = Time;

            lastTickTime = tickTime;
            scheduledTick = null;

            TimeSpan nextTick = Granular.Compatibility.TimeSpan.MaxValue;

            foreach (ClockSchedule clockSchedule in clocksSchedule)
            {
                if (clockSchedule.NextTick <= tickTime)
                {
                    clockSchedule.Tick(tickTime);
                    nextTick = nextTick.Min(clockSchedule.NextTick);
                }
            }

            CleanClocks();

            if (nextTick < Granular.Compatibility.TimeSpan.MaxValue)
            {
                ScheduleTick(nextTick);
            }
        }

        private void CleanClocks()
        {
            int i = 0;
            while (i < clocksSchedule.Count)
            {
                if (clocksSchedule[i].NextTick == Granular.Compatibility.TimeSpan.MaxValue)
                {
                    clocksSchedule.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
