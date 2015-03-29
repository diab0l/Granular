using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public interface IStyleSelector
    {
        Style SelectStyle(object item, DependencyObject container);
    }
}
