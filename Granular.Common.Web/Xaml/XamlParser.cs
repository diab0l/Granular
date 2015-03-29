using System;
using System.Collections;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Text;
using System.Xml;
using Granular.Extensions;

namespace System.Xaml
{
    public static class XmlElementExtensions
    {
        private class XmlEnumerable<T> : IEnumerable<T>
        {
            private Func<IEnumerator<T>> getEnumerator;

            public XmlEnumerable(Func<IEnumerator<T>> getEnumerator)
            {
                this.getEnumerator = getEnumerator;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return getEnumerator();
            }

            [System.Runtime.CompilerServices.Reflectable(false)]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IEnumerable<XmlNode> Nodes(this XmlElement xmlElement)
        {
            return new XmlEnumerable<XmlNode>(xmlElement.ChildNodes.GetEnumerator);
        }

        public static IEnumerable<XmlElement> Elements(this XmlElement xmlElement)
        {
            return xmlElement.Nodes().OfType<XmlElement>();
        }

        public static IEnumerable<XmlAttribute> Attributes(this XmlElement xmlElement)
        {
            return new XmlEnumerable<XmlAttribute>(xmlElement.Attributes.GetEnumerator);
        }
    }

    public static class XamlParser
    {
        public static XamlElement Parse(string content)
        {
            DOMParser domParser = new DOMParser();
            XmlElement xmlElement = domParser.ParseFromString(content, DOMParserSupportedType.ApplicationXml).DocumentElement;

            if (xmlElement.NodeName == "parsererror" || xmlElement.FirstChild != null && xmlElement.FirstChild.NodeName == "parsererror")
            {
                throw new Exception(GetParserErrorMessage(xmlElement.TextContent));
            }

            return Parse(xmlElement);
        }

        private static XamlElement Parse(XmlElement element)
        {
            return CreateXamlElement(element, XamlNamespaces.Empty);
        }

        private static XamlElement CreateXamlElement(XmlElement root, XamlNamespaces namespaces)
        {
            if (root.Nodes().OfType<XmlText>().Where(text => !text.Value.IsNullOrWhitespace()).Count() > 1)
            {
                throw new Granular.Exception("Xml cannot contain more than one text node");
            }

            IEnumerable<NamespaceDeclaration> elementNamespaces = root.Attributes().Where(attribute => IsNamespaceDeclaration(attribute)).Select(attribute => new NamespaceDeclaration(GetNamespaceDeclarationPrefix(attribute), attribute.Value)).ToArray();
            if (elementNamespaces.Any())
            {
                namespaces = namespaces.Merge(elementNamespaces);
            }

            IEnumerable<XamlAttribute> attributes = root.Attributes().Where(attribute => !IsNamespaceDeclaration(attribute)).Select(attribute => CreateXamlAttribute(attribute, namespaces)).ToArray();
            IEnumerable<XamlElement> elements = root.Elements().Select(element => CreateXamlElement(element, namespaces)).ToArray();

            string textValue = root.Nodes().OfType<XmlText>().Where(text => !text.Value.IsNullOrWhitespace()).Select(text => text.Value.Trim()).DefaultIfEmpty(String.Empty).First();

            return new XamlElement(new XamlName(root.LocalName, root.NamespaceURI), namespaces, attributes, elements, textValue);
        }

        private static bool IsNamespaceDeclaration(XmlAttribute attribute)
        {
            string name = attribute.Name.ToLower();
            return name == "xmlns" || name.StartsWith("xmlns:");
        }

        private static XamlAttribute CreateXamlAttribute(XmlAttribute attribute, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(attribute.LocalName, attribute.NamespaceURI.IsNullOrEmpty() ? namespaces.Get(String.Empty) : attribute.NamespaceURI);
            object value = (object)MarkupExtensionParser.Parse(attribute.Value, namespaces);
            return new XamlAttribute(name, namespaces, value);
        }

        private static string GetNamespaceDeclarationPrefix(XmlAttribute attribute)
        {
            return attribute.Prefix.IsNullOrEmpty() ? String.Empty : attribute.LocalName;
        }

        private static string GetParserErrorMessage(string textContent)
        {
            string errorMessage = textContent.Replace(Environment.NewLine, " ");

            if (errorMessage.EndsWith("^"))
            {
                errorMessage = errorMessage.Substring(0, errorMessage.LastIndexOf(" "));
            }

            if (!errorMessage.ToLower().StartsWith("xml"))
            {
                errorMessage = String.Format("Xml parser error: {0}", errorMessage);
            }

            return errorMessage;
        }
    }
}
