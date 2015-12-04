using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> ConcatSingle<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return source.Concat(new[] { value });
        }
    }
}
