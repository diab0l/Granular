using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    public interface IResourceContainer
    {
        event EventHandler<ResourcesChangedEventArgs> ResourcesChanged;

        [System.Runtime.CompilerServices.Reflectable(false)]
        bool TryGetResource(object resourceKey, out object value);
    }

    public static class ResourceContainerExtensions
    {
        public static object FindResource(this IResourceContainer resourceContainer, object resourceKey)
        {
            object value;

            if (resourceContainer.TryGetResource(resourceKey, out value))
            {
                return value;
            }

            throw new Granular.Exception("Resource \"{0}\" is not found", resourceKey);
        }
    }
}
