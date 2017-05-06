using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Granular.Extensions;

namespace System.Windows.Media
{
    public abstract class DrawingContext
    {
        public abstract void Close();
    }

    public class RenderElementDrawingContext : DrawingContext
    {
        public event EventHandler Closed;

        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        public bool IsEmpty { get { return children.Count == 0; } }

        private IContainerRenderElement container;
        private IRenderElementFactory factory;
        private List<object> availableChildren;
        private RenderElementDrawingContext innerContext;
        private bool isClosed;

        public RenderElementDrawingContext(IContainerRenderElement container, IRenderElementFactory factory)
        {
            this.container = container;
            this.factory = factory;
            this.availableChildren = container.Children?.ToList();
            this.children = new List<object>();
        }

        private void SetInnerContext(IContainerRenderElement innerContainer)
        {
            if (innerContext != null)
            {
                throw new Granular.Exception("Inner context is already set");
            }

            innerContext = new RenderElementDrawingContext(innerContainer, factory);
            innerContext.Closed += (sender, e) => innerContext = null;
        }

        public override void Close()
        {
            VerifyNotClosed();

            if (innerContext != null)
            {
                innerContext.Close();
            }

            container.SetChildren(children);

            foreach (IDisposable child in availableChildren.OfType<IDisposable>())
            {
                child.Dispose();
            }

            isClosed = true;

            Closed.Raise(this);
        }

        private T GetChild<T>(Func<T> factory)
        {
            T child = availableChildren.OfType<T>().FirstOrDefault();

            if (child != null)
            {
                availableChildren.Remove(child);
            }
            else
            {
                child = factory();
            }

            children.Add(child);

            return child;
        }

        private void VerifyNotClosed()
        {
            if (isClosed)
            {
                throw new Granular.Exception("DrawingContext is closed");
            }
        }
    }
}