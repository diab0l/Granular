extern alias wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Host.Wpf.Render
{
    public class WpfVisualRenderElement : System.Windows.Media.IVisualRenderElement, IWpfRenderElement
    {
        private System.Windows.Media.Brush background;
        public System.Windows.Media.Brush Background
        {
            get { return background; }
            set
            {
                if (background == value)
                {
                    return;
                }

                if (background != null)
                {
                    background.Changed -= OnBackgroundChanged;
                }

                background = value;
                container.Background = converter.Convert(background);

                if (background != null)
                {
                    background.Changed += OnBackgroundChanged;
                }
            }
        }

        private System.Windows.Rect bounds;
        public System.Windows.Rect Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                wpf::System.Windows.Controls.Canvas.SetLeft(container, bounds.Left);
                wpf::System.Windows.Controls.Canvas.SetTop(container, bounds.Top);
                container.Width = bounds.Width;
                container.Height = bounds.Height;
            }
        }

        public bool ClipToBounds
        {
            get { return container.ClipToBounds; }
            set { container.ClipToBounds = value; }
        }

        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                isHitTestVisible = value;
                container.IsHitTestVisible = isHitTestVisible;
            }
        }

        public bool IsVisible
        {
            get { return container.Visibility == wpf::System.Windows.Visibility.Visible; }
            set { container.Visibility = value ? wpf::System.Windows.Visibility.Visible : wpf::System.Windows.Visibility.Collapsed; }
        }

        public double Opacity
        {
            get { return container.Opacity; }
            set { container.Opacity = value; }
        }

        public System.Windows.Media.Transform Transform { get; set; }

        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        public wpf::System.Windows.FrameworkElement WpfElement { get { return container; } }

        private wpf::System.Windows.Controls.Canvas container;
        private IWpfValueConverter converter;

        public WpfVisualRenderElement(object owner, IWpfValueConverter converter)
        {
            this.converter = converter;
            children = new List<object>();
            container = new wpf::System.Windows.Controls.Canvas { Name = owner.GetType().Name };
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

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            container.Background = converter.Convert(Background);
        }
    }
}
