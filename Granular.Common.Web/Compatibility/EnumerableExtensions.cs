using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            TSource minimum = source.First();

            foreach (TSource item in source.Skip(1))
            {
                if (Comparer<TSource>.Default.Compare(minimum, item) > 0)
                {
                    minimum = item;
                }
            }

            return minimum;
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            TSource maximum = source.First();

            foreach (TSource item in source.Skip(1))
            {
                if (Comparer<TSource>.Default.Compare(maximum, item) < 0)
                {
                    maximum = item;
                }
            }

            return maximum;
        }

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
