using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public class ResourcesChangedEventArgs : EventArgs
    {
        public static readonly ResourcesChangedEventArgs Reset = new ResourcesChangedEventArgs(null);

        private IEnumerable<object> changedKeys;

        private ResourcesChangedEventArgs(IEnumerable<object> changedKeys)
        {
            this.changedKeys = changedKeys;
        }

        public bool Contains(object resourceKey)
        {
            return changedKeys == null ? true : changedKeys.Contains(resourceKey);
        }

        public static ResourcesChangedEventArgs FromKey(object changedKey)
        {
            return new ResourcesChangedEventArgs(new[] { changedKey });
        }

        public static ResourcesChangedEventArgs FromKeyCollection(IEnumerable<object> changedKeys)
        {
            return new ResourcesChangedEventArgs(changedKeys);
        }
    }
}
