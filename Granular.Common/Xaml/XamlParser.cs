using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Granular.Extensions;

namespace System.Windows.Markup
{
    public static class XamlParser
    {
        public static XamlElement Parse(string content)
        {
            return Parse(XDocument.Parse(content).Root);
        }

        private static XamlElement Parse(XElement element)
        {
            return CreateXamlElement(element, XamlNamespaces.Empty);
        }

        private static XamlElement CreateXamlElement(XElement element, XamlNamespaces namespaces)
        {
            IEnumerable<NamespaceDeclaration> elementNamespaces = element.Attributes().Where(attribute => attribute.IsNamespaceDeclaration).Select(attribute => new NamespaceDeclaration(GetNamespaceDeclarationPrefix(attribute), attribute.Value)).ToArray();
            if (elementNamespaces.Any())
            {
                namespaces = namespaces.Merge(elementNamespaces);
            }

            return new XamlElement(new XamlName(element.Name.LocalName, element.Name.NamespaceName), namespaces, CreateXamlMembers(element, namespaces), CreateValues(element, namespaces), CreateDirectives(element, namespaces));
        }

        private static IEnumerable<XamlMember> CreateXamlMembers(XElement element, XamlNamespaces namespaces)
        {
            IEnumerable<XamlMember> attributeMembers = element.Attributes().Where(attribute => !IsDirective(attribute.Name) && !attribute.IsNamespaceDeclaration).Select(attribute => CreateXamlMember(attribute, namespaces));
            IEnumerable<XamlMember> elementMembers = element.Elements().Where(child => IsMemberName(child.Name)).Select(child => CreateXamlMember(child, namespaces));

            return attributeMembers.Concat(elementMembers).ToArray();
        }

        private static XamlMember CreateXamlMember(XAttribute attribute, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(attribute.Name.LocalName, attribute.Name.NamespaceName.IsNullOrEmpty() ? namespaces.Get(String.Empty) : attribute.Name.NamespaceName);
            object value = (object)MarkupExtensionParser.Parse(attribute.Value, namespaces);

            return new XamlMember(name, namespaces, value);
        }

        private static XamlMember CreateXamlMember(XElement element, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(element.Name.LocalName, element.Name.NamespaceName.IsNullOrEmpty() ? namespaces.Get(String.Empty) : element.Name.NamespaceName);

            if (element.Attributes().Any())
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain attributes", element.Name);
            }

            if (element.Elements().Any(child => IsMemberName(child.Name)))
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain member elements", element.Name);
            }

            return new XamlMember(name, namespaces, CreateValues(element, namespaces));
        }

        private static IEnumerable<XamlMember> CreateDirectives(XElement element, XamlNamespaces namespaces)
        {
            IEnumerable<XamlMember> attributeDirectives = element.Attributes().Where(attribute => IsDirective(attribute.Name) && !attribute.IsNamespaceDeclaration).Select(attribute => CreateXamlMember(attribute, namespaces));
            IEnumerable<XamlMember> elementDirectives = element.Elements().Where(child => IsDirective(child.Name)).Select(child => CreateXamlMember(child, namespaces));

            return attributeDirectives.Concat(elementDirectives).ToArray();
        }

        private static IEnumerable<object> CreateValues(XElement element, XamlNamespaces namespaces)
        {
            return element.Nodes().Where(node => IsValue(node)).Select(node => CreateValue(node, namespaces)).ToArray();
        }

        private static bool IsValue(XNode node)
        {
            return node is XText || node is XElement && IsValueName(((XElement)node).Name);
        }

        private static object CreateValue(XNode node, XamlNamespaces namespaces)
        {
            if (node is XText)
            {
                return ((XText)node).Value.Trim();
            }

            if (node is XElement)
            {
                return CreateXamlElement((XElement)node, namespaces);
            }

            throw new Granular.Exception("Node \"{0}\" doesn't contain a value", node);
        }

        private static bool IsMemberName(XName name)
        {
            return name.LocalName.Contains('.') && !IsDirective(name);
        }

        private static bool IsValueName(XName name)
        {
            return !name.LocalName.Contains('.') && !IsDirective(name);
        }

        private static bool IsDirective(XName name)
        {
            return XamlLanguage.NamespaceName == name.NamespaceName &&
                XamlLanguage.IsDirective(new XamlName(name.LocalName, name.NamespaceName));
        }

        private static string GetNamespaceDeclarationPrefix(XAttribute attribute)
        {
            return attribute.Name.NamespaceName.IsNullOrEmpty() ? String.Empty : attribute.Name.LocalName;
        }
    }
}
