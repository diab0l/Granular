using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Windows.Markup
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class XmlnsDefinitionAttribute : Attribute
    {
        public string XmlNamespace { get; private set; }
        public string ClrNamespace { get; private set; }
        public string AssemblyName { get; private set; }

        public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace, string assemblyName = null)
        {
            this.XmlNamespace = xmlNamespace;
            this.ClrNamespace = clrNamespace;
            this.AssemblyName = assemblyName;
        }
    }
}
