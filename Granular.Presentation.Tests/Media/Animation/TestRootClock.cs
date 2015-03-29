using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace Granular.Presentation.Media.Animation.Tests
{
    public class TestRootClock : IRootClock
    {
        public TimeSpan Time { get; private set; }

        private List<IClock> clocks;
        public IEnumerable<IClock> Clocks { get; private set; }

        public TestRootClock()
        {
            clocks = new List<IClock>();
            Clocks = new ReadOnlyCollection<IClock>(clocks);
        }

        public void AddClock(IClock clock)
        {
            if (clocks.Contains(clock))
            {
                return;
            }

            clocks.Add(clock);

            Tick(Time);
        }

        public void RemoveClock(IClock clock)
        {
            clocks.Remove(clock);
        }

        public void Tick(TimeSpan time)
        {
            this.Time = time;

            foreach (IClock clock in clocks)
            {
                clock.Tick(time);
            }

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
