using System;
using System.Collections;
using System.Collections.Generic;

namespace Granular.Compatibility.Linq
{
    public static class Enumerable
    {
        public static TSource Aggregate<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, TSource> func)
        {
            return System.Linq.Enumerable.Aggregate(source, func);
        }

        public static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.All(source, predicate);
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Any(source);
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.Any(source, predicate);
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            return System.Linq.Enumerable.Cast<TResult>(source);
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.Concat(first, second);
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            return System.Linq.Enumerable.Contains(source, value);
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Count(source);
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.Count(source, predicate);
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.DefaultIfEmpty(source);
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            return System.Linq.Enumerable.DefaultIfEmpty(source, defaultValue);
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Distinct(source);
        }

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            return System.Linq.Enumerable.ElementAt(source, index);
        }

        public static IEnumerable<TResult> Empty<TResult>()
        {
            return new TResult[0];
        }

        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.Except(first, second);
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.First(source);
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.First(source, predicate);
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.FirstOrDefault(source);
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.FirstOrDefault(source, predicate);
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.Intersect(first, second);
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Last(source);
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.Last(source, predicate);
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.LastOrDefault(source);
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.LastOrDefault(source, predicate);
        }

        public static double Max(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Max(source);
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Max(source);
        }

        public static double Min(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Min(source);
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Min(source);
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
        {
            return System.Linq.Enumerable.OfType<TResult>(source);
        }

        public static IEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return System.Linq.Enumerable.OrderBy(source, keySelector);
        }

        public static IEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return System.Linq.Enumerable.OrderByDescending(source, keySelector);
        }

        public static IEnumerable<TSource> Reverse<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Reverse(source);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return System.Linq.Enumerable.SelectMany(source, selector);
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.SequenceEqual(first, second);
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Single(source);
        }

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            return System.Linq.Enumerable.Skip(source, count);
        }

        public static int Sum(this IEnumerable<int> source)
        {
            return System.Linq.Enumerable.Sum(source);
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return System.Linq.Enumerable.Sum(source, selector);
        }

        public static double Sum(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Sum(source);
        }

        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return System.Linq.Enumerable.Sum(source, selector);
        }

        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            return System.Linq.Enumerable.Take(source, count);
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.ToArray(source);
        }

        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.ToList(source);
        }

        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.Union(first, second);
        }

        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return System.Linq.Enumerable.Where(source, predicate);
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return System.Linq.Enumerable.Zip(first, second, resultSelector);
        }
    }
}
