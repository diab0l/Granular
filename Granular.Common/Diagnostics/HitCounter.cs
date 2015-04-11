using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Diagnostics
{
    public class HitCounter
    {
        private string name;
        private Func<string> getAdditionalStatus;

        private int totalHitsCount;
        private int lastHitsCount;
        private DateTime lastReportTime;

        public HitCounter(string name, Func<string> getAdditionalStatus = null)
        {
            this.name = name;
            this.getAdditionalStatus = getAdditionalStatus;

            lastReportTime = DateTime.Now;
            Console.WriteLine(String.Format("{0} - HitCounter initialized", name));
        }

        public void Hit()
        {
            totalHitsCount++;

            DateTime now = DateTime.Now;
            TimeSpan interval = Granular.Compatibility.TimeSpan.Subtract(now, lastReportTime);

            if (interval >= TimeSpan.FromSeconds(1))
            {
                double rate = Math.Round((totalHitsCount - lastHitsCount) / interval.TotalSeconds, 1);
                string additionalStatus = getAdditionalStatus != null ? getAdditionalStatus() : String.Empty;

                if (additionalStatus.IsNullOrEmpty())
                {
                    Console.WriteLine(String.Format("{0} - Total: {1} hits, Rate: {2} hits/sec", name, totalHitsCount, rate));
                }
                else
                {
                    Console.WriteLine(String.Format("{0} - Total: {1} hits, Rate: {2} hits/sec - {3}", name, totalHitsCount, rate, additionalStatus));
                }

                lastReportTime = now;
                lastHitsCount = totalHitsCount;
            }
        }
    }
}
