using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Granular.Host.Render
{
    public class HtmlBorderRenderElement : HtmlRenderElement, IBorderRenderElement
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

                if (IsLoaded && background != null)
                {
                    background.Changed -= OnBackgroundChanged;
                }

                background = value;
                renderQueue.InvokeAsync(() =>
                {
                    SetBackground();
                    SetIsHitTestVisible();
                });

                if (IsLoaded && background != null)
                {
                    background.Changed += OnBackgroundChanged;
                }
            }
        }

        private Thickness borderThickness;
        public Thickness BorderThickness
        {
            get { return borderThickness; }
            set
            {
                if (borderThickness == value)
                {
                    return;
                }

                borderThickness = value;
                renderQueue.InvokeAsync(() =>
                {
                    HtmlElement.SetHtmlBorderThickness(borderThickness, converter);
                    SetBounds();
                    SetBackground();
                    SetCornerRadius();
                });
            }
        }

        private Brush borderBrush;
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                if (borderBrush == value)
                {
                    return;
                }

                if (IsLoaded && borderBrush != null)
                {
                    borderBrush.Changed -= OnBorderBrushChanged;
                }

                borderBrush = value;
                renderQueue.InvokeAsync(SetBorderBrush);

                if (IsLoaded && borderBrush != null)
                {
                    borderBrush.Changed += OnBorderBrushChanged;
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
                renderQueue.InvokeAsync(() =>
                {
                    SetBounds();
                    SetBackground();
                    SetBorderBrush();
                });
            }
        }

        private CornerRadius cornerRadius;
        public CornerRadius CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                if (cornerRadius == value)
                {
                    return;
                }

                cornerRadius = value;
                renderQueue.InvokeAsync(SetCornerRadius);
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
                renderQueue.InvokeAsync(SetIsHitTestVisible);
            }
        }

        private RenderQueue renderQueue;
        private HtmlValueConverter converter;

        public HtmlBorderRenderElement(RenderQueue renderQueue, HtmlValueConverter converter)
        {
            this.renderQueue = renderQueue;
            this.converter = converter;

            bounds = Rect.Zero;
            borderThickness = Thickness.Zero;
            cornerRadius = CornerRadius.Zero;

            HtmlElement.SetHtmlStyleProperty("background-clip", "content-box");

            SetBackground();
            SetBorderBrush();
            SetBounds();
            SetCornerRadius();
            SetIsHitTestVisible();
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            renderQueue.InvokeAsync(SetBackground);
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            renderQueue.InvokeAsync(SetBorderBrush);
        }

        private void SetBackground()
        {
            HtmlElement.SetHtmlBackground(background, new Rect(BorderThickness.Location, (Bounds.Size - BorderThickness.Size).Max(Size.Zero)), converter);
        }

        private void SetBorderBrush()
        {
            HtmlElement.SetHtmlBorderBrush(BorderBrush, Bounds.Size, converter);
        }

        private void SetBounds()
        {
            HtmlElement.SetHtmlBounds(new Rect(Bounds.Location, (Bounds.Size - BorderThickness.Size).Max(Size.Zero)), converter);
        }

        private void SetCornerRadius()
        {
            // CornerRadius is relative to the center of the border line, interpolate the outline radius
            CornerRadius borderOutlineCornerRadius = CornerRadius == CornerRadius.Zero ? CornerRadius.Zero : new CornerRadius(
                CornerRadius.TopLeft + (BorderThickness.Top + BorderThickness.Left) / 4,
                CornerRadius.TopRight + (BorderThickness.Top + BorderThickness.Right) / 4,
                CornerRadius.BottomRight + (BorderThickness.Bottom + BorderThickness.Right) / 4,
                CornerRadius.BottomLeft + (BorderThickness.Bottom + BorderThickness.Left) / 4);

            HtmlElement.SetHtmlCornerRadius(borderOutlineCornerRadius, converter);
        }

        private void SetIsHitTestVisible()
        {
            HtmlElement.SetHtmlIsHitTestVisible(IsHitTestVisible && Background != null);
        }

        protected override void OnLoad()
        {
            if (Background != null)
            {
                Background.Changed += OnBackgroundChanged;
            }

            if (BorderBrush != null)
            {
                BorderBrush.Changed += OnBorderBrushChanged;
            }

            renderQueue.InvokeAsync(() =>
            {
                SetBackground();
                SetBorderBrush();
                SetIsHitTestVisible();
            });
        }

        protected override void OnUnload()
        {
            if (BorderBrush != null)
            {
                BorderBrush.Changed -= OnBorderBrushChanged;
            }

            if (Background != null)
            {
                Background.Changed -= OnBackgroundChanged;
            }
        }
    }
}
