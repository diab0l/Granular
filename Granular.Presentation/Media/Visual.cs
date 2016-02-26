using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Granular.Extensions;

namespace System.Windows.Media
{
    public class Visual : DependencyObject
    {
        public event EventHandler VisualAncestorChanged;

        public event EventHandler VisualParentChanged;
        private Visual visualParent;
        public Visual VisualParent
        {
            get { return visualParent; }
            private set
            {
                if (visualParent == value)
                {
                    return;
                }

                if (visualParent != null)
                {
                    visualParent.VisualAncestorChanged -= OnVisualAncestorChanged;
                }

                Visual oldVisualParent = visualParent;
                visualParent = value;

                if (visualParent != null)
                {
                    visualParent.VisualAncestorChanged += OnVisualAncestorChanged;
                }

                OnVisualParentChanged(oldVisualParent, visualParent);
                VisualParentChanged.Raise(this);

                OnVisualAncestorChanged();
                VisualAncestorChanged.Raise(this);
            }
        }

        private List<Visual> visualChildren;
        public ReadOnlyCollection<Visual> VisualChildren { get; private set; }

        public Point VisualOffset { get { return VisualBounds.Location; } }
        public Size VisualSize { get { return VisualBounds.Size; } }

        private Brush visualBackground;
        protected Brush VisualBackground
        {
            get { return visualBackground; }
            set
            {
                if (visualBackground == value)
                {
                    return;
                }

                visualBackground = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.Background = visualBackground;
                }
            }
        }

