using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xaml;
using Granular.Collections;
using Granular.Extensions;

namespace System.Windows
{
    public enum ResourceDictionaryLocation
    {
        None,
        SourceAssembly,
        ExternalAssembly,
    }

    public class ThemeInfoAttribute : Attribute
    {
        public ResourceDictionaryLocation GenericDictionaryLocation { get; private set; }

        public ThemeInfoAttribute(ResourceDictionaryLocation themeDictionaryLocation, ResourceDictionaryLocation genericDictionaryLocation) :
            this(genericDictionaryLocation)
        {
            //
        }

        public ThemeInfoAttribute(ResourceDictionaryLocation genericDictionaryLocation)
        {
            this.GenericDictionaryLocation = genericDictionaryLocation;
        }
    }

    public class SystemResources : IResourceContainer
    {
        private const string ThemeName = "Generic";
        private const string ThemeNameAndColor = "Generic";

        event EventHandler<ResourcesChangedEventArgs> IResourceContainer.ResourcesChanged { add { } remove { } }

        private CacheDictionary<object, object> resourcesCache;

        public SystemResources()
        {
            resourcesCache = new CacheDictionary<object, object>(TryResolveResource);
        }

        public bool TryGetResource(object resourceKey, out object value)
        {
            return resourcesCache.TryGetValue(resourceKey, out value);
        }

        private bool TryResolveResource(object resourceKey, out object value)
        {
            value = null;

            if (!(resourceKey is IResourceKey) || ((IResourceKey)resourceKey).Assembly == null)
            {
                return false;
            }

            Assembly assembly = ((IResourceKey)resourceKey).Assembly;
            ThemeInfoAttribute themeInfoAttribute = assembly.FirstOrDefaultCustomAttributeCached<ThemeInfoAttribute>();

            if (themeInfoAttribute == null || themeInfoAttribute.GenericDictionaryLocation == ResourceDictionaryLocation.None)
            {
                return false;
            }

            string assemblyName = themeInfoAttribute.GenericDictionaryLocation == ResourceDictionaryLocation.SourceAssembly ? assembly.GetName().Name : String.Format("{0}.{1}", assembly.GetName().Name, ThemeName);

            ResourceDictionary resourceDictionary = (ResourceDictionary)EmbeddedResourceLoader.LoadResourceElement(assemblyName, String.Format("Themes/{0}.xaml", ThemeNameAndColor));
            return resourceDictionary.TryGetValue(resourceKey, out value);
        }
    }
}
