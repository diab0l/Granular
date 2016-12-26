using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Markup
{
    public static class XamlLanguage
    {
        public const string NamespaceName = "http://schemas.microsoft.com/winfx/2006/xaml";

        public static readonly XamlName ClassDirective = new XamlName("Class", NamespaceName);
        public static readonly XamlName NameDirective = new XamlName("Name", NamespaceName);
        public static readonly XamlName KeyDirective = new XamlName("Key", NamespaceName);
        public static readonly XamlName SharedDirective = new XamlName("Shared", NamespaceName);

        public static readonly XamlName NullTypeName = new XamlName("Null", NamespaceName);
        public static readonly XamlName TypeTypeName = new XamlName("Type", NamespaceName);

        public static bool IsDirective(string namespaceName, string localName)
        {
            return namespaceName == NamespaceName && (localName == ClassDirective.LocalName || localName == NameDirective.LocalName || localName == KeyDirective.LocalName || localName == SharedDirective.LocalName);
        }

        public static bool IsXamlType(string namespaceName, string localName)
        {
            return namespaceName == NamespaceName && (localName == NullTypeName.LocalName || localName == TypeTypeName.LocalName);
        }
    }
}
