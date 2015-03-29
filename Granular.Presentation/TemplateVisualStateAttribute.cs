using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TemplateVisualStateAttribute : Attribute
    {
        public string GroupName { get; private set; }
        public string Name { get; private set; }

        public TemplateVisualStateAttribute(string groupName, string name)
        {
            this.GroupName = groupName;
            this.Name = name;
        }
    }
}
