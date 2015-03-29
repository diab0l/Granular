using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Markup
{
    public interface IMarkupExtension
    {
        object ProvideValue(InitializeContext context);
    }
}
