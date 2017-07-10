using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace System.Windows.Media
{
    [TypeConverter(typeof(ImageSourceTypeConverter))]
    public abstract class ImageSource : Animatable
    {
        public Size Size { get { return renderResource != null ? renderResource.Size : Size.Empty; } }
        public double Width { get { return Size.Width; } }
        public double Height { get { return Size.Width; } }

        private IImageSourceRenderResource renderResource;

        public object GetRenderResource(IRenderElementFactory factory)
        {
            if (renderResource == null)
            {
                object resource = CreateRenderResource(factory);
                OnRenderResourceCreated(resource);

                if (renderResource == null)
                {
                    throw new Granular.Exception("base.OnRenderResourceCreated was not called for \"{0}\"", GetType().Name);
                }
            }

            return renderResource;
        }

        protected abstract object CreateRenderResource(IRenderElementFactory factory);

        protected virtual void OnRenderResourceCreated(object renderResource)
        {
            this.renderResource = (IImageSourceRenderResource)renderResource;
        }
    }

    public class ImageSourceTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            return new BitmapImage
            {
                BaseUri = sourceUri,
                UriSource = Granular.Compatibility.Uri.CreateRelativeOrAbsoluteUri((string)value),
            };
        }
    }
}
