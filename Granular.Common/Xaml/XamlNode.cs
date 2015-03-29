using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Xaml
{
    public class XamlNode
    {
        public XamlName Name { get; private set; }
        public XamlNamespaces Namespaces { get; private set; }

        public XamlNode(XamlName name, XamlNamespaces namespaces)
        {
            this.Name = name;
            this.Namespaces = namespaces;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
