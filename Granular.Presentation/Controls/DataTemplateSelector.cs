using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public interface IDataTemplateSelector
    {
        DataTemplate SelectTemplate(object item, DependencyObject container);
    }
}
