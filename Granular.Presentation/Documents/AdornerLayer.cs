using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows.Documents
{
    public interface IAdornerLayerHost
    {
        AdornerLayer AdornerLayer { get; }
    }

    public class AdornerLayer : FrameworkElement
    {
        protected override void OnVisualParentChanged(Visual oldVisualParent, Visual newVisualParent)
        {
            base.OnVisualParentChanged(oldVisualParent, newVisualParent);

            if (oldVisualParent != null)
            {
                ((UIElement)oldVisualParent).LayoutUpdated -= OnParentLayoutUpdated;
            }

            if (newVisualParent != null)
            {
                ((UIElement)newVisualParent).LayoutUpdated += OnParentLayoutUpdated;
            }
        }

        private void OnParentLayoutUpdated(object sender, EventArgs e)
        {
            foreach (Adorner adorner in VisualChildren)
            {
                adorner.Arrange();
            }
        }

        public void Add(Adorner adorner)
        {
            AddLogicalChild(adorner);
            AddVisualChild(adorner);

            InvalidateArrange();
        }

        public void Remove(Adorner adorner)
        {
            RemoveVisualChild(adorner);
            RemoveLogicalChild(adorner);
        }

        public static AdornerLayer GetAdornerLayer(Visual visual)
        {
            while (visual != null)
            {
                if (visual is IAdornerLayerHost)
                {
                    return ((IAdornerLayerHost)visual).AdornerLayer;
                }

                visual = visual.VisualParent;
            }

            return null;
        }
    }
}
