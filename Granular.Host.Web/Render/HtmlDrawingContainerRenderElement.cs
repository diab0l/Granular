using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Granular.Compatibility.Linq;
using Granular.Extensions;
using Bridge.Html5;

namespace Granular.Host.Render
{
    public class HtmlDrawingContainerRenderElement : HtmlContainerRenderElement, IDrawingContainerRenderElement
    {
        private double opacity;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity == value)
                {
                    return;
                }

                opacity = value;
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgOpacity(opacity, converter));
            }
        }

        private RenderQueue renderQueue;
        private SvgValueConverter converter;

        public HtmlDrawingContainerRenderElement(RenderQueue renderQueue, SvgValueConverter converter) :
            base(SvgDocument.CreateElement("g"), renderQueue)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;
        }
    }
}
