using System;
using System.Collections.Generic;
using System.Text;
using Granular.Compatibility.Linq;

namespace Granular.Collections
{
    public class ConvertedStringDictionary<TKey, TValue> : IMinimalDictionary<TKey, TValue>, IMinimalDictionary
    {
        private static readonly TValue DefaultValue = default(TValue);

        private IMinimalDictionary<string, object> keys;
        private IMinimalDictionary<string, object> values;
        private Func<TKey, string> getStringKey;

        public ConvertedStringDictionary(Func<TKey, string> getStringKey = null)
        {
            this.keys = new Granular.Compatibility.StringDictionary();
            this.values = new Granular.Compatibility.StringDictionary();
            this.getStringKey = getStringKey ?? (key => key.ToString());
        }

        public void Add(TKey key, TValue value)
        {
            string stringKey = getStringKey(key);

            keys.Add(stringKey, key);
            values.Add(stringKey, value);
        }

        public bool ContainsKey(TKey key)
        {
            return keys.ContainsKey(getStringKey(key));
        }

        public bool Remove(TKey key)
        {
            string stringKey = getStringKey(key);

            return keys.Remove(stringKey) && values.Remove(stringKey);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            object result;
            if (values.TryGetValue(getStringKey(key), out result))
            {
                value = (dynamic)result;
                return true;
            }

            value = DefaultValue;
            return false;
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public IEnumerable<TKey> GetKeys()
        {
            return keys.GetValues().Cast<TKey>();
        }

        public IEnumerable<TValue> GetValues()
        {
            return values.GetValues().Cast<TValue>();
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs()
        {
            string[] stringKeys = keys.GetKeys().ToArray();

            KeyValuePair<TKey, TValue>[] pairs = new KeyValuePair<TKey, TValue>[stringKeys.Length];

            int i = 0;
            foreach (string stringKey in stringKeys)
            {
                pairs[i] = new KeyValuePair<TKey, TValue>((dynamic)keys.GetValue(stringKey), (dynamic)values.GetValue(stringKey));
                i++;
            }

            return pairs;
        }

        void IMinimalDictionary.Add(object key, object value)
        {
            string stringKey = getStringKey((dynamic)key);

            keys.Add(stringKey, key);
            values.Add(stringKey, value);
        }

        bool IMinimalDictionary.ContainsKey(object key)
        {
            return keys.ContainsKey(getStringKey((dynamic)key));
        }

        bool IMinimalDictionary.Remove(object key)
        {
            string stringKey = getStringKey((dynamic)key);

            return keys.Remove(stringKey) && values.Remove(stringKey);
        }

        bool IMinimalDictionary.TryGetValue(object key, out object value)
        {
            return values.TryGetValue(getStringKey((dynamic)key), out value);
        }

        IEnumerable<object> IMinimalDictionary.GetKeys()
        {
            return keys.GetValues();
        }

        IEnumerable<object> IMinimalDictionary.GetValues()
        {
            return values.GetValues();
        }
    }
}
