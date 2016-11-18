using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Controls
{
    [DictionaryKeyProperty("Key")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class ControlTemplate : FrameworkTemplate
    {
        private Type targetType;
        public Type TargetType
        {
            get { return targetType; }
            set
            {
                if (targetType == value)
                {
                    return;
                }

                targetType = value;
            }
        }

        private object key;
        public object Key
        {
            get { return key ?? new TemplateKey(TargetType); }
            set { key = value; }
        }

        public ControlTemplate()
        {
            //
        }
    }
}
