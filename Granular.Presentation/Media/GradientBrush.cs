using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace System.Windows.Media
{
    public enum GradientSpreadMethod
    {
        Pad,
        Reflect,
        Repeat
    }

    public class GradientStop : Animatable
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(GradientStop), new FrameworkPropertyMetadata(Colors.Transparent));
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(GradientStop), new FrameworkPropertyMetadata());
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public GradientStop()
        {
            //
        }

        public GradientStop(Color color, double offset)
        {
            this.Color = color;
            this.Offset = offset;
        }
    }

    public class GradientStopCollection : FreezableCollection<GradientStop>
    {
        public GradientStopCollection()
        {
            //
        }

        public GradientStopCollection(IEnumerable<GradientStop> collection) :
            base(collection)
        {
            //
        }
    }

    [ContentProperty("GradientStops")]
    public abstract class GradientBrush : Brush
    {
        public static readonly DependencyProperty SpreadMethodProperty = DependencyProperty.Register("SpreadMethod", typeof(GradientSpreadMethod), typeof(GradientBrush), new FrameworkPropertyMetadata());
        public GradientSpreadMethod SpreadMethod
        {
            get { return (GradientSpreadMethod)GetValue(SpreadMethodProperty); }
            set { SetValue(SpreadMethodProperty, value); }
        }

        public static readonly DependencyProperty MappingModeProperty = DependencyProperty.Register("MappingMode", typeof(BrushMappingMode), typeof(GradientBrush), new FrameworkPropertyMetadata(BrushMappingMode.RelativeToBoundingBox));
        public BrushMappingMode MappingMode
        {
            get { return (BrushMappingMode)GetValue(MappingModeProperty); }
            set { SetValue(MappingModeProperty, value); }
        }

        public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), typeof(GradientBrush), new FrameworkPropertyMetadata());
        public GradientStopCollection GradientStops
        {
            get { return (GradientStopCollection)GetValue(GradientStopsProperty); }
            set { SetValue(GradientStopsProperty, value); }
        }

        public GradientBrush()
        {
            //
        }

        public GradientBrush(IEnumerable<GradientStop> gradientStops)
        {
            this.GradientStops = new GradientStopCollection(gradientStops);
        }
    }
}
