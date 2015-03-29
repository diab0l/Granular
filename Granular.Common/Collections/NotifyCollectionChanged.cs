using System;
using System.Collections.Generic;
using System.Linq;

namespace Granular.Collections
{
    public delegate void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e);

    public interface INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }

    public enum NotifyCollectionChangedAction
    {
        Add,
        Remove,
        Replace,
        Move,
        Reset,
    }

    public class NotifyCollectionChangedEventArgs : EventArgs
    {
        public NotifyCollectionChangedAction Action { get; private set; }
        public IEnumerable<object> NewItems { get; private set; }
        public int NewStartingIndex { get; private set; }
        public IEnumerable<object> OldItems { get; private set; }
        public int OldStartingIndex { get; private set; }

        private NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IEnumerable<object> oldItems, int oldStartingIndex, IEnumerable<object> newItems, int newStartingIndex)
        {
            this.Action = action;
            this.OldItems = oldItems;
            this.OldStartingIndex = oldStartingIndex;
            this.NewItems = newItems;
            this.NewStartingIndex = newStartingIndex;
        }

        public static NotifyCollectionChangedEventArgs Add(object item, int index)
        {
            return AddRange(new [] { item }, index);
        }

        public static NotifyCollectionChangedEventArgs AddRange(IEnumerable<object> items, int startingIndex)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object[0], -1, items, startingIndex);
        }

        public static NotifyCollectionChangedEventArgs Remove(object item, int index)
        {
            return RemoveRange(new [] { item }, index);
        }

        public static NotifyCollectionChangedEventArgs RemoveRange(IEnumerable<object> items, int startingIndex)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items, startingIndex, new object[0], -1);
        }

        public static NotifyCollectionChangedEventArgs Replace(object oldItem, object newItem, int index)
        {
            return ReplaceRange(new [] { oldItem }, new [] { newItem }, index);
        }

        public static NotifyCollectionChangedEventArgs ReplaceRange(IEnumerable<object> oldItems, IEnumerable<object> newItems, int index)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldItems, index, newItems, index);
        }

        public static NotifyCollectionChangedEventArgs Move(object item, int oldIndex, int newIndex)
        {
            return MoveRange(new[] { item }, oldIndex, newIndex);
        }

        public static NotifyCollectionChangedEventArgs MoveRange(IEnumerable<object> items, int oldStartingIndex, int newStartingIndex)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, items, oldStartingIndex, items, newStartingIndex);
        }

        public static NotifyCollectionChangedEventArgs Reset(IEnumerable<object> oldItems, IEnumerable<object> newItems)
        {
            return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, oldItems, 0, newItems, 0);
        }
    }

    public static class NotifyCollectionChangedEventHandlerExtensions
    {
        static public void Raise(this NotifyCollectionChangedEventHandler handler, object sender, NotifyCollectionChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
