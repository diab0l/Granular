using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows
{
    [MarkupExtensionParameter("Property")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class TemplateBindingExtension : IMarkupExtension
    {
        public IPropertyPathElement Property { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return new Binding
            {
                Path = new PropertyPath(new [] { Property }),
                RelativeSource = new RelativeSource { Mode = RelativeSourceMode.TemplatedParent },
                Converter = Converter,
                ConverterParameter = ConverterParameter,
            };
        }
    }
}
