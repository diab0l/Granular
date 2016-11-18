using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    [DictionaryKeyProperty("Key")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class DataTemplate : FrameworkTemplate
    {
        private Type dataType;
        public Type DataType
        {
            get { return dataType; }
            set
            {
                if (dataType == value)
                {
                    return;
                }

                dataType = value;
            }
        }

        private object key;
        public object Key
        {
            get { return key ?? new TemplateKey(DataType); }
            set { key = value; }
        }

        public DataTemplate()
        {
            //
        }
    }
}
