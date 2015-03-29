using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Xaml
{
    public class XamlAttribute : XamlNode
    {
        public object Value { get; private set; }

        public XamlAttribute(XamlName name, XamlNamespaces namespaces, object value) :
            base(name, namespaces)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return String.Format("{0}={1}", base.ToString(), Value);
        }
    }
}
