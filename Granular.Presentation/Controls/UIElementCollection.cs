using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace System.Windows.Controls
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class UIElementCollection : IList<UIElement>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count { get { return children.Count; } }

        public UIElement this[int index]
        {
            get { return children[index]; }
            set
            {
                if (children[index] == value)
                {
                    return;
                }

                NotifyCollectionChangedEventArgs args = NotifyCollectionChangedEventArgs.Replace(children[index], value, index);

                if (children[index] != null)
                {
                    ClearChildParent(children[index]);
                }

                children[index] = value;

                if (children[index] != null)
                {
                    SetChildParent(children[index]);
                }

                CollectionChanged.Raise(this, args);
            }
        }

        public bool IsReadOnly { get { return false; } }

        private UIElement parent;
        private List<UIElement> children;

        public UIElementCollection(UIElement parent)
        {
            this.parent = parent;
            this.children = new List<UIElement>();
        }

        public void Add(UIElement item)
        {
            children.Add(item);
            SetChildParent(item);

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Add(item, children.Count));
        }

        public void Clear()
        {
            IEnumerable<UIElement> lastChildren = children.ToArray();
            children.Clear();

            foreach (UIElement child in lastChildren)
            {
                ClearChildParent(child);
            }

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.RemoveRange(lastChildren, 0));
        }

        public bool Contains(UIElement item)
        {
            return children.Contains(item);
        }

        public int IndexOf(UIElement item)
        {
            return children.IndexOf(item);
        }

        public void Insert(int index, UIElement item)
        {
            children.Insert(index, item);
            SetChildParent(item);

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Add(item, index));
        }

        public bool Remove(UIElement item)
        {
            int index = children.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            bool result = children.Remove(item);
            ClearChildParent(item);

            CollectionChanged.Raise(this, NotifyCollectionChangedEventArgs.Remove(item, index));

            return result;
        }

        public void RemoveAt(int index)
        {
            Remove(children[index]);
        }

        public void CopyTo(UIElement[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        private void ClearChildParent(UIElement child)
        {
            parent.RemoveLogicalChild(child);
            parent.RemoveVisualChild(child);
        }

        private void SetChildParent(UIElement child)
        {
            parent.AddLogicalChild(child);
            parent.AddVisualChild(child);
        }

        public IEnumerator<UIElement> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
