using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Xaml
{
    public sealed class XamlName
    {
        public static readonly XamlName Empty = new XamlName(String.Empty);

        public string LocalName { get; private set; }
        public string NamespaceName { get; private set; }

        public bool IsMemberName { get; private set; }
        public string MemberName { get; private set; }
        public XamlName ContainingTypeName { get; private set; }

        public bool IsEmpty { get { return LocalName.IsNullOrEmpty(); } }

        public XamlName(string localName, string namespaceName = null)
        {
            this.LocalName = localName ?? String.Empty;
            this.NamespaceName = namespaceName ?? String.Empty;

            int typeSeparatorIndex = LocalName.IndexOf('.');

            if (typeSeparatorIndex != -1)
            {
                MemberName = LocalName.Substring(typeSeparatorIndex + 1);
                ContainingTypeName = new XamlName(LocalName.Substring(0, typeSeparatorIndex), NamespaceName);

                IsMemberName = true;
            }
            else
            {
                MemberName = LocalName;
            }
        }

        public override string ToString()
        {
            return NamespaceName.IsNullOrEmpty() ? LocalName : String.Format("{0}:{1}", NamespaceName, LocalName);
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

            int namespaceSeparatorIndex = prefixedName.IndexOf(":");
            if (namespaceSeparatorIndex != -1)
            {
                typeNamespacePrefix = prefixedName.Substring(0, namespaceSeparatorIndex);
                typeName = prefixedName.Substring(namespaceSeparatorIndex + 1);
            }

            return namespaces.Contains(typeNamespacePrefix) ? new XamlName(typeName, namespaces.Get(typeNamespacePrefix)) : XamlName.Empty;
        }
    }
}
