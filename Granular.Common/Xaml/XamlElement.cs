using System;
using System.Collections.Generic;
using System.Linq;
using Granular.Extensions;

namespace System.Xaml
{
    public class XamlElement : XamlNode
    {
        private static readonly IEnumerable<XamlMember> EmptyMembers = new XamlMember[0];
        private static readonly IEnumerable<object> EmptyValues = new object[0];
        private static readonly IEnumerable<XamlMember> EmptyDirectives = new XamlMember[0];

        public IEnumerable<XamlMember> Members { get; private set; }
        public IEnumerable<object> Values { get; private set; }
        public IEnumerable<XamlMember> Directives { get; private set; }

        public XamlElement(XamlName name, XamlNamespaces namespaces, IEnumerable<XamlMember> members = null, IEnumerable<object> values = null, IEnumerable<XamlMember> directives = null) :
            base(name, namespaces)
        {
            this.Members = members ?? EmptyMembers;
            this.Values = values ?? EmptyValues;
            this.Directives = directives ?? EmptyDirectives;
        }
    }
}
