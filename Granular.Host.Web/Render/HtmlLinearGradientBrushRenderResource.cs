using System;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlLinearGradientBrushRenderResource : HtmlGradientBrushRenderResource, ILinearGradientBrushRenderResource
    {
        private Point startPoint;
        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                if (startPoint == value)
                {
                    return;
                }

                startPoint = value;
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgStartPoint(startPoint, converter));
            }
        }

        private Point endPoint;
        public Point EndPoint
        {
            get { return endPoint; }
            set
            {
                if (endPoint == value)
                {
                    return;
                }

                endPoint = value;
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgEndPoint(endPoint, converter));
            }
        }

        private RenderQueue renderQueue;
        private SvgValueConverter converter;

        public HtmlLinearGradientBrushRenderResource(RenderQueue renderQueue, SvgValueConverter converter, SvgDefinitionContainer svgDefinitionContainer) :
            base("linearGradient", renderQueue, converter, svgDefinitionContainer)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;
        }
    }
}
