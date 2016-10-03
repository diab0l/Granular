using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Extensions
{
    public static class EnumerableExtensions
    {
        public delegate bool TrySelectDelegate<TSource, TResult>(TSource source, out TResult result);

        public static IEnumerable<TSource> ConcatSingle<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return source.Concat(new[] { value });
        }

        public static IEnumerable<TResult> TrySelect<TSource, TResult>(this IEnumerable<TSource> source, TrySelectDelegate<TSource, TResult> selector)
        {
            foreach (TSource item in source)
            {
                TResult result;
                if (selector(item, out result))
                {
                    yield return result;
                }
            }
        }
    }
}
