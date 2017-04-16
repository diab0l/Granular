using System;
using System.Collections.Generic;
using Bridge.Html5;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Host.Render
{
    public class HtmlRenderElement : IRenderItem
    {
        public HTMLElement HtmlElement { get; private set; }
        public HtmlStyleDictionary Style { get; private set; }

        private bool isRenderValid;
        private IRenderQueue renderQueue;

        public HtmlRenderElement(IRenderQueue renderQueue) :
            this(Document.CreateElement("div"), renderQueue)
        {
            //
        }

        public HtmlRenderElement(HTMLElement htmlElement, IRenderQueue renderQueue)
        {
            this.HtmlElement = htmlElement;
            this.renderQueue = renderQueue;

            Style = new HtmlStyleDictionary(HtmlElement);
            Style.Invalidated += (sender, e) => InvalidateRender();

            this.isRenderValid = true;
        }

        protected void InvalidateRender()
        {
            if (!isRenderValid)
            {
                return;
            }

            isRenderValid = false;
            renderQueue.Add(this);
        }

        public void Render()
        {
            isRenderValid = true;

            Style.Apply();

            OnRender();
        }

        protected virtual void OnRender()
        {
            //
        }
    }
}
