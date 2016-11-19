using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Collections
{
    public enum ListSortDirection
    {
        Ascending,
        Descending
    }

    public interface ICollectionView : IObservableCollection<object>
    {
        event EventHandler CurrentChanged;

        IEnumerable SourceCollection { get; }

        object CurrentItem { get; set; }
        int CurrentItemIndex { get; set; }

        bool CanFilter { get; }
        Func<object, bool> FilterPredicate { get; set; }

        bool CanSort { get; }
        Func<object, object> SortKeySelector { get; set; }
        ListSortDirection SortDirection { get; set; }
    }
}
