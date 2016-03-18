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

                if (background != null)
                {
                    background.Changed -= OnBackgroundChanged;
                }

                background = value;
                SetBackground();
                SetIsHitTestVisible();

                if (background != null)
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
                Style.SetBorderThickness(borderThickness, converter);
                SetBounds();
                SetBackground();
                SetCornerRadius();
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

                if (borderBrush != null)
                {
                    borderBrush.Changed -= OnBorderBrushChanged;
                }

                borderBrush = value;
                SetBorderBrush();

                if (borderBrush != null)
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
                SetBounds();
                SetBackground();
                SetBorderBrush();
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
                SetCornerRadius();
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
                SetIsHitTestVisible();
            }
        }

        private IHtmlValueConverter converter;

        public HtmlBorderRenderElement(IRenderQueue renderQueue, IHtmlValueConverter converter) :
            base(renderQueue)
        {
            this.converter = converter;

            bounds = Rect.Zero;
            borderThickness = Thickness.Zero;
            cornerRadius = CornerRadius.Zero;

            Style.SetValue("background-clip", "content-box");

            SetBackground();
            SetBorderBrush();
            SetBounds();
            SetCornerRadius();
            SetIsHitTestVisible();
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            SetBackground();
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            SetBorderBrush();
        }

        private void SetBackground()
        {
            Style.SetBackground(background, new Rect(BorderThickness.Location, (Bounds.Size - BorderThickness.Size).Max(Size.Zero)), converter);
        }

        private void SetBorderBrush()
        {
            Style.SetBorderBrush(BorderBrush, Bounds.Size, converter);
        }

        private void SetBounds()
        {
            Style.SetBounds(new Rect(Bounds.Location, (Bounds.Size - BorderThickness.Size).Max(Size.Zero)), converter);
        }

        private void SetCornerRadius()
        {
            // CornerRadius is relative to the center of the border line, interpolate the outline radius
            CornerRadius borderOutlineCornerRadius = CornerRadius == CornerRadius.Zero ? CornerRadius.Zero : new CornerRadius(
                CornerRadius.TopLeft + (BorderThickness.Top + BorderThickness.Left) / 4,
                CornerRadius.TopRight + (BorderThickness.Top + BorderThickness.Right) / 4,
                CornerRadius.BottomRight + (BorderThickness.Bottom + BorderThickness.Right) / 4,
                CornerRadius.BottomLeft + (BorderThickness.Bottom + BorderThickness.Left) / 4);

            Style.SetCornerRadius(borderOutlineCornerRadius, converter);
        }

        private void SetIsHitTestVisible()
        {
            Style.SetIsHitTestVisible(IsHitTestVisible && Background != null);
        }
    }
}
