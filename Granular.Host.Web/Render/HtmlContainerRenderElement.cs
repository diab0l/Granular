using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridge.Html5;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlContainerRenderElement : HtmlRenderElement, IContainerRenderElement
    {
        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        private RenderQueue renderQueue;

        public HtmlContainerRenderElement(HTMLElement htmlElement, RenderQueue renderQueue) :
            base(htmlElement)
        {
            this.renderQueue = renderQueue;

            children = new List<object>();
        }

        public void InsertChild(int index, object child)
        {
            children.Insert(index, child);

            renderQueue.InvokeAsync(() => HtmlElement.InsertChild(index, ((HtmlRenderElement)child).HtmlElement));
        }

        public void RemoveChild(object child)
        {
            int childIndex = children.IndexOf(child);

            if (childIndex == -1)
            {
                return;
            }

            children.RemoveAt(childIndex);
            renderQueue.InvokeAsync(() => HtmlElement.RemoveChild(((HtmlRenderElement)child).HtmlElement));
        }
    }
}
