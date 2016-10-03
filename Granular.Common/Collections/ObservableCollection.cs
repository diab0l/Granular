using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Collections
{
    public interface IObservableCollection<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        //
    }

    public class ObservableCollection<T> : IObservableCollection<T>, IList<T>, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs("Count");
        public int Count { get { return items.Count; } }

        public T this[int index]
        {
            get { return items[index]; }
            set
            {
                if (Granular.Compatibility.EqualityComparer<T>.Default.Equals(items[index], value))
                {
                    return;
                }

                T oldItem = items[index];
                items[index] = value;
                CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Replace(oldItem, items[index], index));
            }
        }

        public bool IsReadOnly { get { return false; } }

        private List<T> items;

        public ObservableCollection() :
            this(new List<T>())
        {
            //
        }

        public ObservableCollection(IEnumerable<T> collection) :
            this(new List<T>(collection))
        {
            //
        }

        public ObservableCollection(int capacity) :
            this(new List<T>(capacity))
        {
            //
        }

        private ObservableCollection(List<T> items)
        {
            this.items = items;
        }

        public void Add(T item)
        {
            items.Add(item);
            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Add(item, Count - 1));
            PropertyChanged.Raise(this, CountPropertyChangedEventArgs);
        }

        public void Clear()
        {
            NotifyCollectionChangedEventArgs e = NotifyCollectionChangedEventArgs.RemoveRange(items.Cast<object>().ToArray(), 0);
            items.Clear();
            CollectionChanged.Raise(this, e);
            PropertyChanged.Raise(this, CountPropertyChangedEventArgs);
        }

        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Add(item, index));
            PropertyChanged.Raise(this, CountPropertyChangedEventArgs);
        }

        public bool Remove(T item)
        {
            int index = items.IndexOf(item);

            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            NotifyCollectionChangedEventArgs e = NotifyCollectionChangedEventArgs.Remove(items[index], index);
            items.RemoveAt(index);
            CollectionChanged.Raise(this, e);
            PropertyChanged.Raise(this, CountPropertyChangedEventArgs);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
