using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Granular.Extensions;

namespace System.Windows.Controls
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class Adorner : FrameworkElement
    {
        private class VisualPathTransform : Transform, IDisposable
        {
            private Matrix value;
            public override Matrix Value { get { return value; } }

            private Visual visual;
            private Visual ancestor;
            private IEnumerable<Visual> visualPath;

            public VisualPathTransform(Visual visual, Visual ancestor)
            {
                this.visual = visual;
                this.ancestor = ancestor;

                visualPath = visual.GetVisualPath(ancestor).ToArray();

                foreach (Visual element in visualPath)
                {
                    element.VisualTransformChanged += OnVisualTransformChanged;
                }

                SetValue();
            }

            public void Dispose()
            {
                foreach (Visual element in visualPath)
                {
                    element.VisualTransformChanged -= OnVisualTransformChanged;
                }
            }

            private void OnVisualTransformChanged(object sender, EventArgs e)
            {
                SetValue();
            }

            public void SetValue()
            {
                Matrix newValue = visual.TransformToAncestor(ancestor);

                if (value != newValue)
                {
                    value = newValue;
                    RaiseChanged();
                }
            }
        }

        private UIElement child;
        public UIElement Child
        {
            get { return child; }
            set
            {
                if (child == value)
                {
                    return;
                }

                if (child != null)
                {
                    RemoveLogicalChild(child);
                    RemoveVisualChild(child);
                }

                child = value;

                if (child != null)
                {
                    AddLogicalChild(child);
                    AddVisualChild(child);
                }

                InvalidateMeasure();
            }
        }

        public UIElement AdornedElement { get; private set; }
        private VisualPathTransform visualPathTransform;

        public Adorner(UIElement adornedElement)
        {
            this.AdornedElement = adornedElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Child == null)
            {
                return Size.Zero;
            }

            Child.Measure(AdornedElement.RenderSize);
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                Child.Arrange(new Rect(finalSize));
            }

            visualPathTransform.SetValue();

            return finalSize;
        }

        protected override void OnVisualParentChanged(Visual oldVisualParent, Visual newVisualParent)
        {
            base.OnVisualParentChanged(oldVisualParent, newVisualParent);

            if (oldVisualParent != null)
            {
                visualPathTransform.Dispose();
                RenderTransform = Transform.Identity;
            }

            if (newVisualParent != null)
            {
                visualPathTransform = new VisualPathTransform(AdornedElement, newVisualParent.VisualParent);
                RenderTransform = visualPathTransform;
            }
        }

        public void Arrange()
        {
            Arrange(new Rect(AdornedElement.RenderSize));
            visualPathTransform.SetValue();
        }
    }
}
