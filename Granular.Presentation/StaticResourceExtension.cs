using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    [MarkupExtensionParameter("ResourceKey")]
    public class StaticResourceExtension : IMarkupExtension
    {
        public object ResourceKey { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return GetStaticResource(context, ResourceKey);
        }

        private static object GetStaticResource(InitializeContext context, object resourceKey)
        {
            if (context == null)
            {
                throw new Granular.Exception("StaticResource \"{0}\" was not found", resourceKey);
            }

            object value;
            return context.Target is IResourceContainer && ((IResourceContainer)context.Target).TryGetResource(resourceKey, out value) ? value : GetStaticResource(context.ParentContext, resourceKey);
        }
    }
}
