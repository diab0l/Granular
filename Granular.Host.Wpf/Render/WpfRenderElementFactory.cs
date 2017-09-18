using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Wpf.Render
{
    public class WpfRenderElementFactory : IRenderElementFactory
    {
        private WpfValueConverter wpfValueConverter;

        public WpfRenderElementFactory(WpfValueConverter wpfValueConverter)
        {
            this.wpfValueConverter = wpfValueConverter;
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new WpfVisualRenderElement(owner, this, wpfValueConverter);
        }

        public IContainerRenderElement CreateDrawingRenderElement(object owner)
        {
            return new WpfDrawingRenderElement();
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new WpfTextBoxRenderElement(this, wpfValueConverter);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new WpfTextBlockRenderElement(this, wpfValueConverter);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new WpfBorderRenderElement(this, wpfValueConverter);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new WpfImageRenderElement(this, wpfValueConverter);
        }

        public IDrawingContainerRenderElement CreateDrawingContainerRenderElement()
        {
            return new WpfDrawingContainerRenderElement(wpfValueConverter);
        }

        public IDrawingGeometryRenderElement CreateDrawingGeometryRenderElement()
        {
            return new WpfDrawingGeometryRenderElement(this, wpfValueConverter);
        }

        public IDrawingImageRenderElement CreateDrawingImageRenderElement()
        {
            return new WpfDrawingImageElement(this, wpfValueConverter);
        }

        public ISolidColorBrushRenderResource CreateSolidColorBrushRenderResource()
        {
            return new WpfSolidColorBrushRenderResource(wpfValueConverter);
        }

        public ILinearGradientBrushRenderResource CreateLinearGradientBrushRenderResource()
        {
            return new WpfLinearGradientBrushRenderResource(wpfValueConverter);
        }

        public IRadialGradientBrushRenderResource CreateRadialGradientBrushRenderResource()
        {
            return new WpfRadialGradientBrushRenderResource(wpfValueConverter);
        }

        public IImageBrushRenderResource CreateImageBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public IImageSourceRenderResource CreateImageSourceRenderResource()
        {
            return new WpfImageSourceRenderResource();
        }

        public ITransformRenderResource CreateTransformRenderResource()
        {
            return new WpfTransformRenderResource(wpfValueConverter);
        }

        public IGeometryRenderResource CreateGeometryRenderResource()
        {
            return new WpfGeometryRenderResource(this, wpfValueConverter);
        }
    }
}
