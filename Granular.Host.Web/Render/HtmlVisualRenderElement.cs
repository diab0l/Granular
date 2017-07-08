using Granular.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Bridge.Html5;
using Granular.Extensions;

namespace Granular.Host.Render
{
    public class HtmlVisualRenderElement : HtmlRenderElement, IVisualRenderElement
    {
        private static readonly CacheDictionary<Type, string> ElementTagNameCache = CacheDictionary<Type, string>.CreateUsingStringKeys(ResolveElementTagName, type => type.FullName);

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
                Style.SetBackground(background, new Rect(Bounds.Size), converter);
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
                Style.SetBackground(background, new Rect(Bounds.Size), converter);
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

        private Matrix transform;
        public Matrix Transform
        {
            get { return transform; }
            set
            {
                if (transform == value)
                {
                    return;
                }

                transform = value;
                Style.SetTransform(transform, converter);
            }
        }

        private object content;
        public object Content
        {
            get { return content; }
            set
            {
                if (content == value)
                {
                    return;
                }

                if (content != null)
                {
                    renderQueue.InvokeAsync(() =>
                    {
                        RemoveChildElement(((HtmlRenderElement)content).HtmlElement);
                        childrenStartIndex--;
                    });
                }

                content = value;

                if (content != null)
                {
                    renderQueue.InvokeAsync(() =>
                    {
                        InsertChildElement(0, ((HtmlRenderElement)content).HtmlElement);
                        childrenStartIndex++;
                    });
                }
            }
        }

        private int childrenStartIndex;
        private List<object> children;
        public IEnumerable<object> Children { get { return children; } }

        private RenderQueue renderQueue;
        private IHtmlValueConverter converter;

        public HtmlVisualRenderElement(object owner, RenderQueue renderQueue, IHtmlValueConverter converter) :
            base(CreateHtmlElement(owner), renderQueue)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            this.children = new List<object>();

            bounds = Rect.Zero;
            isVisible = true;
            opacity = 1;
            transform = Matrix.Identity;

            Style.SetBounds(Bounds, converter);
            Style.SetClipToBounds(ClipToBounds);
            Style.SetIsHitTestVisible(IsHitTestVisible && Background != null);
            Style.SetIsVisible(IsVisible);
            Style.SetOpacity(Opacity, converter);
            Style.SetTransform(Transform, converter);
        }

        public void InsertChild(int index, object child)
        {
            if (!(child is HtmlRenderElement))
            {
                throw new Granular.Exception("Can't add child of type \"{0}\"", child.GetType().Name);
            }

            children.Insert(index, child);

            renderQueue.InvokeAsync(() => InsertChildElement(index + childrenStartIndex, ((HtmlRenderElement)child).HtmlElement));
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
                renderQueue.InvokeAsync(() => HtmlElement.RemoveChild(((HtmlRenderElement)child).HtmlElement));
            }
        }

        private void InsertChildElement(int index, HTMLElement child)
        {
            if (index < HtmlElement.ChildElementCount)
            {
                HtmlElement.InsertBefore(child, HtmlElement.Children[index]);
            }
            else
            {
                HtmlElement.AppendChild(child);
            }
        }

        private void RemoveChildElement(HTMLElement child)
        {
            HtmlElement.RemoveChild(child);
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            Style.SetBackground(background, new Rect(Bounds.Size), converter);
        }

        private static string GetElementTagName(object target)
        {
            return ElementTagNameCache.GetValue(target.GetType());
        }

        private static string ResolveElementTagName(Type type)
        {
            string typeName = type.Name.Replace('$', '_');
            return HtmlDefinition.Tags.Contains(typeName.ToLower()) ? String.Format("{0}_", typeName) : typeName;
        }

        private static string GetElementId(object target)
        {
            RuntimeNamePropertyAttribute nameAttribute = target.GetType().GetCustomAttributes(typeof(RuntimeNamePropertyAttribute), true).FirstOrDefault() as RuntimeNamePropertyAttribute;
            return nameAttribute != null ? (string)target.GetType().GetProperty(nameAttribute.Name).GetValue(target) : String.Empty;
        }

        private static HTMLElement CreateHtmlElement(object owner)
        {
            HTMLElement htmlElement = Document.CreateElement(GetElementTagName(owner));

            string htmlElementId = GetElementId(owner);
            if (!htmlElementId.IsNullOrEmpty())
            {
                htmlElement.Id = htmlElementId;
            }

            return htmlElement;
        }
    }
}
