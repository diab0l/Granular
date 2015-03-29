using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Granular.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyInfo GetInstanceProperty(this Type type, string propertyName)
        {
            return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }

        public static PropertyInfo GetDefaultIndexProperty(this Type type)
        {
            return type.GetProperties().FirstOrDefault(property => property.GetIndexParameters().Any());
        }

        public static Type GetInterfaceType(this Type type, Type interfaceGenericType)
        {
            return type.GetInterfaces().FirstOrDefault(interfaceType => interfaceType == interfaceGenericType ||
                interfaceType.GetIsGenericType() && interfaceGenericType == interfaceType.GetGenericTypeDefinition());
        }
    }
}
