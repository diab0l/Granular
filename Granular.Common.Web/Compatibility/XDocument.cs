using System;
using System.Collections.Generic;
using System.Linq;
using Bridge;
using Bridge.Html5;
using Granular.Extensions;

namespace System.Xml.Linq
{
    public abstract class XNode
    {
        //
    }

    public class XText : XNode
    {
        public string Value { get; set; }

        private Node node;

        public XText(Node node)
        {
            this.node = node;
            this.Value = node.NodeValue;
        }
    }

    public abstract class XContainer : XNode
    {
        private Node node;
        private IEnumerable<XNode> nodes;
        private IEnumerable<XElement> elements;

        public XContainer(Node node)
        {
            this.node = node;
            this.nodes = node.ChildNodes.TrySelect<Node, XNode>(XNodeFactory.TryCreateNode).ToArray();
            this.elements = nodes.OfType<XElement>().ToArray();
        }

        public IEnumerable<XNode> Nodes()
        {
            return nodes;
        }

        public IEnumerable<XElement> Elements()
        {
            return elements;
        }
    }

    public class XDocument : XContainer
    {
        public XElement Root { get; private set; }

        private Node node;

        public XDocument(Node node) :
            base(node)
        {
            this.node = node;
            this.Root = new XElement((Element)node.ChildNodes.Single());
        }

        public static XDocument Parse(string text)
        {
            DOMParser parser = new DOMParser();
            return new XDocument(parser.ParseFromString(text, SupportedType.ApplicationXml));
        }
    }

    public class XElement : XContainer
    {
        public XName Name { get; set; }

        private Element element;
        private IEnumerable<XAttribute> attributes;

        public XElement(Element element) :
            base(element)
        {
            this.element = element;
            this.Name = XName.Get(element.GetLocalName(), element.GetNamespaceURI());
            this.attributes = element.Attributes.Select(node => new XAttribute(node));
        }

        public IEnumerable<XAttribute> Attributes()
        {
            return attributes;
        }
    }

    public class XName
    {
        public string LocalName { get; private set; }
        public string NamespaceName { get; private set; }

        private XName(string localName, string namespaceName)
        {
            this.LocalName = localName;
            this.NamespaceName = namespaceName;
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(NamespaceName) ? LocalName : $"{{{NamespaceName}}}{LocalName}";
        }

        public static XName Get(string localName, string namespaceName)
        {
            return new XName(localName, namespaceName);
        }
    }

    public class XAttribute
    {
        public XName Name { get; private set; }
        public string Value { get; set; }

        public bool IsNamespaceDeclaration { get; private set; }

        private Node node;

        public XAttribute(Node node)
        {
            this.node = node;

            string nodeName = node.NodeName;

            if (nodeName == "xmlns")
            {
                this.Name = XName.Get(String.Empty, node.GetNamespaceURI());
                this.IsNamespaceDeclaration = true;
            }
            else
            {
                this.Name = XName.Get(node.GetLocalName(), node.GetNamespaceURI());
                this.IsNamespaceDeclaration = nodeName.StartsWith("xmlns:");
            }

            this.Value = node.NodeValue;
        }
    }

    public static class XNodeFactory
    {
        public static bool TryCreateNode(Node node, out XNode result)
        {
            if (node.NodeType == NodeType.Element)
            {
                result = new XElement((Element)node);
                return true;
            }

            if (node.NodeType == NodeType.Text && !node.NodeValue.IsNullOrWhiteSpace())
            {
                result = new XText(node);
                return true;
            }

            result = null;
            return false;
        }
    }

    public static class NodeExtensions
    {
        [Bridge.Template("{node}.namespaceURI")]
        public static extern string GetNamespaceURI(this Node node);

        public static string GetLocalName(this Node node)
        {
            return node.NodeName.Substring(node.NodeName.IndexOf(':') + 1);
        }
    }

    [External]
    [Enum(Emit.StringName)]
    public enum SupportedType
    {
        [Name("text/html")]
        TextHtml,
        [Name("text/xml")]
        TextXml,
        [Name("application/xml")]
        ApplicationXml,
        [Name("application/xhtml+xml")]
        ApplicationXhtmlXml,
        [Name("image/svg+xml")]
        ImageSvgXml
    };

    [External]
    [Name("DOMParser")]
    public class DOMParser
    {
        public virtual extern DocumentInstance ParseFromString(string str, SupportedType type);
    }
}