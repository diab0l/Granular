using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection
{
    public class ParameterInfo
    {
        public Type ParameterType { get; private set; }

        public ParameterInfo(Type parameterType)
        {
            this.ParameterType = parameterType;
        }
    }

    public static class ParameterInfoExtensions
    {
        public static ParameterInfo[] GetParameters(this ConstructorInfo constructorInfo)
        {
            return constructorInfo.ParameterTypes.Select(type => new ParameterInfo(type)).ToArray();
        }

        public static ParameterInfo[] GetParameters(this MethodInfo methodInfo)
        {
            return methodInfo.ParameterTypes.Select(type => new ParameterInfo(type)).ToArray();
        }

        public static ParameterInfo[] GetIndexParameters(this PropertyInfo propertyInfo)
        {
            return propertyInfo.IndexParameterTypes.Select(type => new ParameterInfo(type)).ToArray();
        }
    }
}
