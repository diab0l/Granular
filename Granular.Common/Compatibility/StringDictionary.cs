using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace Granular.Compatibility
{
    public class StringDictionary : IMinimalDictionary<string, object>
    {
        private Dictionary<string, object> dictionary;

        public StringDictionary()
        {
            this.dictionary = new Dictionary<string, object>();
        }

        public void Add(string key, object value)
        {
            dictionary.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public IEnumerable<string> GetKeys()
        {
            return dictionary.Keys;
        }

        public IEnumerable<object> GetValues()
        {
            return dictionary.Values;
        }
    }
}
