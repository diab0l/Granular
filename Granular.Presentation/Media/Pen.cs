using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace System.Windows.Media
{
    public enum PenLineCap
    {
        Flat,
        Square,
        Round,
        Triangle,
    }

    public enum PenLineJoin
    {
        Miter,
        Bevel,
        Round,
    }

    public class Pen : Animatable
    {
        public static readonly DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(Pen), new FrameworkPropertyMetadata());
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static readonly DependencyProperty DashCapProperty = DependencyProperty.Register("DashCap", typeof(PenLineCap), typeof(Pen), new FrameworkPropertyMetadata(PenLineCap.Square));
        public PenLineCap DashCap
        {
            get { return (PenLineCap)GetValue(DashCapProperty); }
            set { SetValue(DashCapProperty, value); }
        }

        public static readonly DependencyProperty DashStyleProperty = DependencyProperty.Register("DashStyle", typeof(DashStyle), typeof(Pen), new FrameworkPropertyMetadata(DashStyles.Solid));
        public DashStyle DashStyle
        {
            get { return (DashStyle)GetValue(DashStyleProperty); }
            set { SetValue(DashStyleProperty, value); }
        }

        public static readonly DependencyProperty StartLineCapProperty = DependencyProperty.Register("StartLineCap", typeof(PenLineCap), typeof(Pen), new FrameworkPropertyMetadata(PenLineCap.Flat));
        public PenLineCap StartLineCap
        {
            get { return (PenLineCap)GetValue(StartLineCapProperty); }
            set { SetValue(StartLineCapProperty, value); }
        }

        public static readonly DependencyProperty EndLineCapProperty = DependencyProperty.Register("EndLineCap", typeof(PenLineCap), typeof(Pen), new FrameworkPropertyMetadata(PenLineCap.Flat));
        public PenLineCap EndLineCap
        {
            get { return (PenLineCap)GetValue(EndLineCapProperty); }
            set { SetValue(EndLineCapProperty, value); }
        }

        public static readonly DependencyProperty LineJoinProperty = DependencyProperty.Register("LineJoin", typeof(PenLineJoin), typeof(Pen), new FrameworkPropertyMetadata(PenLineJoin.Miter));
        public PenLineJoin LineJoin
        {
            get { return (PenLineJoin)GetValue(LineJoinProperty); }
            set { SetValue(LineJoinProperty, value); }
        }

        public static readonly DependencyProperty MiterLimitProperty = DependencyProperty.Register("MiterLimit", typeof(double), typeof(Pen), new FrameworkPropertyMetadata(10.0));
        public double MiterLimit
        {
            get { return (double)GetValue(MiterLimitProperty); }
            set { SetValue(MiterLimitProperty, value); }
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register("Thickness", typeof(double), typeof(Pen), new FrameworkPropertyMetadata(1.0));
        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public Pen()
        {
            //
        }

        public Pen(Brush brush, double thickness) :
            this()
        {
            this.Brush = brush;
            this.Thickness = thickness;
        }
    }
}