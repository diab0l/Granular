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

        protected override void OnLoad()
        {
            base.OnLoad();

            foreach (HtmlRenderElement child in Children)
            {
                child.Load();
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            foreach (HtmlRenderElement child in Children)
            {
                child.Unload();
            }
        }

        public void InsertChild(int index, object child)
        {
            HtmlRenderElement childElement = (HtmlRenderElement)child;

            if (IsLoaded)
            {
                childElement.Load();
            }

            children.Insert(index, childElement);
            renderQueue.InvokeAsync(() => HtmlElement.InsertChild(index, childElement.HtmlElement));
        }

        public void RemoveChild(object child)
        {
            HtmlRenderElement childElement = (HtmlRenderElement)child;

            if (IsLoaded)
            {
                childElement.Unload();
            }

            int childIndex = children.IndexOf(childElement);

            if (childIndex == -1)
            {
                return;
            }

            children.RemoveAt(childIndex);
            renderQueue.InvokeAsync(() => HtmlElement.RemoveChild(((HtmlRenderElement)child).HtmlElement));
        }
    }
}
