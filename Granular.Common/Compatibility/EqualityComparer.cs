using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        public static readonly EqualityComparer<T> Default = new EqualityComparer<T>(System.Collections.Generic.EqualityComparer<T>.Default);

        private System.Collections.Generic.EqualityComparer<T> comparer;

        public EqualityComparer(System.Collections.Generic.EqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return comparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return comparer.GetHashCode(obj);
        }
    }
}
