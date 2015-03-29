using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Granular.Extensions;

namespace System.Xaml
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

        private static XamlElement CreateXamlElement(XElement root, XamlNamespaces namespaces)
        {
            if (root.Nodes().OfType<XText>().Count() > 1)
            {
                throw new Granular.Exception("Xml cannot contain more than one text node");
            }

            IEnumerable<NamespaceDeclaration> elementNamespaces = root.Attributes().Where(attribute => attribute.IsNamespaceDeclaration).Select(attribute => new NamespaceDeclaration(GetNamespaceDeclarationPrefix(attribute), attribute.Value)).ToArray();
            if (elementNamespaces.Any())
            {
                namespaces = namespaces.Merge(elementNamespaces);
            }

            IEnumerable<XamlAttribute> attributes = root.Attributes().Where(attribute => !attribute.IsNamespaceDeclaration).Select(attribute => CreateXamlAttribute(attribute, namespaces)).ToArray();
            IEnumerable<XamlElement> elements = root.Elements().Select(element => CreateXamlElement(element, namespaces)).ToArray();

            string textValue = root.Nodes().OfType<XText>().Select(text => text.Value.Trim()).DefaultIfEmpty(String.Empty).First();

            return new XamlElement(new XamlName(root.Name.LocalName, root.Name.NamespaceName), namespaces, attributes, elements, textValue);
        }

        private static XamlAttribute CreateXamlAttribute(XAttribute attribute, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(attribute.Name.LocalName, attribute.Name.NamespaceName.IsNullOrEmpty() ? namespaces.Get(String.Empty) : attribute.Name.NamespaceName);
            object value = (object)MarkupExtensionParser.Parse(attribute.Value, namespaces);
            return new XamlAttribute(name, namespaces, value);
        }

        private static string GetNamespaceDeclarationPrefix(XAttribute attribute)
        {
            return attribute.Name.NamespaceName.IsNullOrEmpty() ? String.Empty : attribute.Name.LocalName;
        }
    }
}
