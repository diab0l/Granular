using System;
using System.Collections.Generic;
using System.Linq;
using System.Xaml;

namespace System.Windows.Markup
{
    public static class XamlTypes
    {
        private class NullProvider : IMarkupExtension
        {
            public object ProvideValue(InitializeContext context)
            {
                return null;
            }
        }

        [MarkupExtensionParameter("Type")]
        private class TypeProvider : IMarkupExtension
        {
            public Type Type { get; set; }

            public object ProvideValue(InitializeContext context)
            {
                return Type;
            }
        }

        private static readonly IDictionary<XamlName, Type> TypeProviders = new Dictionary<XamlName, Type>
        {
            { XamlLanguage.NullTypeName, typeof(NullProvider)},
            { XamlLanguage.TypeTypeName,  typeof(TypeProvider) },
        };

        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryParseXamlType(XamlName name, out Type type)
        {
            return TypeProviders.TryGetValue(name, out type);
        }

        public static Type ParseXamlType(XamlName xamlName)
        {
            Type type;

            if (!TryParseXamlType(xamlName, out type))
            {
                throw new Granular.Exception("Type {0} wasn't found", xamlName);
            }

            return type;
        }
    }
}
