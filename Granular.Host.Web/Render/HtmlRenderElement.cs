using System;
using System.Collections.Generic;
using Bridge.Html5;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Host.Render
{
    public class HtmlRenderElement
    {
        public HTMLElement HtmlElement { get; private set; }
        public HtmlStyleDictionary Style { get; private set; }

        private RenderQueue renderQueue;

        public HtmlRenderElement(RenderQueue renderQueue) :
            this(Document.CreateElement("div"), renderQueue)
        {
            //
        }

        public HtmlRenderElement(HTMLElement htmlElement, RenderQueue renderQueue)
        {
            this.HtmlElement = htmlElement;

            Style = new HtmlStyleDictionary(HtmlElement);
            Style.Invalidated += (sender, e) => renderQueue.InvokeAsync(() => Style.Apply());
        }
    }
}
