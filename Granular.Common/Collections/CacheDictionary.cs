using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public class CacheDictionary<TKey, TValue>
    {
        private static readonly TValue DefaultValue = default(TValue);

        public delegate bool TryResolveValue(TKey key, out TValue value);
        public delegate TValue ResolveValue(TKey key);

        private TryResolveValue tryResolveValue;
        private ResolveValue resolveValue;

        private Dictionary<TKey, TValue> dictionary;
        private HashSet<TKey> unsetValues;

        public CacheDictionary(ResolveValue resolveValue, IEqualityComparer<TKey> equalityComparer = null) :
            this(null, resolveValue, equalityComparer)
        {
            //
        }

        public CacheDictionary(TryResolveValue tryResolveValue, IEqualityComparer<TKey> equalityComparer = null) :
            this(tryResolveValue, null, equalityComparer)
        {
            //
        }

        private CacheDictionary(TryResolveValue tryResolveValue, ResolveValue resolveValue, IEqualityComparer<TKey> equalityComparer)
        {
            this.tryResolveValue = tryResolveValue;
            this.resolveValue = resolveValue;

            dictionary = new Dictionary<TKey, TValue>(equalityComparer);
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

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }

            if (unsetValues.Contains(key))
            {
                value = DefaultValue;
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
            value = DefaultValue;
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
