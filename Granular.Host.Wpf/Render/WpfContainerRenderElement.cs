extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfContainerRenderElement : IWpfRenderElement
    {
        public wpf::System.Windows.FrameworkElement WpfElement { get { return container; } }

        private wpf::System.Windows.Controls.Panel container;
        private List<IWpfRenderElement> children;
        public IEnumerable<object> Children { get { return children; } }

        public WpfContainerRenderElement(wpf::System.Windows.Controls.Panel container)
        {
            this.container = container;
            children = new List<IWpfRenderElement>();
        }

        public void InsertChild(int index, object child)
        {
            IWpfRenderElement childElement = (IWpfRenderElement)child;

            if (child is IWpfDeferredRenderElement)
            {
                ((IWpfDeferredRenderElement)child).WpfElementCreated += OnChildWpfElementCreated;
            }

            children.Insert(index, childElement);

            if (childElement.WpfElement != null)
            {
                index = children.Take(index).Count(c => c.WpfElement != null);
                container.Children.Insert(index, childElement.WpfElement);
            }
        }

        public void RemoveChild(object child)
        {
            IWpfRenderElement childElement = (IWpfRenderElement)child;

            if (child is IWpfDeferredRenderElement)
            {
                ((IWpfDeferredRenderElement)child).WpfElementCreated -= OnChildWpfElementCreated;
            }

            children.Remove(childElement);

            if (childElement.WpfElement != null)
            {
                container.Children.Remove(childElement.WpfElement);
            }
        }

        private void OnChildWpfElementCreated(object sender, EventArgs e)
        {
            IWpfRenderElement childElement = (IWpfRenderElement)sender;
            int index = children.IndexOf(childElement);

            index = children.Take(index).Count(c => c.WpfElement != null);
            container.Children.Insert(index, childElement.WpfElement);
        }
    }
}
