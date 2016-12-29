using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Granular.Collections;

namespace Granular.Compatibility
{
    public class StringDictionary : IMinimalDictionary<string, object>
    {
        private object dictionary;

        static StringDictionary()
        {
            /*@
                if (!Object.values) {
                    Object.values = function (obj) {
                        return Object.keys(obj).map(function (key) { return obj[key]; });
                    }
                }
            */
        }

        public StringDictionary()
        {
            dictionary = new object();
        }

        public void Add(string key, object value)
        {
            if (!IsUndefined(dictionary[key]))
            {
                throw new Granular.Exception("An item with the same key has already been added.");
            }

            dictionary[key] = value;
        }

        public bool ContainsKey(string key)
        {
            return !IsUndefined(dictionary[key]);
        }

        public bool Remove(string key)
        {
            if (IsUndefined(dictionary[key]))
            {
                return false;
            }

            DeleteProperty(dictionary[key]);
            return true;
        }

        public bool TryGetValue(string key, out object value)
        {
            value = dictionary[key];

            if (IsUndefined(value))
            {
                value = null;
                return false;
            }

            return true;
        }

        public void Clear()
        {
            dictionary = new object();
        }

        public IEnumerable<string> GetKeys()
        {
            return GetKeys(dictionary);
        }

        public IEnumerable<object> GetValues()
        {
            return GetValues(dictionary);
        }

        [Bridge.Template("({value} === undefined)")]
        private static extern bool IsUndefined(object value);

        [Bridge.Template("delete {value}")]
        private static extern object DeleteProperty(object value);

        [Bridge.Template("Object.keys({obj})")]
        private static extern string[] GetKeys(object obj);

        [Bridge.Template("Object.values({obj})")]
        private static extern object[] GetValues(object obj);
    }
}
