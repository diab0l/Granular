using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static bool IsDelegate(this PropertyInfo propertyInfo)
        {
            return typeof(Delegate).IsAssignableFrom(propertyInfo.PropertyType);
        }
    }
}
