using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class TimeSpan
    {
        public static readonly System.TimeSpan MinValue = System.TimeSpan.MinValue;
        public static readonly System.TimeSpan MaxValue = System.TimeSpan.MaxValue;

        public static bool TryParse(string s, out System.TimeSpan result)
        {
            return System.TimeSpan.TryParse(s, out result);
        }

        public static System.TimeSpan Subtract(DateTime value1, DateTime value2)
        {
            return value1 - value2;
        }
    }
}
