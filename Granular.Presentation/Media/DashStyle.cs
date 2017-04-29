using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace System.Windows.Media
{
    public class DashStyle : Animatable
    {
        public static readonly DependencyProperty DashesProperty = DependencyProperty.Register("Dashes", typeof(IEnumerable<double>), typeof(DashStyle), new FrameworkPropertyMetadata());
        public IEnumerable<double> Dashes
        {
            get { return (IEnumerable<double>)GetValue(DashesProperty); }
            set { SetValue(DashesProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset", typeof(double), typeof(DashStyle), new FrameworkPropertyMetadata(0.0));
        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public DashStyle()
        {
            //
        }

        public DashStyle(IEnumerable<double> dashes, double offset) :
            this()
        {
            this.Dashes = dashes;
            this.Offset = offset;
        }
    }

    public static class DashStyles
    {
        private static DashStyle solid;
        public static DashStyle Solid
        {
            get
            {
                if (solid == null)
                {
                    solid = new DashStyle();
                    solid.Freeze();
                }

                return solid;
            }
        }

        private static DashStyle dash;
        public static DashStyle Dash
        {
            get
            {
                if (dash == null)
                {
                    dash = new DashStyle(new double[] { 2, 2 }, 1);
                    dash.Freeze();
                }

                return dash;
            }
        }

        private static DashStyle dashDot;
        public static DashStyle DashDot
        {
            get
            {
                if (dashDot == null)
                {
                    dashDot = new DashStyle(new double[] { 2, 2, 0, 2 }, 1);
                    dashDot.Freeze();
                }

                return dashDot;
            }
        }

        private static DashStyle dashDotDot;
        public static DashStyle DashDotDot
        {
            get
            {
                if (dashDotDot == null)
                {
                    dashDotDot = new DashStyle(new double[] { 2, 2, 0, 2, 0, 2 }, 1);
                    dashDotDot.Freeze();
                }

                return dashDotDot;
            }
        }

        private static DashStyle dot;
        public static DashStyle Dot
        {
            get
            {
                if (dot == null)
                {
                    dot = new DashStyle(new double[] { 0, 2 }, 0);
                    dot.Freeze();
                }

                return dot;
            }
        }
    }
}
