using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace System.Windows.Media
{
    [TypeConverter(typeof(TransformTypeConverter))]
    public abstract class Transform : Animatable
    {
        private class IdentityTransform : Transform
        {
            public override Matrix Value { get { return Matrix.Identity; } }
        }

        public static readonly Transform Identity = CreateIdentityTransform();

        public abstract Matrix Value { get; }

        private static Transform CreateIdentityTransform()
        {
            Transform identityTransform = new IdentityTransform();
            identityTransform.Freeze();
            return identityTransform;
        }
    }

    public class TransformTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
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
