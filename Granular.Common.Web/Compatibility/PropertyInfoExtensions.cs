using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetMethod;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo propertyInfo)
        {
            return propertyInfo.SetMethod;
        }

        public static bool IsDelegate(this PropertyInfo propertyInfo)
        {
            return !propertyInfo.Name.EndsWith("Type") && typeof(Delegate).IsAssignableFrom(propertyInfo.PropertyType);
        }
    }
}
