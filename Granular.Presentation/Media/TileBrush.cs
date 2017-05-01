using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media
{
    public enum TileMode
    {
        None,
        //FlipX,
        //FlipY,
        //FlipXY,
        Tile
    }

    public enum BrushMappingMode
    {
        Absolute,
        RelativeToBoundingBox
    }

    public abstract class TileBrush : Brush
    {
        public static readonly DependencyProperty TileModeProperty = DependencyProperty.Register("TileMode", typeof(TileMode), typeof(TileBrush), new FrameworkPropertyMetadata(TileMode.None, (sender, e) => ((TileBrush)sender).OnTileModeChanged(e)));
        public TileMode TileMode
        {
            get { return (TileMode)GetValue(TileModeProperty); }
            set { SetValue(TileModeProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(TileBrush), new FrameworkPropertyMetadata(Stretch.None, (sender, e) => ((TileBrush)sender).OnStretchChanged(e)));
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Rect), typeof(TileBrush), new FrameworkPropertyMetadata(null, (sender, e) => ((TileBrush)sender).OnViewportChanged(e)));
        public Rect Viewport
        {
            get { return (Rect)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        public static readonly DependencyProperty ViewportUnitsProperty = DependencyProperty.Register("ViewportUnits", typeof(BrushMappingMode), typeof(TileBrush), new FrameworkPropertyMetadata(BrushMappingMode.Absolute, (sender, e) => ((TileBrush)sender).OnViewportUnitsChanged(e)));
        public BrushMappingMode ViewportUnits
        {
            get { return (BrushMappingMode)GetValue(ViewportUnitsProperty); }
            set { SetValue(ViewportUnitsProperty, value); }
        }

        private ITileBrushRenderResource renderResource;

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (ITileBrushRenderResource)renderResource;
            this.renderResource.TileMode = TileMode;
            this.renderResource.Stretch = Stretch;
            this.renderResource.Viewport = Viewport;
            this.renderResource.ViewportUnits = ViewportUnits;
        }

        private void OnTileModeChanged(DependencyPropertyChangedEventArgs e)
        {
            renderResource.TileMode = TileMode;
        }

        private void OnStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            renderResource.Stretch = Stretch;
        }

        private void OnViewportChanged(DependencyPropertyChangedEventArgs e)
        {
            renderResource.Viewport = Viewport;
        }

        private void OnViewportUnitsChanged(DependencyPropertyChangedEventArgs e)
        {
            renderResource.ViewportUnits = ViewportUnits;
        }
    }
}
