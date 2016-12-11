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
        public static XamlElement Parse(string content, Uri sourceUri = null)
        {
            return Parse(XDocument.Parse(content).Root, sourceUri);
        }

        private static XamlElement Parse(XElement element, Uri sourceUri = null)
        {
            var namespaces = new XamlNamespaces(GetNamespaces(element));
            var ignorableNamespaces = new XamlNamespaces(GetIgnorableNamespaces(element, namespaces));

            return CreateXamlElement(element, namespaces, ignorableNamespaces, sourceUri);
        }

        private static XamlElement CreateXamlElement(XElement element, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            return new XamlElement(new XamlName(element.Name.LocalName, element.Name.NamespaceName), namespaces, sourceUri, CreateXamlMembers(element, namespaces, ignorableNamespaces, sourceUri), CreateValues(element, namespaces, ignorableNamespaces, sourceUri), CreateDirectives(element, namespaces, ignorableNamespaces, sourceUri));
        }

        private static IEnumerable<XamlMember> CreateXamlMembers(XElement element, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            List<XamlMember> members = new List<XamlMember>(element.Attributes().Where(attribute => !IsDirective(attribute.Name) && !attribute.IsNamespaceDeclaration && !IsIgnorableAttribute(attribute) && !IsIgnorable(attribute.Name, ignorableNamespaces)).Select(attribute => CreateXamlMember(attribute, namespaces, ignorableNamespaces, sourceUri)));

            foreach (XElement memberElement in element.Elements())
            {
                if (!IsMemberName(memberElement.Name))
                {
                    continue;
                }

                XamlNamespaces memberNamespaces = namespaces.Merge(GetNamespaces(memberElement));
                XamlNamespaces memberIgnorableNamespaces = ignorableNamespaces.Merge(GetIgnorableNamespaces(memberElement, memberNamespaces));

                if (IsIgnorable(memberElement.Name, memberIgnorableNamespaces))
                {
                    continue;
                }

                members.Add(CreateXamlMember(memberElement, memberNamespaces, memberIgnorableNamespaces, sourceUri));
            }

            return members;
        }

        private static XamlMember CreateXamlMember(XAttribute attribute, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            XamlName name = new XamlName(attribute.Name.LocalName, attribute.Name.NamespaceName.IsNullOrEmpty() ? namespaces.GetDefaultNamespace() : attribute.Name.NamespaceName);
            object value = MarkupExtensionParser.Parse(attribute.Value, namespaces, sourceUri);

            return new XamlMember(name, namespaces, sourceUri, value);
        }

        private static XamlMember CreateXamlMember(XElement element, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            XamlName name = new XamlName(element.Name.LocalName, element.Name.NamespaceName.IsNullOrEmpty() ? namespaces.GetDefaultNamespace() : element.Name.NamespaceName);

            if (element.Attributes().Any(attribute => !IsIgnorable(attribute.Name, ignorableNamespaces)))
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain attributes", element.Name);
            }

            if (element.Elements().Any(child => !IsIgnorable(child.Name, ignorableNamespaces) && IsMemberName(child.Name)))
            {
                throw new Granular.Exception("Member \"{0}\" cannot contain member elements", element.Name);
            }

            return new XamlMember(name, namespaces, sourceUri, CreateValues(element, namespaces, ignorableNamespaces, sourceUri));
        }

        private static IEnumerable<XamlMember> CreateDirectives(XElement element, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            IEnumerable<XamlMember> attributeDirectives = element.Attributes().Where(attribute => IsDirective(attribute.Name)).Select(attribute => CreateXamlMember(attribute, namespaces, ignorableNamespaces, sourceUri));
            IEnumerable<XamlMember> elementDirectives = element.Elements().Where(child => IsDirective(child.Name)).Select(child => CreateXamlMember(child, namespaces, ignorableNamespaces, sourceUri));

            return attributeDirectives.Concat(elementDirectives).ToArray();
        }

        private static IEnumerable<object> CreateValues(XElement element, XamlNamespaces namespaces, XamlNamespaces ignorableNamespaces, Uri sourceUri)
        {
            List<object> values = new List<object>();

            foreach (XNode value in element.Nodes())
            {
                XText valueText = value as XText;
                if (valueText != null)
                {
                    values.Add(valueText.Value.Trim());
                }

                XElement valueElement = value as XElement;
                if (valueElement != null)
                {
                    if (!IsValueName(valueElement.Name))
                    {
                        continue;
                    }

                    XamlNamespaces valueNamespaces = namespaces.Merge(GetNamespaces(valueElement));
                    XamlNamespaces valueIgnorableNamespaces = ignorableNamespaces.Merge(GetIgnorableNamespaces(valueElement, valueNamespaces));

                    if (IsIgnorable(valueElement.Name, valueIgnorableNamespaces))
                    {
                        continue;
                    }

                    values.Add(CreateXamlElement(valueElement, valueNamespaces, valueIgnorableNamespaces, sourceUri));
                }
            }

            return values;
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
            return XamlLanguage.IsDirective(name.NamespaceName, name.LocalName);
        }

        private static bool IsIgnorable(XName name, XamlNamespaces ignorableNamespaces)
        {
            return ignorableNamespaces.ContainsNamespace(name.NamespaceName);
        }

        private static bool IsIgnorableAttribute(XAttribute attribute)
        {
            return attribute.Name.NamespaceName == MarkupCompatibility.IgnorableDirective.NamespaceName && attribute.Name.LocalName == MarkupCompatibility.IgnorableDirective.LocalName;
        }

        private static IEnumerable<NamespaceDeclaration> GetNamespaces(XElement element)
        {
            return element.Attributes().Where(attribute => attribute.IsNamespaceDeclaration).Select(attribute => new NamespaceDeclaration(GetNamespaceDeclarationPrefix(attribute), attribute.Value)).ToArray();
        }

        private static IEnumerable<NamespaceDeclaration> GetIgnorableNamespaces(XElement element, XamlNamespaces namespaces)
        {
            return element.Attributes().Where(IsIgnorableAttribute).SelectMany(attribute => attribute.Value.Split(' ')).Select(prefix => namespaces.GetNamespaceDeclaration(prefix)).ToArray();
        }

        private static string GetNamespaceDeclarationPrefix(XAttribute attribute)
        {
            return attribute.Name.NamespaceName.IsNullOrEmpty() ? String.Empty : attribute.Name.LocalName;
        }
    }
}
