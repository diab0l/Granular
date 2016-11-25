using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Markup
{
    public class XamlNode
    {
        public XamlName Name { get; private set; }
        public XamlNamespaces Namespaces { get; private set; }
        public Uri SourceUri { get; private set; }

        public XamlNode(XamlName name, XamlNamespaces namespaces, Uri sourceUri)
        {
            this.Name = name;
            this.Namespaces = namespaces;
            this.SourceUri = sourceUri;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
