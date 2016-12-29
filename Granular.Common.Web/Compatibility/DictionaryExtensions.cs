using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static void Clear<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            ((Dictionary<TKey, TValue>)dictionary).Clear();
        }
    }
}
