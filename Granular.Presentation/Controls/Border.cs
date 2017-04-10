using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace System.Windows.Controls
{
    public class Border : Decorator
    {
        public static readonly DependencyProperty BackgroundProperty = Panel.BackgroundProperty.AddOwner(typeof(Border), new FrameworkPropertyMetadata(null, (sender, e) => ((Border)sender).OnBackgroundChanged(e)));
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Border), new FrameworkPropertyMetadata(null, (sender, e) => ((Border)sender).OnBorderBrushChanged(e)));
        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(Thickness.Zero, FrameworkPropertyMetadataOptions.AffectsMeasure, propertyChangedCallback: (sender, e) => ((Border)sender).OnBorderThicknessChanged(e)));
        public Thickness BorderThickness
        {
            get { return (Thickness)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Border), new FrameworkPropertyMetadata(CornerRadius.Zero, (sender, e) => ((Border)sender).OnCornerRadiusChanged(e)));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding", typeof(Thickness), typeof(Border), new FrameworkPropertyMetadata(Thickness.Zero, FrameworkPropertyMetadataOptions.AffectsMeasure));
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        private IBorderRenderElement borderRenderElement;

        public Border()
        {
            //
        }

        protected override object CreateRenderElementContentOverride(IRenderElementFactory factory)
        {
            if (borderRenderElement == null)
            {
                borderRenderElement = factory.CreateBorderRenderElement(this);

                borderRenderElement.Background = Background;
                borderRenderElement.BorderBrush = BorderBrush;
                borderRenderElement.BorderThickness = BorderThickness;
                borderRenderElement.Bounds = new Rect(VisualSize);
                borderRenderElement.CornerRadius = CornerRadius;
                borderRenderElement.IsHitTestVisible = IsHitTestVisible;
            }

            return borderRenderElement;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride((availableSize - BorderThickness.Size - Padding.Size).Max(Size.Zero)) + BorderThickness.Size + Padding.Size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Child != null)
            {
                Child.Arrange(new Rect(BorderThickness.Location + Padding.Location, (finalSize - BorderThickness.Size - Padding.Size).Max(Size.Zero)));
            }

            if (borderRenderElement != null)
            {
                borderRenderElement.Bounds = new Rect(finalSize);
            }

            return finalSize;
        }

        protected override bool HitTestOverride(Point position)
        {
            return VisualSize.Contains(position) &&
                (Background != null || BorderBrush != null && IsOverBorder(position, VisualSize, BorderThickness, CornerRadius));
        }

        private void OnBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (borderRenderElement != null)
            {
                borderRenderElement.Background = (Brush)e.NewValue;
            }
        }

        private void OnBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (borderRenderElement != null)
            {
                borderRenderElement.BorderBrush = (Brush)e.NewValue;
            }
        }

        private void OnBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (borderRenderElement != null)
            {
                borderRenderElement.BorderThickness = (Thickness)e.NewValue;
            }
        }

        private void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (borderRenderElement != null)
            {
                borderRenderElement.CornerRadius = (CornerRadius)e.NewValue;
            }
        }

        private static bool IsOverBorder(Point position, Size borderSize, Thickness borderTickness, CornerRadius cornerRadius)
        {
            return position.X < borderTickness.Left ||
                position.Y < borderTickness.Top ||
                borderSize.Width - position.X < borderTickness.Right ||
                borderSize.Height - position.Y < borderTickness.Bottom; // cornerRadius is ignored
        }
    }
}
