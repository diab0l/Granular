using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (T item in source)
            {
                array[i] = item;
                i++;
            }
        }
    }
}
