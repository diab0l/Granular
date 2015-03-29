using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace System.Windows.Data
{
    public enum RelativeSourceMode
    {
        //PreviousData,
        TemplatedParent,
        Self,
        FindAncestor,
    }

    [MarkupExtensionParameter("Mode")]
    public class RelativeSource : IMarkupExtension
    {
        public RelativeSourceMode Mode { get; set; }
        public int AncestorLevel { get; set; }
        public Type AncestorType { get; set; }

        public object ProvideValue(InitializeContext context)
        {
            return this;
        }
    }
}
