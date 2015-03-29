using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Double
    {
        public static bool TryParse(string s, out double result)
        {
            return System.Double.TryParse(s, out result);
        }

        public static bool IsInfinity(double d)
        {
            return System.Double.IsInfinity(d);
        }
    }
}
