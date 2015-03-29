using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Media.Animation
{
    // root clock that can be connected / disconnected to / from a parent root clock
    public class AnimatableRootClock : IRootClock
    {
        public TimeSpan Time { get { return rootClock.Time; } }

        private IRootClock rootClock;
        private List<IClock> clocks;

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                if (isConnected == value)
                {
                    return;
                }

                isConnected = value;

                if (isConnected)
                {
                    foreach (IClock clock in clocks)
                    {
                        rootClock.AddClock(clock);
                    }
                }
                else
                {
                    foreach (IClock clock in clocks)
                    {
                        rootClock.RemoveClock(clock);
                    }

                    CleanClocks();
                }
            }
        }

        public AnimatableRootClock(IRootClock rootClock, bool isConnected)
        {
            this.rootClock = rootClock;
            this.isConnected = isConnected;

            clocks = new List<IClock>();
        }

        public void AddClock(IClock clock)
        {
            if (clocks.Contains(clock))
            {
                return;
            }

            clocks.Add(clock);

            if (IsConnected)
            {
                rootClock.AddClock(clock);
            }
        }

        public void RemoveClock(IClock clock)
        {
            clocks.Remove(clock);

            if (IsConnected)
            {
                rootClock.RemoveClock(clock);
            }
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
