using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Extensions;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public interface ISelectionGroup<T>
    {
        event EventHandler SelectionChanged;
        T Selection { get; set; }
    }

    public class SelectionGroup<T> : ISelectionGroup<T>
    {
        public event EventHandler SelectionChanged;

        public T selection;
        public T Selection
        {
            get { return selection; }
            set
            {
                if (Granular.Compatibility.EqualityComparer<object>.Default.Equals(this.selection, value))
                {
                    return;
                }

                selection = value;
                SelectionChanged.Raise(this);
            }
        }
    }

    public interface ISelectionGroupScope<T>
    {
        ISelectionGroup<T> GetSelectionGroup(string groupName);
    }

    public class SelectionGroupScope<T> : ISelectionGroupScope<T>
    {
        private Dictionary<string, ISelectionGroup<T>> groups;

        public SelectionGroupScope()
        {
            groups = new Dictionary<string, ISelectionGroup<T>>();
        }

        public ISelectionGroup<T> GetSelectionGroup(string groupName)
        {
            ISelectionGroup<T> group;

            if (!groups.TryGetValue(groupName, out group))
            {
                group = new SelectionGroup<T>();
                groups.Add(groupName, group);
            }

            return group;
        }
    }
}
