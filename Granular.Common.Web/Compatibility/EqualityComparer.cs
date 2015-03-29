using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        public static EqualityComparer<T> Default = new EqualityComparer<T>(System.Collections.Generic.EqualityComparer<T>.Default);

        private IEqualityComparer<T> comparer;

        public EqualityComparer(IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return x is double && System.Double.IsNaN((double)((object)x)) &&
                   y is double && System.Double.IsNaN((double)((object)y)) ||
                   comparer.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return comparer.GetHashCode(obj);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        [System.Runtime.CompilerServices.NonScriptable]
        bool IEqualityComparer.Equals(object x, object y)
        {
            return false;
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        [System.Runtime.CompilerServices.NonScriptable]
        int IEqualityComparer.GetHashCode(object obj)
        {
            return 0;
        }
    }
}