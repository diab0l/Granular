using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static readonly RootClock Default = new RootClock();

        public TimeSpan Time { get { return Granular.Compatibility.TimeSpan.Subtract(DateTime.Now, startTime); } }

        private List<IClock> clocks;
        private DateTime startTime;

        private RootClock()
        {
            clocks = new List<IClock>();

            startTime = DateTime.Now;
        }

        public void AddClock(IClock clock)
        {
            if (clocks.Contains(clock))
            {
                return;
            }

            clocks.Add(clock);

            Tick();
        }

        public void RemoveClock(IClock clock)
        {
            clocks.Remove(clock);
        }

        public void Tick()
        {
            TimeSpan tickTime = Time;

            IEnumerable<ClockState> states = clocks.Select(clock => clock.Tick(tickTime)).ToArray();
            TimeSpan nextTick = states.Select(state => state.NextTick).DefaultIfEmpty(Granular.Compatibility.TimeSpan.MaxValue).Min();

            if (nextTick < Granular.Compatibility.TimeSpan.MaxValue)
            {
                ApplicationHost.Current.TaskScheduler.ScheduleTask(TimeSpan.FromMilliseconds(Math.Max((nextTick - Time).TotalMilliseconds, 25)), Tick);
            }

            CleanClocks();
        }

        private void CleanClocks()
        {
            TimeSpan time = Time;

            int i = 0;
            while (i < clocks.Count)
            {
                if (clocks[i].LastTick < time)
                {
                    clocks.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
