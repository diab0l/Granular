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
        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        public WpfContainerRenderElement(wpf::System.Windows.Controls.Panel container)
        {
            this.container = container;
            children = new List<object>();
        }

        public void InsertChild(int index, object child)
        {
            children.Insert(index, child);
            container.Children.Insert(index, ((IWpfRenderElement)child).WpfElement);
        }

        public void RemoveChild(object child)
        {
            children.Remove(child);
            container.Children.Remove(((IWpfRenderElement)child).WpfElement);
        }
    }
}
