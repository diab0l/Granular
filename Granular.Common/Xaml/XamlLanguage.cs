using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Xaml
{
    public static class XamlLanguage
    {
        public const string NamespaceName = "http://schemas.microsoft.com/winfx/2006/xaml";

        public static readonly XamlName ClassDirective = new XamlName("Class", NamespaceName);
        public static readonly XamlName NameDirective = new XamlName("Name", NamespaceName);
        public static readonly XamlName KeyDirective = new XamlName("Key", NamespaceName);
        public static readonly XamlName SharedDirective = new XamlName("Shared", NamespaceName);

        private static readonly IEnumerable<XamlName> Directives = new[]
        {
            ClassDirective,
            NameDirective,
            KeyDirective,
            SharedDirective,
        };

        public static readonly XamlName NullTypeName = new XamlName("Null", NamespaceName);
        public static readonly XamlName TypeTypeName = new XamlName("Type", NamespaceName);

        private static readonly IEnumerable<XamlName> XamlTypes = new[]
        {
            NullTypeName,
            TypeTypeName,
        };

        public static bool IsDirective(XamlName name)
        {
            return Directives.Contains(name);
        }

        public static bool IsXamlType(XamlName name)
        {
            return XamlTypes.Contains(name);
        }
    }
}
