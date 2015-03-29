using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Array
    {
        public static int IndexOf<T>(T[] array, T value)
        {
            return System.Array.IndexOf(array, value);
        }

        public static int FindIndex<T>(T[] array, Predicate<T> match)
        {
            return System.Array.FindIndex(array, match);
        }

        public static int FindLastIndex<T>(T[] array, Predicate<T> match)
        {
            return System.Array.FindLastIndex(array, match);
        }
    }
}
