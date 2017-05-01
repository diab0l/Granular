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
        public static readonly DependencyProperty SpreadMethodProperty = DependencyProperty.Register("SpreadMethod", typeof(GradientSpreadMethod), typeof(GradientBrush), new FrameworkPropertyMetadata(GradientSpreadMethod.Pad, (sender, e) => ((GradientBrush)sender).OnSpreadMethodChanged(e)));
        public GradientSpreadMethod SpreadMethod
        {
            get { return (GradientSpreadMethod)GetValue(SpreadMethodProperty); }
            set { SetValue(SpreadMethodProperty, value); }
        }

        public static readonly DependencyProperty MappingModeProperty = DependencyProperty.Register("MappingMode", typeof(BrushMappingMode), typeof(GradientBrush), new FrameworkPropertyMetadata(BrushMappingMode.RelativeToBoundingBox, (sender, e) => ((GradientBrush)sender).OnMappingModeChanged(e)));
        public BrushMappingMode MappingMode
        {
            get { return (BrushMappingMode)GetValue(MappingModeProperty); }
            set { SetValue(MappingModeProperty, value); }
        }

        public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), typeof(GradientBrush), new FrameworkPropertyMetadata(null, (sender, e) => ((GradientBrush)sender).OnGradientStopsChanged(e)));
        public GradientStopCollection GradientStops
        {
            get { return (GradientStopCollection)GetValue(GradientStopsProperty); }
            set { SetValue(GradientStopsProperty, value); }
        }

        private IGradientBrushRenderResource renderResource;

        public GradientBrush() :
            this(new GradientStopCollection())
        {
            //
        }

        public GradientBrush(IEnumerable<GradientStop> gradientStops) :
            this(new GradientStopCollection(gradientStops))
        {
            //
        }

        private GradientBrush(GradientStopCollection gradientStops)
        {
            this.GradientStops = gradientStops;
        }

        protected override void OnRenderResourceCreated(object renderResource)
        {
            base.OnRenderResourceCreated(renderResource);

            this.renderResource = (IGradientBrushRenderResource)renderResource;
            this.renderResource.SpreadMethod = SpreadMethod;
            this.renderResource.MappingMode = MappingMode;
            this.renderResource.GradientStops = GradientStops.Select(gradientStop => new RenderGradientStop(gradientStop.Color, gradientStop.Offset)).ToArray();
        }

        private void OnSpreadMethodChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.SpreadMethod = SpreadMethod;
            }
        }

        private void OnMappingModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (renderResource != null)
            {
                renderResource.MappingMode = MappingMode;
            }
        }

        private void OnGradientStopsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((GradientStopCollection)e.OldValue).TrySetContextParent(null);
            }

            if (e.NewValue != null)
            {
                ((GradientStopCollection)e.NewValue).TrySetContextParent(this);
            }

            if (renderResource != null)
            {
                this.renderResource.GradientStops = e.NewValue != null ? ((GradientStopCollection)e.NewValue).Select(gradientStop => new RenderGradientStop(gradientStop.Color, gradientStop.Offset)).ToArray() : null;
            }
        }
    }
}
