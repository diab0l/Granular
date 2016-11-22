using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TemplateVisualStateAttribute : Attribute
    {
        public string GroupName { get; set; }
        public string Name { get; set; }

        public TemplateVisualStateAttribute()
        {
            //
        }

        public TemplateVisualStateAttribute(string groupName, string name) :
            this()
        {
            this.GroupName = groupName;
            this.Name = name;
        }
    }
}
