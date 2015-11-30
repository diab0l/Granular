using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows.Documents
{
    [TypeConverter(typeof(InlineConverter))]
    public abstract class Inline : TextElement
    {
        public ObservableCollection<Inline> SiblingInlines { get; private set; }
    }

    public class InlineConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return new Run((string)value);
        }
    }
}
