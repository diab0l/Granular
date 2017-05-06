extern alias wpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Granular.Extensions;

namespace Granular.Host.Wpf.Render
{
    public class WpfDrawingRenderElement : IWpfDeferredRenderElement, IContainerRenderElement
    {
        public event EventHandler WpfElementCreated;
        public wpf::System.Windows.FrameworkElement WpfElement { get { return container?.WpfElement; } }

        public IEnumerable<object> Children { get { return container != null ? container.Children : new object[0]; } }

        private WpfContainerRenderElement container;

        public void InsertChild(int index, object child)
        {
            if (container == null)
            {
                container = new WpfContainerRenderElement(new wpf::System.Windows.Controls.Canvas());
                WpfElementCreated.Raise(this);
            }

            container.InsertChild(index, child);
        }

        public void RemoveChild(object child)
        {
            if (container != null)
            {
                container.RemoveChild(child);
            }
        }
    }
}
