using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Granular.Compatibility.Linq;
using Granular.Extensions;
using Bridge.Html5;

namespace Granular.Host.Render
{
    public class HtmlDrawingContainerRenderElement : HtmlContainerRenderElement, IDrawingContainerRenderElement
    {
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
                renderQueue.InvokeAsync(() => HtmlElement.SetSvgOpacity(opacity, converter));
            }
        }

        private HtmlTransformRenderResource transformRenderResource;
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

                if (transformRenderResource != null && IsLoaded)
                {
                    transformRenderResource.MatrixChanged -= OnTransformRenderResourceMatrixChanged;
                }

                transform = value;
                transformRenderResource = (HtmlTransformRenderResource)(transform?.GetRenderResource(factory));

                if (transformRenderResource != null && IsLoaded)
                {
                    transformRenderResource.MatrixChanged += OnTransformRenderResourceMatrixChanged;
                }

                renderQueue.InvokeAsync(() => HtmlElement.SetSvgTransform(transformRenderResource?.Matrix, converter));
            }
        }

        private IRenderElementFactory factory;
        private RenderQueue renderQueue;
        private SvgValueConverter converter;

        public HtmlDrawingContainerRenderElement(IRenderElementFactory factory, RenderQueue renderQueue, SvgValueConverter converter) :
            base(SvgDocument.CreateElement("g"), renderQueue)
        {
            this.factory = factory;
            this.renderQueue = renderQueue;
            this.converter = converter;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            if (transformRenderResource != null)
            {
                transformRenderResource.MatrixChanged += OnTransformRenderResourceMatrixChanged;
            }

            renderQueue.InvokeAsync(() => HtmlElement.SetSvgTransform(transformRenderResource?.Matrix, converter));
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            if (transformRenderResource != null)
            {
                transformRenderResource.MatrixChanged -= OnTransformRenderResourceMatrixChanged;
            }
        }

        private void OnTransformRenderResourceMatrixChanged(object sender, EventArgs e)
        {
            renderQueue.InvokeAsync(() => HtmlElement.SetSvgTransform(transformRenderResource.Matrix, converter));
        }
    }
}
