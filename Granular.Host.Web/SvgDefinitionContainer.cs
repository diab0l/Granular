using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using Granular.Host.Render;

namespace Granular.Host
{
    public class SvgDefinitionContainer
    {
        public HTMLElement HtmlElement { get; private set; }

        private RenderQueue renderQueue;
        private HTMLElement definitionsElement;
        private int id;

        public SvgDefinitionContainer(RenderQueue renderQueue)
        {
            this.renderQueue = renderQueue;

            HtmlElement = SvgDocument.CreateElement("svg");
            HtmlElement.Style.SetProperty("overflow", "hidden");
            HtmlElement.Style.Width = "0px";
            HtmlElement.Style.Height = "0px";

            definitionsElement = SvgDocument.CreateElement("defs");
            HtmlElement.AppendChild(definitionsElement);
        }

        public int GetNextId()
        {
            id++;
            return id;
        }

        public void Add(HtmlRenderResource svgDefinition)
        {
            renderQueue.InvokeAsync(() => definitionsElement.AppendChild(svgDefinition.HtmlElement));
        }

        public void Remove(HtmlRenderResource svgDefinition)
        {
            renderQueue.InvokeAsync(() => definitionsElement.RemoveChild(svgDefinition.HtmlElement));
        }
    }
}
