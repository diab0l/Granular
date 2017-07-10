using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlImageRenderElement : HtmlRenderElement, IImageRenderElement
    {
        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                renderQueue.InvokeAsync(() =>
                {
                    HtmlElement.SetHtmlBounds(bounds, converter);
                    SetSourceRect();
                });
            }
        }

        private HtmlImageSourceRenderResource renderResource;
        private ImageSource source;
        public ImageSource Source
        {
            get { return source; }
            set
            {
                if (source == value)
                {
                    return;
                }

                source = value;
                renderResource = source != null ? (HtmlImageSourceRenderResource)source.GetRenderResource(factory) : null;

                renderQueue.InvokeAsync(() =>
                {
                    HtmlElement.SetHtmlBackgroundImage(renderResource?.Url, converter, factory);
                    SetSourceRect();
                });
            }
        }

        private IRenderElementFactory factory;
        private RenderQueue renderQueue;
        private HtmlValueConverter converter;

        public HtmlImageRenderElement(IRenderElementFactory factory, RenderQueue renderQueue, HtmlValueConverter converter)
        {
            this.factory = factory;
            this.renderQueue = renderQueue;
            this.converter = converter;

            bounds = Rect.Zero;

            HtmlElement.SetHtmlBounds(Bounds, converter);
        }

        private void SetSourceRect()
        {
            if (renderResource == null || Bounds.IsNullOrEmpty() || Bounds.Size.Width == 0 || Bounds.Size.Height == 0)
            {
                return;
            }

            if (!renderResource.SourceRect.IsNullOrEmpty())
            {
                double widthFactor = Bounds.Size.Width / renderResource.SourceRect.Width;
                double heightFactor = Bounds.Size.Height / renderResource.SourceRect.Height;

                Point location = new Point(-renderResource.SourceRect.Left * widthFactor, -renderResource.SourceRect.Top * heightFactor);
                Size size = new Size(renderResource.ImageSize.Width * widthFactor, renderResource.ImageSize.Height * heightFactor);

                HtmlElement.SetHtmlBackgroundBounds(new Rect(location, size), converter);
            }
            else
            {
                HtmlElement.SetHtmlBackgroundBounds(new Rect(Bounds.Size), converter);
            }
        }
    }
}
