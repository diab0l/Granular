using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public class CacheDictionary<TKey, TValue>
    {
        public delegate bool TryResolveValue(TKey key, out TValue value);
        public delegate TValue ResolveValue(TKey key);

        private TryResolveValue tryResolveValue;
        private ResolveValue resolveValue;

        private Dictionary<TKey, TValue> dictionary;
        private HashSet<TKey> unsetValues;

        public CacheDictionary(ResolveValue resolveValue) :
            this(null, resolveValue)
        {
            //
        }

        public CacheDictionary(TryResolveValue tryResolveValue) :
            this(tryResolveValue, null)
        {
            //
        }

        private CacheDictionary(TryResolveValue tryResolveValue, ResolveValue resolveValue)
        {
            this.tryResolveValue = tryResolveValue;
            this.resolveValue = resolveValue;

            dictionary = new Dictionary<TKey, TValue>();
            unsetValues = new HashSet<TKey>();
        }

        public TValue GetValue(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
            {
                return value;
            }

            throw new Granular.Exception("Key \"{0}\" was not found", key);
        }

        [System.Runtime.CompilerServices.Reflectable(false)]
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }

            if (unsetValues.Contains(key))
            {
                value = default(TValue);
                return false;
            }

            if (tryResolveValue != null && tryResolveValue(key, out value))
            {
                dictionary.Add(key, value);
                return true;
            }

            if (resolveValue != null)
            {
                value = resolveValue(key);
                dictionary.Add(key, value);
                return true;
            }

            unsetValues.Add(key);
            value = default(TValue);
            return false;
        }

        public bool Contains(TKey key)
        {
            return dictionary.ContainsKey(key) || unsetValues.Contains(key);
        }

        public void Remove(TKey key)
        {
            dictionary.Remove(key);
            unsetValues.Remove(key);
        }

        public void Clear()
        {
            dictionary.Clear();
            unsetValues.Clear();
        }
    }
}
