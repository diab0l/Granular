using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public class ConvertedStringSet<T> : IMinimalSet<T>, IMinimalSet
    {
        private IMinimalDictionary<string, object> items;
        private Func<T, string> getStringItem;

        public ConvertedStringSet(Func<T, string> getStringItem = null)
        {
            this.items = new Granular.Compatibility.StringDictionary();
            this.getStringItem = getStringItem ?? (item => item.ToString());
        }

        public bool Add(T item)
        {
            string stringItem = getStringItem(item);

            if (items.ContainsKey(stringItem))
            {
                return false;
            }

            items.Add(stringItem, item);
            return true;
        }

        public bool Contains(T item)
        {
            return items.ContainsKey(getStringItem(item));
        }

        public bool Remove(T item)
        {
            string stringItem = getStringItem(item);

            if (!items.ContainsKey(stringItem))
            {
                return false;
            }

            items.Remove(stringItem);
            return true;
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerable<T> GetValues()
        {
            return items.GetValues().Cast<T>();
        }

        bool IMinimalSet.Add(object item)
        {
            string stringItem = getStringItem((dynamic)item);

            if (items.ContainsKey(stringItem))
            {
                return false;
            }

            items.Add(stringItem, item);
            return true;
        }

        bool IMinimalSet.Contains(object item)
        {
            return items.ContainsKey(getStringItem((dynamic)item));
        }

        bool IMinimalSet.Remove(object item)
        {
            string stringItem = getStringItem((dynamic)item);

            if (!items.ContainsKey(stringItem))
            {
                return false;
            }

            items.Remove(stringItem);
            return true;
        }

        IEnumerable<object> IMinimalSet.GetValues()
        {
            return items.GetValues();
        }
    }
}
