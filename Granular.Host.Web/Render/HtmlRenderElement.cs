using System;
using System.Collections.Generic;
using System.Html;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace Granular.Host.Render
{
    public class HtmlRenderElement : IRenderItem
    {
        public Element HtmlElement { get; private set; }
        public HtmlStyleDictionary Style { get; private set; }

        private bool isRenderValid;
        private IRenderQueue renderQueue;

        public HtmlRenderElement(IRenderQueue renderQueue) :
            this("div", String.Empty, renderQueue)
        {
            //
        }

        public HtmlRenderElement(string htmlElementTagName, IRenderQueue renderQueue) :
            this(htmlElementTagName, String.Empty, renderQueue)
        {
            //
        }

        public HtmlRenderElement(string htmlElementTagName, string htmlElementId, IRenderQueue renderQueue)
        {
            this.HtmlElement = Document.CreateElement(htmlElementTagName);
            this.renderQueue = renderQueue;

            if (!htmlElementId.IsNullOrEmpty())
            {
                this.HtmlElement.Id = htmlElementId;
            }

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
