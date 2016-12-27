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
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.All(source, predicate);
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (!predicate(sourceArray[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Any(source);
            }

            return sourceArray.Length > 0;
        }

        public static bool Any<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Any(source, predicate);
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (predicate(sourceArray[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
        {
            System.Array sourceArray = source as System.Array;

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Cast<TResult>(source);
            }

            TResult[] resultArray = new TResult[sourceArray.Length];

            for (int i = 0; i < sourceArray.Length; i++)
            {
                resultArray[i] = (TResult)sourceArray.GetValue(i);
            }

            return resultArray;
        }

        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            TSource[] firstArray = first.AsArray();
            TSource[] secondArray = second.AsArray();

            if (firstArray == null || secondArray == null)
            {
                return System.Linq.Enumerable.Concat(first, second);
            }

            TSource[] resultArray = new TSource[firstArray.Length + secondArray.Length];
            int j = 0;

            for (int i = 0; i < firstArray.Length; i++)
            {
                resultArray[j] = firstArray[i];
                j++;
            }

            for (int i = 0; i < secondArray.Length; i++)
            {
                resultArray[j] = secondArray[i];
                j++;
            }

            return resultArray;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Contains(source, value);
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (Object.Equals(sourceArray[i], value))
                {
                    return true;
                }
            }

            return false;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Count(source);
            }

            return sourceArray.Length;
        }

        public static int Count<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Count(source, predicate);
            }

            int count = 0;

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (predicate(sourceArray[i]))
                {
                    count++;
                }
            }

            return count;
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.DefaultIfEmpty(source);
            }

            return sourceArray.Length > 0 ? sourceArray : new TSource[] { default(TSource) };
        }

        public static IEnumerable<TSource> DefaultIfEmpty<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.DefaultIfEmpty(source, defaultValue);
            }

            return sourceArray.Length > 0 ? sourceArray : new TSource[] { defaultValue };
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return System.Linq.Enumerable.Distinct(source);
        }

        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.ElementAt(source, index);
            }

            return sourceArray[index];
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
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.First(source);
            }

            if (sourceArray.Length == 0)
            {
                throw new Granular.Exception("Sequence contains no elements");
            }

            return sourceArray[0];
        }

        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.First(source, predicate);
            }

            if (sourceArray.Length == 0)
            {
                throw new Exception("Sequence contains no elements");
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (predicate(sourceArray[i]))
                {
                    return sourceArray[i];
                }
            }

            throw new Exception("Sequence contains no matching element");
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.FirstOrDefault(source);
            }

            return sourceArray.Length > 0 ? sourceArray[0] : default(TSource);
        }

        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.FirstOrDefault(source, predicate);
            }

            for (int i = 0; i < sourceArray.Length; i++)
            {
                if (predicate(sourceArray[i]))
                {
                    return sourceArray[i];
                }
            }

            return default(TSource);
        }

        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.Intersect(first, second);
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Last(source);
            }

            if (sourceArray.Length == 0)
            {
                throw new Granular.Exception("Sequence contains no elements");
            }

            return sourceArray[sourceArray.Length - 1];
        }

        public static TSource Last<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Last(source, predicate);
            }

            if (sourceArray.Length == 0)
            {
                throw new Exception("Sequence contains no elements");
            }

            for (int i = sourceArray.Length - 1; i >= 0; i--)
            {
                if (predicate(sourceArray[i]))
                {
                    return sourceArray[i];
                }
            }

            throw new Exception("Sequence contains no matching element");
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.LastOrDefault(source);
            }

            return sourceArray.Length > 0 ? sourceArray[sourceArray.Length - 1] : default(TSource);
        }

        public static TSource LastOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.LastOrDefault(source, predicate);
            }

            for (int i = sourceArray.Length - 1; i >= 0; i--)
            {
                if (predicate(sourceArray[i]))
                {
                    return sourceArray[i];
                }
            }

            return default(TSource);
        }

        public static double Max(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Max(source);
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source) where TSource : IComparable
        {
            return System.Linq.Enumerable.Max(source);
        }

        public static double Min(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Min(source);
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source) where TSource : IComparable
        {
            return System.Linq.Enumerable.Min(source);
        }

        public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source) where TResult : class
        {
            object[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.OfType<TResult>(source);
            }

            TResult[] resultArray = new TResult[0];

            for (int i = 0; i < sourceArray.Length; i++)
            {
                TResult item = sourceArray[i] as TResult;
                if (item != null)
                {
                    resultArray.Push(item);
                }
            }

            return sourceArray.Length == resultArray.Length ? (dynamic)sourceArray : resultArray;
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
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Select(source, selector);
            }

            TResult[] resultArray = new TResult[sourceArray.Length];

            for (int i = 0; i < sourceArray.Length; i++)
            {
                resultArray[i] = selector(sourceArray[i]);
            }

            return resultArray;
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.SelectMany(source, selector);
            }

            TResult[] resultArray = new TResult[0];

            for (int i = 0; i < sourceArray.Length; i++)
            {
                foreach (TResult result in selector(sourceArray[i]))
                {
                    resultArray.Push(result);
                }
            }

            return resultArray;
        }

        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second)
        {
            return System.Linq.Enumerable.SequenceEqual(first, second);
        }

        public static TSource Single<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Single(source);
            }

            if (sourceArray.Length == 1)
            {
                return sourceArray[0];
            }

            if (sourceArray.Length == 0)
            {
                throw new Granular.Exception("Sequence contains no elements");
            }

            throw new Granular.Exception("Sequence contains more than one element");
        }

        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Skip(source, count);
            }

            int length = sourceArray.Length - count;
            TSource[] resultArray = new TSource[length];

            System.Array.Copy(sourceArray, count, resultArray, 0, length);

            return resultArray;
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
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Take(source, count);
            }

            TSource[] resultArray = new TSource[count];

            System.Array.Copy(sourceArray, resultArray, count);

            return resultArray;
        }

        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source)
        {
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.ToArray(source);
            }

            TSource[] resultArray = new TSource[sourceArray.Length];

            System.Array.Copy(sourceArray, resultArray, sourceArray.Length);

            return resultArray;
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
            TSource[] sourceArray = source.AsArray();

            if (sourceArray == null)
            {
                return System.Linq.Enumerable.Where(source, predicate);
            }

            TSource[] resultArray = new TSource[0];

            for (int i = 0; i < sourceArray.Length; i++)
            {
                TSource item = sourceArray[i];
                if (predicate(item))
                {
                    resultArray.Push(item);
                }
            }

            return sourceArray.Length == resultArray.Length ? sourceArray : resultArray;
        }

        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            return System.Linq.Enumerable.Zip(first, second, resultSelector);
        }

        private static object[] AsArray(this IEnumerable source)
        {
            return (dynamic)(source as System.Array ?? source["items"] as System.Array);
        }

        private static TSource[] AsArray<TSource>(this IEnumerable<TSource> source)
        {
            return (dynamic)(source as System.Array ?? source["items"] as System.Array);
        }
    }
}
