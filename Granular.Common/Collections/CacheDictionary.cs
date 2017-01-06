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

        private IMinimalDictionary values;
        private IMinimalSet unsetValues;

        private CacheDictionary(TryResolveValue tryResolveValue, ResolveValue resolveValue, IMinimalDictionary valuesContainer, IMinimalSet unsetValuesContainer)
        {
            this.tryResolveValue = tryResolveValue;
            this.resolveValue = resolveValue;
            this.values = valuesContainer;
            this.unsetValues = unsetValuesContainer;
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
            object result;

            if (values.TryGetValue(key, out result))
            {
                value = (dynamic)result;
                return true;
            }

            if (resolveValue != null)
            {
                value = resolveValue(key);
                values.Add(key, value);
                return true;
            }

            if (unsetValues.Contains(key))
            {
                value = DefaultValue;
                return false;
            }

            if (tryResolveValue(key, out value))
            {
                values.Add(key, value);
                return true;
            }

            unsetValues.Add(key);
            value = DefaultValue;
            return false;
        }

        public bool Contains(TKey key)
        {
            return values.ContainsKey(key) || unsetValues.Contains(key);
        }

        public void Remove(TKey key)
        {
            values.Remove(key);
            unsetValues.Remove(key);
        }

        public void Clear()
        {
            values.Clear();
            unsetValues.Clear();
        }

        public static CacheDictionary<TKey, TValue> Create(TryResolveValue tryResolveValue, IEqualityComparer<TKey> comparer = null)
        {
            return new CacheDictionary<TKey, TValue>(tryResolveValue, null, new MinimalDictionary<TKey, TValue>(comparer), new MinimalSet<TKey>(comparer));
        }

        public static CacheDictionary<TKey, TValue> Create(ResolveValue resolveValue, IEqualityComparer<TKey> comparer = null)
        {
            return new CacheDictionary<TKey, TValue>(null, resolveValue, new MinimalDictionary<TKey, TValue>(comparer), new MinimalSet<TKey>(comparer));
        }

        public static CacheDictionary<TKey, TValue> CreateUsingStringKeys(TryResolveValue tryResolveValue, Func<TKey, string> getStringKey = null)
        {
            return new CacheDictionary<TKey, TValue>(tryResolveValue, null, new ConvertedStringDictionary<TKey, TValue>(getStringKey), new ConvertedStringSet<TKey>(getStringKey));
        }

        public static CacheDictionary<TKey, TValue> CreateUsingStringKeys(ResolveValue resolveValue, Func<TKey, string> getStringKey = null)
        {
            return new CacheDictionary<TKey, TValue>(null, resolveValue, new ConvertedStringDictionary<TKey, TValue>(getStringKey), new ConvertedStringSet<TKey>(getStringKey));
        }
    }
}
