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

    public enum Stretch
    {
        None,
        Fill,
        Uniform,
        UniformToFill
    }

    public enum BrushMappingMode
    {
        Absolute,
        RelativeToBoundingBox
    }

    public class TileBrush : Brush
    {
        public static readonly DependencyProperty TileModeProperty = DependencyProperty.Register("TileMode", typeof(TileMode), typeof(TileBrush), new FrameworkPropertyMetadata());
        public TileMode TileMode
        {
            get { return (TileMode)GetValue(TileModeProperty); }
            set { SetValue(TileModeProperty, value); }
        }

        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(TileBrush), new FrameworkPropertyMetadata());
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register("Viewport", typeof(Rect), typeof(TileBrush), new FrameworkPropertyMetadata());
        public Rect Viewport
        {
            get { return (Rect)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        public static readonly DependencyProperty ViewportUnitsProperty = DependencyProperty.Register("ViewportUnits", typeof(BrushMappingMode), typeof(TileBrush), new FrameworkPropertyMetadata());
        public BrushMappingMode ViewportUnits
        {
            get { return (BrushMappingMode)GetValue(ViewportUnitsProperty); }
            set { SetValue(ViewportUnitsProperty, value); }
        }
    }
}
