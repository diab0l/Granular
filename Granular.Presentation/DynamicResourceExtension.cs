using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows
{
    [MarkupExtensionParameter("ResourceKey")]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class DynamicResourceExtension : IMarkupExtension
    {
        public object ResourceKey { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return new ResourceReferenceExpressionProvider(ResourceKey);
        }
    }
}
