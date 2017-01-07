using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Granular.Collections;

namespace System.Windows.Markup
{
    public interface IDeferredValueKeyProvider
    {
        object GetValueKey(XamlElement element);
    }

    public class DeferredValueKeyProviderAttribute : Attribute
    {
        public Type ProviderType { get; private set; }

        public DeferredValueKeyProviderAttribute(Type providerType)
        {
            this.ProviderType = providerType;
        }
    }

    public static class DeferredValueKeyProviders
    {
        private static CacheDictionary<Type, IDeferredValueKeyProvider> DeferredValueKeyProviderCache = CacheDictionary<Type, IDeferredValueKeyProvider>.CreateUsingStringKeys(ResolveDeferredValueKeyProvider, type => type.FullName);

        public static IDeferredValueKeyProvider GetDeferredValueKeyProvider(Type type)
        {
            return DeferredValueKeyProviderCache.GetValue(type);
        }

        private static IDeferredValueKeyProvider ResolveDeferredValueKeyProvider(Type type)
        {
            DeferredValueKeyProviderAttribute deferredValueKeyProviderAttribute = type.GetCustomAttributes(typeof(DeferredValueKeyProviderAttribute), false).FirstOrDefault() as DeferredValueKeyProviderAttribute;
            if (deferredValueKeyProviderAttribute != null)
            {
                return Activator.CreateInstance(deferredValueKeyProviderAttribute.ProviderType) as IDeferredValueKeyProvider;
            }

            return null;
        }
    }
}
