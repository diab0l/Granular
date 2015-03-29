using System;
using System.Collections.Generic;
using System.Linq;

namespace Granular.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Scale(this TimeSpan timeSpan, double factor)
        {
            return TimeSpan.FromTicks((long)(timeSpan.Ticks * factor));
        }

        public static double Divide(this TimeSpan @this, TimeSpan timeSpan)
        {
            return (double)@this.Ticks / timeSpan.Ticks;
        }

        public static TimeSpan Min(this TimeSpan @this, TimeSpan timeSpan)
        {
            return @this < timeSpan ? @this : timeSpan;
        }

        public static TimeSpan Max(this TimeSpan @this, TimeSpan timeSpan)
        {
            return @this > timeSpan ? @this : timeSpan;
        }
    }
}
