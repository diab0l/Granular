using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Xaml
{
    public sealed class NamespaceDeclaration
    {
        public string Prefix { get; private set; }
        public string Namespace { get; private set; }

        public NamespaceDeclaration(string @namespace) :
            this(string.Empty, @namespace)
        {
            //
        }

        public NamespaceDeclaration(string prefix, string @namespace)
        {
            this.Prefix = prefix;
            this.Namespace = @namespace;
        }

        public override bool Equals(object obj)
        {
            NamespaceDeclaration other = obj as NamespaceDeclaration;

            return Object.ReferenceEquals(this, other) || !Object.ReferenceEquals(other, null) &&
                this.Prefix == other.Prefix && this.Namespace == other.Namespace;
        }

        public override int GetHashCode()
        {
            return Prefix.GetHashCode() ^ Prefix.GetHashCode();
        }
    }

    public class XamlNamespaces
    {
        public static readonly XamlNamespaces Empty = new XamlNamespaces(new NamespaceDeclaration[0]);

        private IEnumerable<NamespaceDeclaration> items;

        public XamlNamespaces(string @namespace) :
            this(new[] { new NamespaceDeclaration(@namespace) })
        {
            //
        }

        public XamlNamespaces(string prefix, string @namespace) :
            this(new[] { new NamespaceDeclaration(prefix, @namespace) })
        {
            //
        }

        public XamlNamespaces(IEnumerable<NamespaceDeclaration> items)
        {
            this.items = items;
        }

        public override string ToString()
        {
            int count = items.Count();
            return count == 0 ? "XamlNamespaces.Empty" : String.Format("XamlNamespaces[{0}]", count);
        }

        public bool Contains(string prefix)
        {
            return items.Any(item => item.Prefix == prefix);
        }

        public string Get(string prefix)
        {
            NamespaceDeclaration namespaceDeclaration = items.FirstOrDefault(item => item.Prefix == prefix);

            if (namespaceDeclaration == null)
            {
                throw new Granular.Exception("Namespaces doesn't contain a namespace with prefix \"{0}\"", prefix);
            }

            return namespaceDeclaration.Namespace;
        }

        public XamlNamespaces Merge(IEnumerable<NamespaceDeclaration> namespaceDeclarations)
        {
            return new XamlNamespaces(items.Concat(namespaceDeclarations).Distinct().ToArray());
        }
    }

    public static class XamlNamespacesExtensions
    {
        public static bool ContainsDefault(this XamlNamespaces @this)
        {
            return @this.Contains(String.Empty);
        }

        public static string GetDefault(this XamlNamespaces @this)
        {
            return @this.Get(String.Empty);
        }
    }
}
