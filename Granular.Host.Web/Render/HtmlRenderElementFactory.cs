using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlRenderElementFactory : IRenderElementFactory
    {
        private RenderQueue renderQueue;
        private HtmlValueConverter htmlValueConverter;
        private ImageElementContainer imageElementContainer;
        private EmbeddedResourceObjectFactory embeddedResourceObjectFactory;
        private SvgValueConverter svgValueConverter;
        private SvgDefinitionContainer svgDefinitionContainer;

        public HtmlRenderElementFactory(RenderQueue renderQueue, HtmlValueConverter htmlValueConverter, ImageElementContainer imageElementContainer, EmbeddedResourceObjectFactory embeddedResourceObjectFactory, SvgValueConverter svgValueConverter, SvgDefinitionContainer svgDefinitionContainer)
        {
            this.renderQueue = renderQueue;
            this.htmlValueConverter = htmlValueConverter;
            this.imageElementContainer = imageElementContainer;
            this.embeddedResourceObjectFactory = embeddedResourceObjectFactory;
            this.svgValueConverter = svgValueConverter;
            this.svgDefinitionContainer = svgDefinitionContainer;
        }

        public IVisualRenderElement CreateVisualRenderElement(object owner)
        {
            return new HtmlVisualRenderElement(owner, this, renderQueue, htmlValueConverter);
        }

        public IDrawingRenderElement CreateDrawingRenderElement(object owner)
        {
            throw new NotImplementedException();
        }

        public ITextBoxRenderElement CreateTextBoxRenderElement(object owner)
        {
            return new HtmlTextBoxRenderElement(this, renderQueue, htmlValueConverter);
        }

        public ITextBlockRenderElement CreateTextBlockRenderElement(object owner)
        {
            return new HtmlTextBlockRenderElement(renderQueue, htmlValueConverter);
        }

        public IBorderRenderElement CreateBorderRenderElement(object owner)
        {
            return new HtmlBorderRenderElement(this, renderQueue, htmlValueConverter);
        }

        public IImageRenderElement CreateImageRenderElement(object owner)
        {
            return new HtmlImageRenderElement(this, renderQueue, htmlValueConverter);
        }

        public ISolidColorBrushRenderResource CreateSolidColorBrushRenderResource()
        {
            return new HtmlSolidColorBrushRenderResource(renderQueue, svgValueConverter, svgDefinitionContainer);
        }

        public ILinearGradientBrushRenderResource CreateLinearGradientBrushRenderResource()
        {
            return new HtmlLinearGradientBrushRenderResource(renderQueue, svgValueConverter, svgDefinitionContainer);
        }

        public IRadialGradientBrushRenderResource CreateRadialGradientBrushRenderResource()
        {
            return new HtmlRadialGradientBrushRenderResource(renderQueue, svgValueConverter, svgDefinitionContainer);
        }

        public IImageBrushRenderResource CreateImageBrushRenderResource()
        {
            throw new NotImplementedException();
        }

        public IImageSourceRenderResource CreateImageSourceRenderResource()
        {
            return new HtmlImageSourceRenderResource(embeddedResourceObjectFactory, imageElementContainer);
        }
    }
}
