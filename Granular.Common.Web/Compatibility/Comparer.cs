using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Comparer<T>
    {
        private class CompatibleComparer : IComparer<T>
        {
            private System.Collections.Generic.Comparer<T> comparer;

            public CompatibleComparer(System.Collections.Generic.Comparer<T> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(T x, T y)
            {
                return comparer.Compare(x, y);
            }
        }

        public static IComparer<T> Default
        {
            get { return new CompatibleComparer(System.Collections.Generic.Comparer<T>.Default); }
        }
    }
}
