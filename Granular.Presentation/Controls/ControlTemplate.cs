using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Controls
{
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

        public ControlTemplate()
        {
            //
        }
    }
}
