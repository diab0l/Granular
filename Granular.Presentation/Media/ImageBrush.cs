using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public class ImageBrush : TileBrush
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageBrush), new FrameworkPropertyMetadata(null, (sender, e) => ((ImageBrush)sender).OnImageSourceChanged(e)));
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private IImageBrushRenderResource renderResource;

        protected override object CreateRenderResource(IRenderElementFactory factory)
        {
            return factory.CreateImageBrushRenderResource();
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (IImageBrushRenderResource)renderResource;
            this.renderResource.ImageSource = ImageSource;
        }

        private void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            renderResource.ImageSource = ImageSource;
        }
    }
}
