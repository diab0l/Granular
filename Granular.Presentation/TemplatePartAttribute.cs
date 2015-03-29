using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TemplatePartAttribute : Attribute
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public TemplatePartAttribute(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
