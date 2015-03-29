using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class Comparer<T>
    {
        public static IComparer<T> Default
        {
            get { return System.Collections.Generic.Comparer<T>.Default; }
        }
    }
}
