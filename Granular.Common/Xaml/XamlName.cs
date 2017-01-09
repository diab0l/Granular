using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public sealed class XamlName
    {
        public static readonly XamlName Empty = new XamlName(String.Empty);

        public string LocalName { get; private set; }
        public string NamespaceName { get; private set; }

        public string FullName { get; private set; }

        public string MemberName { get; private set; }

        public bool HasContainingTypeName { get; private set; }
        public string ContainingTypeName { get; private set; }

        public bool IsEmpty { get { return LocalName.IsNullOrEmpty(); } }

        public XamlName(string localName, string namespaceName = null)
        {
            this.LocalName = localName;
            this.NamespaceName = namespaceName ?? String.Empty;

            this.FullName = namespaceName == null ? localName : namespaceName + ":" + localName;

            int typeSeparatorIndex = localName.IndexOf('.');

            if (typeSeparatorIndex != -1)
            {
                MemberName = localName.Substring(typeSeparatorIndex + 1);
                ContainingTypeName = localName.Substring(0, typeSeparatorIndex);

                HasContainingTypeName = true;
            }
            else
            {
                MemberName = localName;
            }
        }

        public override string ToString()
        {
            return FullName;
        }

        public override int GetHashCode()
        {
            return LocalName.GetHashCode() ^ NamespaceName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            XamlName other = obj as XamlName;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                this.LocalName == other.LocalName &&
                this.NamespaceName == other.NamespaceName;
        }

        public static bool operator ==(XamlName name1, XamlName name2)
        {
            return Object.Equals(name1, name2);
        }

        public static bool operator !=(XamlName name1, XamlName name2)
        {
            return !(name1 == name2);
        }

        public static XamlName FromPrefixedName(string prefixedName, XamlNamespaces namespaces)
        {
            string typeName = prefixedName;
            string typeNamespacePrefix = String.Empty;

            int namespaceSeparatorIndex = prefixedName.IndexOf(':');
            if (namespaceSeparatorIndex != -1)
            {
                typeNamespacePrefix = prefixedName.Substring(0, namespaceSeparatorIndex);
                typeName = prefixedName.Substring(namespaceSeparatorIndex + 1);
            }

            return namespaces.ContainsPrefix(typeNamespacePrefix) ? new XamlName(typeName, namespaces.GetNamespace(typeNamespacePrefix)) : XamlName.Empty;
        }
    }
}
