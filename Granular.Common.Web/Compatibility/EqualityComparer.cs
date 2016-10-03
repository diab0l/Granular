using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class EqualityComparer
    {
        public static readonly EqualityComparer<object> Default = new EqualityComparer<object>(System.Collections.Generic.EqualityComparer<object>.Default);
        public static readonly EqualityComparer<double> Double = new EqualityComparer<double>(System.Collections.Generic.EqualityComparer<double>.Default);
    }

    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly EqualityComparer<T> Default = new EqualityComparer<T>(System.Collections.Generic.EqualityComparer<T>.Default);

        private IEqualityComparer<T> comparer;

        public EqualityComparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return comparer.Equals(x, y) || IsNaN(x) && IsNaN(y);
        }

        public int GetHashCode(T obj)
        {
            return comparer.GetHashCode(obj);
        }

        [Bridge.Template("{value} !== {value}")]
        private static extern bool IsNaN(object value);
    }
}