        public event EventHandler VisualBoundsChanged;
        private Rect visualBounds;
        public Rect VisualBounds
        {
            get { return visualBounds; }
            protected set
            {
                if (visualBounds == value)
                {
                    return;
                }

                visualBounds = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.Bounds = visualBounds;
                }

                InvalidateHitTestBounds();

                OnVisualBoundsChanged();
                VisualBoundsChanged.Raise(this);
            }
        }

        private bool visualClipToBounds;
        protected bool VisualClipToBounds
        {
            get { return visualClipToBounds; }
            set
            {
                if (visualClipToBounds == value)
                {
                    return;
                }

                visualClipToBounds = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.ClipToBounds = visualClipToBounds;
                }

                InvalidateHitTestBounds();
            }
        }

        private bool visualIsHitTestVisible;
        protected bool VisualIsHitTestVisible
        {
            get { return visualIsHitTestVisible; }
            set
            {
                if (visualIsHitTestVisible == value)
                {
                    return;
                }

                visualIsHitTestVisible = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.IsHitTestVisible = visualIsHitTestVisible;
                }
            }
        }

        private bool visualIsVisible;
        protected bool VisualIsVisible
        {
            get { return visualIsVisible; }
            set
            {
                if (visualIsVisible == value)
                {
                    return;
                }

                visualIsVisible = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.IsVisible = visualIsVisible;
                }
            }
        }

        private double visualOpacity;
        protected double VisualOpacity
        {
            get { return visualOpacity; }
            set
            {
                if (visualOpacity == value)
                {
                    return;
                }

                visualOpacity = value;

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.Opacity = visualOpacity;
                }
            }
        }

        public event EventHandler VisualTransformChanged;
        private Transform visualTransform;
        public Transform VisualTransform
        {
            get { return visualTransform; }
            private set
            {
                if (visualTransform == value)
                {
                    return;
                }

                if (visualTransform != null)
                {
                    visualTransform.Changed -= OnVisualTransformValueChanged;
                }

                visualTransform = value;

                if (visualTransform != null)
                {
                    visualTransform.Changed += OnVisualTransformValueChanged;
                }

                foreach (IVisualRenderElement visualRenderElement in visualRenderElements.Values)
                {
                    visualRenderElement.Transform = visualTransform;
                }

                InvalidateHitTestBounds();

                OnVisualTransformChanged();
                VisualTransformChanged.Raise(this);
            }
        }

        private int visualLevel;
        public int VisualLevel
        {
            get
            {
                if (visualLevel == -1)
                {
                    visualLevel = VisualParent != null ? VisualParent.VisualLevel + 1 : 0;
                }

                return visualLevel;
            }
        }

        private Dictionary<IRenderElementFactory, IVisualRenderElement> visualRenderElements;
        private bool containsContentRenderElement;

        private Rect hitTestBounds;
        private bool isHitTestBoundsValid;

        public Visual()
        {
            visualChildren = new List<Visual>();
            VisualChildren = new ReadOnlyCollection<Visual>(visualChildren);

            visualRenderElements = new Dictionary<IRenderElementFactory, IVisualRenderElement>();

            VisualBackground = null;
            VisualBounds = Rect.Zero;
            VisualClipToBounds = true;
            VisualIsHitTestVisible = true;
            VisualIsVisible = true;
            VisualOpacity = 1;
            VisualTransform = Transform.Identity;

            visualLevel = -1;
        }

        public void AddVisualChild(Visual child)
        {
            if (child.VisualParent == this)
            {
                return;
            }

            if (child.VisualParent != null)
            {
                child.VisualParent.RemoveVisualChild(child);
            }

            child.VisualParent = this;
            visualChildren.Add(child);

            int renderChildIndex = containsContentRenderElement ? visualChildren.Count : visualChildren.Count - 1;
            foreach (IRenderElementFactory factory in visualRenderElements.Keys)
            {
                visualRenderElements[factory].InsertChild(renderChildIndex, child.GetRenderElement(factory));
            }

            InvalidateHitTestBounds();
        }

        public void RemoveVisualChild(Visual child)
        {
            if (child.VisualParent != this)
            {
                return;
            }

            visualChildren.Remove(child);
            child.VisualParent = null;

            foreach (IRenderElementFactory factory in visualRenderElements.Keys)
            {
                visualRenderElements[factory].RemoveChild(child.GetRenderElement(factory));
            }

            InvalidateHitTestBounds();
        }

        public void SetVisualChildIndex(Visual child, int newIndex)
        {
            int oldIndex = visualChildren.IndexOf(child);
            if (oldIndex == -1 || oldIndex == newIndex)
            {
                return;
            }

            visualChildren.Remove(child);
            visualChildren.Insert(newIndex, child);

            foreach (IRenderElementFactory factory in visualRenderElements.Keys)
            {
                object childRenderElement = child.GetRenderElement(factory);

                visualRenderElements[factory].RemoveChild(childRenderElement);
                visualRenderElements[factory].InsertChild(newIndex, childRenderElement);
            }
        }

        public void ClearVisualChildren()
        {
            foreach (Visual child in visualChildren.ToArray())
            {
                RemoveVisualChild(child);
            }
        }

        protected virtual void OnVisualParentChanged(Visual oldVisualParent, Visual newVisualParent)
        {
            //
        }

        protected virtual void OnVisualAncestorChanged()
        {
            visualLevel = -1;
        }

        private void OnVisualAncestorChanged(object sender, EventArgs e)
        {
            OnVisualAncestorChanged();
            VisualAncestorChanged.Raise(this);
        }

        public IVisualRenderElement GetRenderElement(IRenderElementFactory factory)
        {
            IVisualRenderElement visualRenderElement;
            if (visualRenderElements.TryGetValue(factory, out visualRenderElement))
            {
                return visualRenderElement;
            }

            visualRenderElement = factory.CreateVisualRenderElement(this);

            visualRenderElement.Background = VisualBackground;
            visualRenderElement.Bounds = VisualBounds;
            visualRenderElement.ClipToBounds = VisualClipToBounds;
            visualRenderElement.IsHitTestVisible = VisualIsHitTestVisible;
            visualRenderElement.IsVisible = VisualIsVisible;
            visualRenderElement.Opacity = VisualOpacity;
            visualRenderElement.Transform = VisualTransform;

            int index = 0;
            foreach (Visual child in VisualChildren)
            {
                child.GetRenderElement(factory);
                visualRenderElement.InsertChild(index, child.GetRenderElement(factory));
                index++;
            }

            object contentRenderElement = CreateContentRenderElementOverride(factory);
            if (contentRenderElement != null)
            {
                visualRenderElement.InsertChild(0, contentRenderElement);
            }

            if (visualRenderElements.Count == 0)
            {
                containsContentRenderElement = contentRenderElement != null;
            }
            else if (containsContentRenderElement != (contentRenderElement != null))
            {
                throw new Granular.Exception("ContentRenderElement for type \"{0}\" must be created for all of the factories or none of them", GetType().Name);
            }

            visualRenderElements.Add(factory, visualRenderElement);
            return visualRenderElement;
        }

        public void RemoveRenderElement(IRenderElementFactory factory)
        {
            visualRenderElements.Remove(factory);

            foreach (Visual child in VisualChildren)
            {
                child.RemoveRenderElement(factory);
            }
        }

        protected virtual object CreateContentRenderElementOverride(IRenderElementFactory factory)
        {
            return null;
        }

        protected virtual void OnVisualBoundsChanged()
        {
            //
        }

        protected virtual void OnVisualTransformChanged()
        {
            //
        }

        private void OnVisualTransformValueChanged(object sender, EventArgs e)
        {
            InvalidateHitTestBounds();

            OnVisualTransformChanged();
            VisualTransformChanged.Raise(this);
        }

        protected void InvalidateVisualTransform()
        {
            VisualTransform = GetVisualTransformOverride();
        }

        protected virtual Transform GetVisualTransformOverride()
        {
            return Transform.Identity;
        }

        public Point PointToRoot(Point point)
        {
            return point * TransformToAncestor(null);
        }

        public Point PointFromRoot(Point point)
        {
            return point * TransformToAncestor(null).Inverse;
        }

        public Matrix TransformToAncestor(Visual ancestor)
        {
            Matrix transformMatrix = !VisualTransform.IsNullOrIdentity() ? VisualTransform.Value : Matrix.Identity;
            Matrix offsetMatrix = VisualOffset != Point.Zero ? Matrix.TranslationMatrix(VisualOffset.X, VisualOffset.Y) : Matrix.Identity;
            Matrix parentMatrix = VisualParent != null && VisualParent != ancestor ? VisualParent.TransformToAncestor(ancestor) : Matrix.Identity;

            Matrix value = transformMatrix * offsetMatrix * parentMatrix;
            return value;
        }

        protected void InvalidateHitTestBounds()
        {
            if (!isHitTestBoundsValid)
            {
                return;
            }

            isHitTestBoundsValid = false;
            if (VisualParent != null)
            {
                VisualParent.InvalidateHitTestBounds();
            }
        }

        protected Rect GetHitTestBounds()
        {
            if (!isHitTestBoundsValid)
            {
                hitTestBounds = GetHitTestBoundsOverride();
                isHitTestBoundsValid = true;
            }

            return hitTestBounds;
        }

        protected virtual Rect GetHitTestBoundsOverride()
        {
            Rect bounds = new Rect(VisualBounds.Size);

            if (!VisualClipToBounds)
            {
                foreach (Visual child in VisualChildren)
                {
                    bounds = bounds.Union(child.GetHitTestBounds());
                }
            }

            return bounds.Transform(VisualTransform.Value).AddOffset(VisualBounds.Location);
        }
    }

    public static class VisualExtensions
    {
        public static bool IsAncestorOf(this Visual visual, Visual descendant)
        {
            while (descendant != null)
            {
                if (descendant.VisualParent == visual)
                {
                    return true;
                }

                descendant = descendant.VisualParent;
            }

            return false;
        }
    }

    // hold a reference to a Visual as long as it's a descendant of ancestor
    public class VisualWeakReference : IDisposable
    {
        public Visual Visual { get; private set; }

        private Visual ancestor;

        public VisualWeakReference(Visual visual, Visual ancestor)
        {
            this.Visual = visual;
            this.ancestor = ancestor;

            visual.VisualAncestorChanged += OnVisualAncestorChanged;
        }

        public void Dispose()
        {
            if (Visual != null)
            {
                Visual.VisualAncestorChanged -= OnVisualAncestorChanged;
                Visual = null;
            }
        }

        private void OnVisualAncestorChanged(object sender, EventArgs e)
        {
            if (!ancestor.IsAncestorOf(Visual))
            {
                Dispose();
            }
        }
    }
}
