using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Xaml;

namespace System.Windows.Media
{
    [TypeConverter(typeof(TransformTypeConverter))]
    public abstract class Transform : Animatable
    {
        private class IdentityTransform : Transform
        {
            public override Matrix Value { get { return Matrix.Identity; } }
        }

        public static readonly Transform Identity = new IdentityTransform();

        public abstract Matrix Value { get; }
    }

    public class TransformTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return new MatrixTransform { Matrix = Matrix.Parse(value.ToString()) };
        }
    }

    public static class TransformExtensions
    {
        public static bool IsNullOrIdentity(this Transform transform)
        {
            return ReferenceEquals(transform, null) || ReferenceEquals(transform, Transform.Identity);
        }
    }
}
