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
                throw new Granular.Exception(GetParserErrorMessage(xmlElement.TextContent));
            }

            return Parse(xmlElement);
        }

        private static XamlElement Parse(XmlElement element)
        {
            return CreateXamlElement(element, XamlNamespaces.Empty);
        }

        private static XamlElement CreateXamlElement(XmlElement element, XamlNamespaces namespaces)
        {
            if (element.Nodes().OfType<XmlText>().Where(text => !text.Value.IsNullOrWhitespace()).Count() > 1)
            {
                throw new Granular.Exception("Element \"{0}\" cannot contain more than one text node", element.LocalName);
            }

            IEnumerable<NamespaceDeclaration> elementNamespaces = element.Attributes().Where(IsNamespaceDeclaration).Select(attribute => new NamespaceDeclaration(GetNamespaceDeclarationPrefix(attribute), attribute.Value)).ToArray();
            if (elementNamespaces.Any())
            {
                namespaces = namespaces.Merge(elementNamespaces);
            }

            return new XamlElement(new XamlName(element.LocalName, element.NamespaceURI), namespaces, CreateXamlMembers(element, namespaces), CreateValues(element, namespaces), CreateDirectives(element, namespaces));
        }

        private static IEnumerable<XamlMember> CreateXamlMembers(XmlElement element, XamlNamespaces namespaces)
        {
            IEnumerable<XamlMember> attributeMembers = element.Attributes().Where(attribute => !IsDirective(attribute) && !IsNamespaceDeclaration(attribute)).Select(attribute => CreateXamlMember(attribute, namespaces));
            IEnumerable<XamlMember> elementMembers = element.Elements().Where(child => IsMemberName(child)).Select(child => CreateXamlMember(child, namespaces));

            return attributeMembers.Concat(elementMembers).ToArray();
        }

        private static XamlMember CreateXamlMember(XmlAttribute attribute, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(attribute.LocalName, attribute.NamespaceURI.IsNullOrEmpty() ? namespaces.Get(String.Empty) : attribute.NamespaceURI);
            object value = (object)MarkupExtensionParser.Parse(attribute.Value, namespaces);

            return new XamlMember(name, namespaces, value);
        }

        private static XamlMember CreateXamlMember(XmlElement element, XamlNamespaces namespaces)
        {
            XamlName name = new XamlName(element.LocalName, element.NamespaceURI.IsNullOrEmpty() ? namespaces.Get(String.Empty) : element.NamespaceURI);

            if (element.Attributes().Any())
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain attributes", element.LocalName);
            }

            if (element.Elements().Any(child => IsMemberName(child)))
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain member elements", element.LocalName);
            }

            if (element.Nodes().OfType<XmlText>().Where(text => !text.Value.IsNullOrWhitespace()).Count() > 1)
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain more than one text node", element.LocalName);
            }

            return new XamlMember(name, namespaces, CreateValues(element, namespaces));
        }

        private static IEnumerable<XamlMember> CreateDirectives(XmlElement element, XamlNamespaces namespaces)
        {
            IEnumerable<XamlMember> attributeDirectives = element.Attributes().Where(attribute => IsDirective(attribute) && !IsNamespaceDeclaration(attribute)).Select(attribute => CreateXamlMember(attribute, namespaces));
            IEnumerable<XamlMember> elementDirectives = element.Elements().Where(child => IsDirective(child)).Select(child => CreateXamlMember(child, namespaces));

            return attributeDirectives.Concat(elementDirectives).ToArray();
        }

        private static IEnumerable<object> CreateValues(XmlElement element, XamlNamespaces namespaces)
        {
            IEnumerable<object> elementValues = element.Elements().Where(child => IsValueName(child)).Select(child => (object)CreateXamlElement(child, namespaces));
            IEnumerable<object> textValues = element.Nodes().OfType<XmlText>().Where(text => !text.Value.IsNullOrWhitespace()).Select(textValue => textValue.Value.Trim());

            return elementValues.Concat(textValues).ToArray();
        }

        private static bool IsMemberName(XmlElement element)
        {
            return element.LocalName.Contains(".") && !IsDirective(element);
        }

        private static bool IsValueName(XmlElement element)
        {
            return !element.LocalName.Contains(".") && !IsDirective(element);
        }

        private static bool IsDirective(XmlAttribute attribute)
        {
            return XamlLanguage.NamespaceName == attribute.NamespaceURI &&
                XamlLanguage.IsDirective(new XamlName(attribute.LocalName, attribute.NamespaceURI));
        }

        private static bool IsDirective(XmlElement element)
        {
            return XamlLanguage.NamespaceName == element.NamespaceURI &&
                XamlLanguage.IsDirective(new XamlName(element.LocalName, element.NamespaceURI));
        }

        private static bool IsNamespaceDeclaration(XmlAttribute attribute)
        {
            string name = attribute.Name.ToLower();
            return name == "xmlns" || name.StartsWith("xmlns:");
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
