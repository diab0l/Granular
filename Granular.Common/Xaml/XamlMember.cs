using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Markup
{
    public class XamlMember : XamlNode
    {
        private static readonly IEnumerable<object> EmptyValues = new object[0];

        public IEnumerable<object> Values { get; private set; }

        public XamlMember(XamlName name, XamlNamespaces namespaces, object value) :
            this(name, namespaces, new object[] { value })
        {
            //
        }

        public XamlMember(XamlName name, XamlNamespaces namespaces, IEnumerable<object> values) :
            base(name, namespaces)
        {
            this.Values = values ?? EmptyValues;
        }

        public override string ToString()
        {
            return Values.Count() == 1 ? String.Format("{0}={1}", base.ToString(), Values.First().ToString()) : base.ToString();
        }
    }

    public static class XamlMemberExtensions
    {
        public static object GetSingleValue(this XamlMember member)
        {
            if (!member.Values.Any())
            {
                throw new Granular.Exception("Member \"{0}\" doesn't have values", member.Name);
            }

            if (member.Values.Count() > 1)
            {
                throw new Granular.Exception("Member \"{0}\" cannot have multiple values", member.Name);
            }

            return member.Values.First();
        }
    }
}
