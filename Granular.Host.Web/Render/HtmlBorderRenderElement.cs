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
                Style.SetBackground(background, converter);
                Style.SetIsHitTestVisible(IsHitTestVisible && background != null);

                if (background != null)
                {
                    background.Changed += OnBackgroundChanged;
                }
            }
        }

        private Thickness borderThickness = Thickness.Zero;
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
                Style.SetBounds(new Rect(Bounds.Location, Bounds.Size - BorderThickness.Size), converter);
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
                Style.SetBorderBrush(BorderBrush, converter);

                if (borderBrush != null)
                {
                    borderBrush.Changed += OnBorderBrushChanged;
                }
            }
        }

        private Rect bounds = Rect.Zero;
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
                Style.SetBounds(new Rect(Bounds.Location, Bounds.Size - BorderThickness.Size), converter);
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
                Style.SetCornerRadius(cornerRadius, converter);
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

        private IHtmlValueConverter converter;

        public HtmlBorderRenderElement(IRenderQueue renderQueue, IHtmlValueConverter converter) :
            base(renderQueue)
        {
            this.converter = converter;

            bounds = Rect.Zero;
            borderThickness = Thickness.Zero;
            cornerRadius = CornerRadius.Zero;

            Style.SetBackground(Background, converter);
            Style.SetBorderThickness(BorderThickness, converter);
            Style.SetBounds(new Rect(Bounds.Location, Bounds.Size - BorderThickness.Size), converter);
            Style.SetBorderBrush(BorderBrush, converter);
            Style.SetCornerRadius(cornerRadius, converter);
            Style.SetIsHitTestVisible(IsHitTestVisible && Background != null);
        }

        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            Style.SetBackground(Background, converter);
        }

        private void OnBorderBrushChanged(object sender, EventArgs e)
        {
            Style.SetBorderBrush(BorderBrush, converter);
        }
    }
}
