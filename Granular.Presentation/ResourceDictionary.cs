using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows
{
    [SupportsValueProvider]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class ResourceDictionary : IDictionary<object, object>, IResourceContainer, IUriContext
    {
        public event EventHandler<ResourcesChangedEventArgs> ResourcesChanged;

        public ObservableCollection<ResourceDictionary> MergedDictionaries { get; private set; }

        public Uri BaseUri { get; set; }

        private ResourceDictionary sourceDictionary;
        private Uri source;
        public Uri Source
        {
            get { return source; }
            set
            {
                if (source == value)
                {
                    return;
                }

                if (sourceDictionary != null)
                {
                    MergedDictionaries.Remove(sourceDictionary);
                }

                source = value;
                sourceDictionary = LoadResourceDictionary(source.ResolveAbsoluteUri(BaseUri));

                if (sourceDictionary != null)
                {
                    MergedDictionaries.Add(sourceDictionary);
                }
            }
        }

        public int Count { get { return dictionary.Count + MergedDictionaries.Select(mergedDictionary => mergedDictionary.Count).DefaultIfEmpty(0).Sum(); } }

        public ICollection<object> Keys { get { return dictionary.Keys; } }

        public ICollection<object> Values { get { return dictionary.Values; } }

        public object this[object key]
        {
            get { return GetValue(key); }
            set { Add(key, value); }
        }

        public bool IsReadOnly { get { return false; } }

        private Dictionary<object, object> dictionary;

        public ResourceDictionary()
        {
            MergedDictionaries = new ObservableCollection<ResourceDictionary>();
            MergedDictionaries.CollectionChanged += OnMergedDictionariesCollectionChanged;

            dictionary = new Dictionary<object, object>();
        }

        public object GetValue(object key)
        {
            object value;

            if (!TryGetValue(key, out value))
            {
                throw new Granular.Exception("Cannot find resource named \"{0}\"", key);
            }

            return value;
        }

        public bool TryGetValue(object key, out object value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                if (value is IValueProvider)
                {
                    value = ((IValueProvider)value).ProvideValue();
                }

                return true;
            }

            foreach (ResourceDictionary mergedDictionary in MergedDictionaries)
            {
                if (mergedDictionary.TryGetValue(key, out value))
                {
                    return true;
                }
            }

            value = null;
            return false;
        }

        public void Add(object key, object value)
        {
            dictionary[key] = value;
            ResourcesChanged.Raise(this, ResourcesChangedEventArgs.FromKey(key));
        }

        public void Clear()
        {
            ResourcesChangedEventArgs e = ResourcesChangedEventArgs.FromKeyCollection(dictionary.Keys.ToArray());
            dictionary.Clear();
            ResourcesChanged.Raise(this, e);
        }

        public bool Contains(object key)
        {
            return dictionary.Keys.Contains(key) || MergedDictionaries.Any(d => ((IDictionary<object, object>)d).Keys.Contains(key));
        }

        public bool Remove(object key)
        {
            if (dictionary.Remove(key))
            {
                ResourcesChanged.Raise(this, ResourcesChangedEventArgs.FromKey(key));
                return true;
            }

            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        private void OnMergedDictionariesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (ResourceDictionary dictionary in e.OldItems)
            {
                dictionary.ResourcesChanged -= OnMergeDictionaryResourcesChanged;
            }

            foreach (ResourceDictionary dictionary in e.NewItems)
            {
                dictionary.ResourcesChanged += OnMergeDictionaryResourcesChanged;
            }

            if (e.Action != NotifyCollectionChangedAction.Move)
            {
                IEnumerable<object> keys = e.OldItems.Concat(e.NewItems).Cast<ResourceDictionary>().SelectMany(GetMergedDictionariesKeys);
                ResourcesChanged.Raise(this, ResourcesChangedEventArgs.FromKeyCollection(keys));
            }
        }

        private void OnMergeDictionaryResourcesChanged(object sender, ResourcesChangedEventArgs e)
        {
            ResourcesChanged.Raise(this, e);
        }

        private static IEnumerable<object> GetMergedDictionariesKeys(ResourceDictionary dictionary)
        {
            return ((IDictionary<object, object>)dictionary).Keys.Concat(dictionary.MergedDictionaries.SelectMany(GetMergedDictionariesKeys));
        }

        private static ResourceDictionary LoadResourceDictionary(Uri source)
        {
            return Application.LoadComponent(source) as ResourceDictionary;
        }

        bool IResourceContainer.TryGetResource(object resourceKey, out object value)
        {
            return TryGetValue(resourceKey, out value);
        }

        bool IDictionary<object, object>.ContainsKey(object key)
        {
            return Contains(key);
        }

        bool IDictionary<object, object>.Remove(object key)
        {
            return Remove(key);
        }

        IEnumerator<KeyValuePair<object, object>> IEnumerable<KeyValuePair<object, object>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Add(KeyValuePair<object, object> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<object, object> item)
        {
            object value;
            return TryGetValue(item.Key, out value) && item.Value == value;
        }

        public bool Remove(KeyValuePair<object, object> item)
        {
            return ((ICollection<KeyValuePair<object, object>>)this).Contains(item) && Remove(item.Key);
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            dictionary.ToArray().CopyTo(array, arrayIndex);
        }
    }
}
