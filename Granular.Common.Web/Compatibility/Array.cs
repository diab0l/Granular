using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Granular.Compatibility
{
    public static class Array
    {
        public static int IndexOf<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (EqualityComparer<T>.Default.Equals(array[i], value))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindIndex<T>(T[] array, Func<T, bool> match)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (match(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int FindLastIndex<T>(T[] array, Func<T, bool> match)
        {
            int lastIndex = -1;

            for (int i = 0; i < array.Length; i++)
            {
                if (match(array[i]))
                {
                    lastIndex = i;
                }
            }

            return lastIndex;
        }

        [InlineCode("{array}")]
        public static T[] ImplicitCast<T>(object array)
        {
            return null;
        }
    }
}
