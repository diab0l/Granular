using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Xaml
{
    public class XamlElement : XamlNode
    {
        public IEnumerable<XamlAttribute> Attributes { get; private set; }
        public IEnumerable<XamlElement> Children { get; private set; }
        public string TextValue { get; private set; }

        public XamlElement(XamlName name, XamlNamespaces namespaces, IEnumerable<XamlAttribute> attributes = null, IEnumerable<XamlElement> children = null, string textValue = null) :
            base(name, namespaces)
        {
            this.Attributes = attributes ?? new XamlAttribute[0];
            this.Children = children ?? new XamlElement[0];
            this.TextValue = textValue ?? String.Empty;
        }
    }

    public static class XamlElementExtensions
    {
        [System.Runtime.CompilerServices.Reflectable(false)]
        public static bool TryGetMemberValue(this XamlElement element, XamlName memberName, out object value)
        {
            XamlAttribute memberAttribute = element.Attributes.Where(attribute => attribute.Name == memberName).FirstOrDefault();
            if (memberAttribute != null)
            {
                value = memberAttribute.Value;
                return true;
            }

            XamlElement memberChild = element.Children.Where(child => child.Name == memberName).FirstOrDefault();
            if (memberChild != null)
            {
                value = memberChild.GetMemberValue();
                return true;
            }

            value = null;
            return false;
        }

        public static IEnumerable<XamlAttribute> GetMemberAttributes(this XamlElement element)
        {
            return element.Attributes.Where(attribute => !XamlLanguage.IsDirective(attribute.Name));
        }

        public static IEnumerable<XamlElement> GetMemberChildren(this XamlElement element)
        {
            return element.Children.Where(child => child.Name.IsMemberName); // member name cannot be directive
        }

        public static IEnumerable<XamlNode> GetMemberNodes(this XamlElement element)
        {
            return element.GetMemberAttributes().Cast<XamlNode>().Concat(element.GetMemberChildren());
        }

        public static IEnumerable<XamlElement> GetContentChildren(this XamlElement element)
        {
            return element.Children.Where(child => !child.Name.IsMemberName && !XamlLanguage.IsDirective(child.Name));
        }

        public static object GetMemberValue(this XamlElement element)
        {
            if (element.GetMemberChildren().Any())
            {
                throw new Granular.Exception("Element \"{0}\" cannot have attributes", element.Name.LocalName);
            }

            IEnumerable<XamlElement> children = element.GetContentChildren();

            if (!children.Any() && element.TextValue.IsNullOrEmpty())
            {
                throw new Granular.Exception("Element \"{0}\" doesn't have a value", element.Name.LocalName);
            }

            if (children.Count() > 1)
            {
                throw new Granular.Exception("Element \"{0}\" can only have one child", element.Name);
            }

            if (children.Any() && !element.TextValue.IsNullOrEmpty())
            {
                throw new Granular.Exception("Element \"{0}\" cannot have both children and text value", element.Name.LocalName);
            }

            return !element.TextValue.IsNullOrEmpty() ? (object)element.TextValue : children.First();
        }
    }
}
