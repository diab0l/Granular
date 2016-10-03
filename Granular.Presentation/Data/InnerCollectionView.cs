using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows.Controls
{
    // implementation of ICollectionView with an inner collection
    public class InnerCollectionView : ICollectionView, IList<object>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler CurrentChanged;

        public IEnumerable SourceCollection { get { return this; } }

        private object currentItem;
        public object CurrentItem
        {
            get { return currentItem; }
            set { SetCurrent(value); }
        }

        private int currentItemIndex;
        public int CurrentItemIndex
        {
            get { return currentItemIndex; }
            set { SetCurrent(value); }
        }

        public bool CanFilter { get { return false; } }

        public Func<object, bool> FilterPredicate { get; set; }

        public bool CanSort { get { return false; } }

        public Func<object, object> SortKeySelector { get; set; }

        public ListSortDirection SortDirection { get; set; }

        public object this[int index]
        {
            get { return innerCollection[index]; }
            set { innerCollection[index] = value; }
        }

        public int Count { get { return innerCollection.Count; } }

        public bool IsReadOnly { get { return false; } }

        private ObservableCollection<object> innerCollection;

        public InnerCollectionView()
        {
            innerCollection = new ObservableCollection<object>();
            innerCollection.CollectionChanged += OnInnerCollectionChanged;
        }

        private void OnInnerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetCurrent(CurrentItem);
            CollectionChanged.Raise(this, e);
        }

        private void SetCurrent(object item)
        {
            int itemIndex = innerCollection.IndexOf(item);

            if (itemIndex != -1)
            {
                SetCurrent(item, itemIndex);
            }
            else if (currentItemIndex >= 0 && currentItemIndex < innerCollection.Count)
            {
                SetCurrent(innerCollection[currentItemIndex], currentItemIndex);
            }
            else
            {
                SetCurrent(null, Math.Min(currentItemIndex, innerCollection.Count));
            }
        }

        private void SetCurrent(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < innerCollection.Count)
            {
                SetCurrent(innerCollection[itemIndex], itemIndex);
            }
            else
            {
                SetCurrent(null, Math.Min(itemIndex, innerCollection.Count));
            }
        }

        private void SetCurrent(object item, int itemIndex)
        {
            if (this.currentItem == item && this.currentItemIndex == itemIndex)
            {
                return;
            }

            this.currentItem = item;
            this.currentItemIndex = itemIndex;

            CurrentChanged.Raise(this);
        }

        public void Add(object value)
        {
            innerCollection.Add(value);
        }

        public void Clear()
        {
            innerCollection.Clear();
        }

        public bool Contains(object value)
        {
            return innerCollection.Contains(value);
        }

        public int IndexOf(object value)
        {
            return innerCollection.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            innerCollection.Insert(index, value);
        }

        public bool Remove(object value)
        {
            return innerCollection.Remove(value);
        }

        public void RemoveAt(int index)
        {
            innerCollection.RemoveAt(index);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            innerCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
