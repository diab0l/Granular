using System;
using System.Windows;
using System.Windows.Media;
using Bridge.Html5;

namespace Granular.Host.Render
{
    public class HtmlSolidColorBrushRenderResource : HtmlBrushRenderResource, ISolidColorBrushRenderResource
    {
        private Color color;
        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value)
                {
                    return;
                }

                color = value;
                renderQueue.InvokeAsync(SetStop);
            }
        }

        private RenderQueue renderQueue;
        private SvgValueConverter converter;
        private HTMLElement stopElement;

        public HtmlSolidColorBrushRenderResource(RenderQueue renderQueue, SvgValueConverter converter, SvgDefinitionContainer svgDefinitionContainer) :
            base("linearGradient", svgDefinitionContainer)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            stopElement = SvgDocument.CreateElement("stop");
            HtmlElement.AppendChild(stopElement);
        }

        protected override void OnOpacityChanged()
        {
            renderQueue.InvokeAsync(SetStop);
        }

        private void SetStop()
        {
            stopElement.SetAttribute("stop-color", converter.ToColorString(Color));
            stopElement.SetAttribute("stop-opacity", converter.ToImplicitValueString(Opacity * Color.A / 255));
        }
    }
}
