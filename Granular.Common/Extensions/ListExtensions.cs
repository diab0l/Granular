using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Extensions
{
    public static class ListExtensions
    {
        public static void InsertRange<T>(this IList<T> list, int index, IEnumerable<T> values)
        {
            foreach (T value in values)
            {
                list.Insert(index, value);
                index++;
            }
        }

        public static void RemoveRange<T>(this IList<T> list, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.RemoveAt(index);
            }
        }
    }
}
