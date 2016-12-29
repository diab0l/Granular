using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public interface IMinimalSet<T>
    {
        bool Add(T item);
        bool Contains(T item);
        bool Remove(T item);
        void Clear();

        IEnumerable<T> GetValues();
    }

    public interface IMinimalSet
    {
        bool Add(object item);
        bool Contains(object item);
        bool Remove(object item);
        void Clear();

        IEnumerable<object> GetValues();
    }

    public class MinimalSet<TValue> : IMinimalSet<TValue>, IMinimalSet
    {
        private HashSet<object> set;

        public MinimalSet(IEqualityComparer<TValue> comparer = null)
        {
            this.set = new HashSet<object>((IEqualityComparer<object>)comparer);
        }

        public bool Add(TValue item)
        {
            if (set.Contains(item))
            {
                return false;
            }

            set.Add(item);
            return true;
        }

        public bool Contains(TValue item)
        {
            return set.Contains(item);
        }

        public bool Remove(TValue item)
        {
            return set.Remove(item);
        }

        public void Clear()
        {
            set.Clear();
        }

        public IEnumerable<TValue> GetValues()
        {
            return set.Cast<TValue>();
        }

        bool IMinimalSet.Add(object item)
        {
            if (set.Contains(item))
            {
                return false;
            }

            set.Add(item);
            return true;
        }

        bool IMinimalSet.Contains(object item)
        {
            return set.Contains(item);
        }

        bool IMinimalSet.Remove(object item)
        {
            return set.Remove(item);
        }

        IEnumerable<object> IMinimalSet.GetValues()
        {
            return set;
        }
    }
}
