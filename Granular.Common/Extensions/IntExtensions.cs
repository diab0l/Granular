using System;
using System.Collections.Generic;
using System.Linq;

namespace Granular.Extensions
{
    public static class IntExtensions
    {
        public static int Min(this int @this, int value)
        {
            return @this < value ? @this : value;
        }

        public static int Max(this int @this, int value)
        {
            return @this > value ? @this : value;
        }

        public static int Bounds(this int @this, int minimum, int maximum)
        {
            if (minimum > maximum)
            {
                throw new Granular.Exception("Invalid bounds (minimum: {0}, maximum: {1})", minimum, maximum);
            }

            return @this.Max(minimum).Min(maximum);
        }
    }
}
