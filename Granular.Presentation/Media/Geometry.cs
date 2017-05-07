using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows.Media
{
    [TypeConverter(typeof(GeometryTypeConverter))]
    public abstract class Geometry : Animatable
    {
        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register("Transform", typeof(Transform), typeof(Geometry), new FrameworkPropertyMetadata(Transform.Identity, (sender, e) => ((Geometry)sender).OnTransformChanged(e)));
        public Transform Transform
        {
            get { return (Transform)GetValue(TransformProperty); }
            set { SetValue(TransformProperty, value); }
        }

        private IGeometryRenderResource renderResource;

        public object GetRenderResource(IRenderElementFactory factory)
        {
            if (renderResource == null)
            {
                renderResource = factory.CreateGeometryRenderResource();
                renderResource.Transform = Transform;
                renderResource.Data = GetRenderResourceData();
            }

            return renderResource;
        }

        protected abstract string GetRenderResourceData();

        protected void InvalidateRenderResource()
        {
            if (renderResource != null)
            {
                renderResource.Data = GetRenderResourceData();
            }
        }

        private void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.Transform = Transform;
            }
        }

        public static Geometry Parse(string source)
        {
            return new StreamGeometry(source);
        }
    }

    public class GeometryTypeConverter : ITypeConverter
    {
        private ColorTypeConverter colorTypeConverter;

        public GeometryTypeConverter()
        {
            colorTypeConverter = new ColorTypeConverter();
        }

        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            if (value is string)
            {
                return new StreamGeometry((string)value);
            }

            throw new Granular.Exception("Can't convert \"{0}\" to Geometry", value);
        }
    }
}
