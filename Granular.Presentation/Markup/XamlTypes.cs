using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

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
        [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
        private class TypeProvider : IMarkupExtension
        {
            public Type Type { get; set; }

            public object ProvideValue(InitializeContext context)
            {
                return Type;
            }
        }

        public static bool TryParseXamlType(XamlName name, out Type type)
        {
            if (name == XamlLanguage.NullTypeName)
            {
                type = typeof(NullProvider);
                return true;
            }

            if (name == XamlLanguage.TypeTypeName)
            {
                type = typeof(TypeProvider);
                return true;
            }

            type = null;
            return false;
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
