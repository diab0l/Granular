using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlVisualRenderElement : HtmlRenderElement, IVisualRenderElement
    {
        private Brush background;
        public Brush Background
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
                Style.SetBackground(background, converter);
                Style.SetIsHitTestVisible(IsHitTestVisible && background != null);

                if (background != null)
                {
                    background.Changed += OnBackgroundChanged;
                }
            }
        }

        private Rect bounds;
        public Rect Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds == value)
                {
                    return;
                }

                bounds = value;
                Style.SetBounds(bounds, converter);
            }
        }

        private bool clipToBounds;
        public bool ClipToBounds
        {
            get { return clipToBounds; }
            set
            {
                if (clipToBounds == value)
                {
                    return;
                }

                clipToBounds = value;
                Style.SetClipToBounds(clipToBounds);
            }
        }

        private bool isHitTestVisible;
        public bool IsHitTestVisible
        {
            get { return isHitTestVisible; }
            set
            {
                if (isHitTestVisible == value)
                {
                    return;
                }

                isHitTestVisible = value;
                Style.SetIsHitTestVisible(isHitTestVisible && Background != null);
            }
        }

        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible == value)
                {
                    return;
                }

                isVisible = value;
                Style.SetIsVisible(isVisible);
            }
        }

        private double opacity;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity == value)
                {
                    return;
                }

                opacity = value;
                Style.SetOpacity(opacity, converter);
            }
        }

        private Transform transform;
        public Transform Transform
        {
            get { return transform; }
            set
            {
                if (transform == value)
                {
                    return;
                }

                if (transform != null)
                {
                    transform.Changed -= OnTransformChanged;
                }

                transform = value;
                Style.SetTransform(transform, converter);

                if (transform != null)
                {
                    transform.Changed += OnTransformChanged;
                }
            }
        }

        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        private List<Action> childrenActions;

        private IHtmlValueConverter converter;

        public HtmlVisualRenderElement(object owner, IRenderQueue renderQueue, IHtmlValueConverter converter) :
            base(GetElementTagName(owner), GetElementId(owner), renderQueue)
        {
            this.converter = converter;

            this.children = new List<object>();
            this.childrenActions = new List<Action>();

            bounds = Rect.Zero;
            isVisible = true;
            opacity = 1;
            transform = Transform.Identity;

            Style.SetBackground(Background, converter);
            Style.SetBounds(Bounds, converter);
            Style.SetClipToBounds(ClipToBounds);
            Style.SetIsHitTestVisible(IsHitTestVisible && Background != null);
            Style.SetIsVisible(IsVisible);
            Style.SetOpacity(Opacity, converter);
            Style.SetTransform(Transform, converter);
        }

        protected override void OnRender()
        {
            foreach (Action action in childrenActions)
            {
                action();
            }

            childrenActions.Clear();
        }

        public void InsertChild(int index, object child)
        {
            if (!(child is HtmlRenderElement))
            {
                throw new Granular.Exception("Can't add child of type \"{0}\"", child.GetType().Name);
            }

            if (index < children.Count)
            {
                children.Insert(index, child);
                childrenActions.Add(() => HtmlElement.InsertBefore(((HtmlRenderElement)child).HtmlElement, HtmlElement.Children[index]));
            }
            else
            {
                children.Add(child);
                childrenActions.Add(() => HtmlElement.AppendChild(((HtmlRenderElement)child).HtmlElement));
            }

            InvalidateRender();
        }

        public void RemoveChild(object child)
        {
            if (!(child is HtmlRenderElement))
            {
                throw new Granular.Exception("Can't remove child of type \"{0}\"", child.GetType().Name);
            }

            int childIndex = children.IndexOf(child);

            if (childIndex != -1)
            {
                children.RemoveAt(childIndex);
                childrenActions.Add(() => HtmlElement.RemoveChild(((HtmlRenderElement)child).HtmlElement));
            }

            InvalidateRender();
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            Style.SetBackground(Background, converter);
        }

        private void OnTransformChanged(object sender, EventArgs e)
        {
            Style.SetTransform(Transform, converter);
        }

        private static string GetElementTagName(object target)
        {
            string typeName = target.GetType().Name.Replace('$', '_');
            return HtmlDefinition.Tags.Contains(typeName.ToLower()) ? String.Format("{0}_", typeName) : typeName;
        }

        private static string GetElementId(object target)
        {
            RuntimeNamePropertyAttribute nameAttribute = target.GetType().GetCustomAttributes(typeof(RuntimeNamePropertyAttribute), true).FirstOrDefault() as RuntimeNamePropertyAttribute;
            return nameAttribute != null ? (string)target.GetType().GetProperty(nameAttribute.Name).GetValue(target) : String.Empty;
        }
    }
}
