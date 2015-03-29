using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace System.Windows
{
    public interface IExpression : IObservableValue
    {
        bool SetValue(object value);
    }

    public interface IExpressionProvider
    {
        IExpression CreateExpression(DependencyObject dependencyObject, DependencyProperty dependencyProperty);
    }
}
