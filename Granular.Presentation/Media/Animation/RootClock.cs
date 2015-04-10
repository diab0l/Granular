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
        private IDisposable scheduledTick;

        private int ticks;
        private TimeSpan lastReport;

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

            IEnumerable<ClockState> states = clocks.ToArray().Select(clock => clock.Tick(tickTime)).ToArray();
            TimeSpan nextTick = states.Select(state => state.NextTick).DefaultIfEmpty(Granular.Compatibility.TimeSpan.MaxValue).Min();

            if (scheduledTick != null)
            {
                scheduledTick.Dispose();
                scheduledTick = null;
            }

            if (tickTime < Granular.Compatibility.TimeSpan.MaxValue)
            {
                scheduledTick = ApplicationHost.Current.TaskScheduler.ScheduleTask(TimeSpan.FromMilliseconds(Math.Max((nextTick - Time).TotalMilliseconds, 25)), Tick);
            }

            ticks++;
            if (tickTime - lastReport >= TimeSpan.FromSeconds(1))
            {
                lastReport = tickTime;
                Console.WriteLine(String.Format("Total ticks: {0}, rate: {1} ticks/sec", ticks, Math.Round(ticks / tickTime.TotalSeconds, 2)));
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
