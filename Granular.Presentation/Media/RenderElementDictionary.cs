using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Media
{
    public class RenderElementDictionary<T> where T : class
    {
        public int Count { get; private set; }
        public IRenderElementFactory[] Factories { get; private set; }
        public T[] Elements { get; private set; }

        private Func<IRenderElementFactory, T> createElement;
        private IRenderElementFactory factory;
        private T element;

        public RenderElementDictionary(Func<IRenderElementFactory, T> createElement)
        {
            this.createElement = createElement;
            this.Factories = new IRenderElementFactory[0];
            this.Elements = new T[0];
        }

        public T GetRenderElement(IRenderElementFactory factory)
        {
            if (this.factory == null)
            {
                this.factory = factory;
                this.element = createElement(factory);
                this.Factories = new IRenderElementFactory[] { factory };
                this.Elements = new T[] { element };
                this.Count = 1;
            }
            else if (this.factory != factory)
            {
                throw new Granular.Exception("Render element was already created for a different IRenderElementFactory");
            }

            return element;
        }

        public void RemoveRenderElement(IRenderElementFactory factory)
        {
            if (this.factory != factory)
            {
                return;
            }

            this.factory = null;
            this.element = null;
            this.Factories = new IRenderElementFactory[0];
            this.Elements = new T[0];
            this.Count = 0;
        }

        public void SetRenderElementsProperty(Action<T> setter)
        {
            if (element != null)
            {
                setter(element);
            }
        }
    }
}
