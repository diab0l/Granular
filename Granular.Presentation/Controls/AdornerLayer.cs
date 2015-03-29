using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public interface IAdornerLayerHost
    {
        AdornerLayer AdornerLayer { get; }
    }

    public class AdornerLayer : FrameworkElement
    {
        private Dictionary<Adorner, Rect> adornersBounds;

        public AdornerLayer()
        {
            adornersBounds = new Dictionary<Adorner, Rect>();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (Adorner adorner in VisualChildren)
            {
                adorner.Measure(availableSize);
            }

            return Size.Zero;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (Adorner adorner in VisualChildren)
            {
                adorner.Arrange(adornersBounds[adorner]);
            }

            return finalSize;
        }

        public void Add(Adorner adorner)
        {
            AddLogicalChild(adorner);
            AddVisualChild(adorner);

            adornersBounds.Add(adorner, GetAdornerBounds(adorner));
            InvalidateArrange();
        }

        public void Remove(Adorner adorner)
        {
            RemoveVisualChild(adorner);
            RemoveLogicalChild(adorner);

            adornersBounds.Remove(adorner);
        }

        protected override void OnVisualParentChanged(Media.Visual oldVisualParent, Media.Visual newVisualParent)
        {
            base.OnVisualParentChanged(oldVisualParent, newVisualParent);

            if (oldVisualParent is UIElement)
            {
                ((UIElement)oldVisualParent).LayoutUpdated -= OnParentLayoutUpdated;
            }

            if (newVisualParent is UIElement)
            {
                ((UIElement)newVisualParent).LayoutUpdated += OnParentLayoutUpdated;
            }
        }

        private void OnParentLayoutUpdated(object sender, EventArgs e)
        {
            SetAdornersBounds();
        }

        private void SetAdornersBounds()
        {
            bool isInvalidated = false;

            foreach (Adorner adorner in VisualChildren)
            {
                Rect newBounds = GetAdornerBounds(adorner);
                Rect oldBounds;

                if (!adornersBounds.TryGetValue(adorner, out oldBounds) || !newBounds.IsClose(oldBounds))
                {
                    adornersBounds[adorner] = newBounds;
                    isInvalidated = true;
                }
            }

            if (isInvalidated)
            {
                InvalidateArrange();
                UpdateLayout();
            }
        }

        private Rect GetAdornerBounds(Adorner adorner)
        {
            return new Rect(PointFromRoot(adorner.AdornedElement.PointToRoot(Point.Zero)), adorner.AdornedElement.VisualSize);
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
