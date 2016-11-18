using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Granular.Collections;

namespace System.Windows.Media
{
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicStaticProperty)]
    public static class Brushes
    {
        public static Brush AliceBlue { get { return brushes.GetValue(Colors.AliceBlue); } }
        public static Brush AntiqueWhite { get { return brushes.GetValue(Colors.AntiqueWhite); } }
        public static Brush Aqua { get { return brushes.GetValue(Colors.Aqua); } }
        public static Brush Aquamarine { get { return brushes.GetValue(Colors.Aquamarine); } }
        public static Brush Azure { get { return brushes.GetValue(Colors.Azure); } }
        public static Brush Beige { get { return brushes.GetValue(Colors.Beige); } }
        public static Brush Bisque { get { return brushes.GetValue(Colors.Bisque); } }
        public static Brush Black { get { return brushes.GetValue(Colors.Black); } }
        public static Brush BlanchedAlmond { get { return brushes.GetValue(Colors.BlanchedAlmond); } }
        public static Brush Blue { get { return brushes.GetValue(Colors.Blue); } }
        public static Brush BlueViolet { get { return brushes.GetValue(Colors.BlueViolet); } }
        public static Brush Brown { get { return brushes.GetValue(Colors.Brown); } }
        public static Brush BurlyWood { get { return brushes.GetValue(Colors.BurlyWood); } }
        public static Brush CadetBlue { get { return brushes.GetValue(Colors.CadetBlue); } }
        public static Brush Chartreuse { get { return brushes.GetValue(Colors.Chartreuse); } }
        public static Brush Chocolate { get { return brushes.GetValue(Colors.Chocolate); } }
        public static Brush Coral { get { return brushes.GetValue(Colors.Coral); } }
        public static Brush CornflowerBlue { get { return brushes.GetValue(Colors.CornflowerBlue); } }
        public static Brush Cornsilk { get { return brushes.GetValue(Colors.Cornsilk); } }
        public static Brush Crimson { get { return brushes.GetValue(Colors.Crimson); } }
        public static Brush Cyan { get { return brushes.GetValue(Colors.Cyan); } }
        public static Brush DarkBlue { get { return brushes.GetValue(Colors.DarkBlue); } }
        public static Brush DarkCyan { get { return brushes.GetValue(Colors.DarkCyan); } }
        public static Brush DarkGoldenrod { get { return brushes.GetValue(Colors.DarkGoldenrod); } }
        public static Brush DarkGray { get { return brushes.GetValue(Colors.DarkGray); } }
        public static Brush DarkGreen { get { return brushes.GetValue(Colors.DarkGreen); } }
        public static Brush DarkKhaki { get { return brushes.GetValue(Colors.DarkKhaki); } }
        public static Brush DarkMagenta { get { return brushes.GetValue(Colors.DarkMagenta); } }
        public static Brush DarkOliveGreen { get { return brushes.GetValue(Colors.DarkOliveGreen); } }
        public static Brush DarkOrange { get { return brushes.GetValue(Colors.DarkOrange); } }
        public static Brush DarkOrchid { get { return brushes.GetValue(Colors.DarkOrchid); } }
        public static Brush DarkRed { get { return brushes.GetValue(Colors.DarkRed); } }
        public static Brush DarkSalmon { get { return brushes.GetValue(Colors.DarkSalmon); } }
        public static Brush DarkSeaGreen { get { return brushes.GetValue(Colors.DarkSeaGreen); } }
        public static Brush DarkSlateBlue { get { return brushes.GetValue(Colors.DarkSlateBlue); } }
        public static Brush DarkSlateGray { get { return brushes.GetValue(Colors.DarkSlateGray); } }
        public static Brush DarkTurquoise { get { return brushes.GetValue(Colors.DarkTurquoise); } }
        public static Brush DarkViolet { get { return brushes.GetValue(Colors.DarkViolet); } }
        public static Brush DeepPink { get { return brushes.GetValue(Colors.DeepPink); } }
        public static Brush DeepSkyBlue { get { return brushes.GetValue(Colors.DeepSkyBlue); } }
        public static Brush DimGray { get { return brushes.GetValue(Colors.DimGray); } }
        public static Brush DodgerBlue { get { return brushes.GetValue(Colors.DodgerBlue); } }
        public static Brush Firebrick { get { return brushes.GetValue(Colors.Firebrick); } }
        public static Brush FloralWhite { get { return brushes.GetValue(Colors.FloralWhite); } }
        public static Brush ForestGreen { get { return brushes.GetValue(Colors.ForestGreen); } }
        public static Brush Fuchsia { get { return brushes.GetValue(Colors.Fuchsia); } }
        public static Brush Gainsboro { get { return brushes.GetValue(Colors.Gainsboro); } }
        public static Brush GhostWhite { get { return brushes.GetValue(Colors.GhostWhite); } }
        public static Brush Gold { get { return brushes.GetValue(Colors.Gold); } }
        public static Brush Goldenrod { get { return brushes.GetValue(Colors.Goldenrod); } }
        public static Brush Gray { get { return brushes.GetValue(Colors.Gray); } }
        public static Brush Green { get { return brushes.GetValue(Colors.Green); } }
        public static Brush GreenYellow { get { return brushes.GetValue(Colors.GreenYellow); } }
        public static Brush Honeydew { get { return brushes.GetValue(Colors.Honeydew); } }
        public static Brush HotPink { get { return brushes.GetValue(Colors.HotPink); } }
        public static Brush IndianRed { get { return brushes.GetValue(Colors.IndianRed); } }
        public static Brush Indigo { get { return brushes.GetValue(Colors.Indigo); } }
        public static Brush Ivory { get { return brushes.GetValue(Colors.Ivory); } }
        public static Brush Khaki { get { return brushes.GetValue(Colors.Khaki); } }
        public static Brush Lavender { get { return brushes.GetValue(Colors.Lavender); } }
        public static Brush LavenderBlush { get { return brushes.GetValue(Colors.LavenderBlush); } }
        public static Brush LawnGreen { get { return brushes.GetValue(Colors.LawnGreen); } }
        public static Brush LemonChiffon { get { return brushes.GetValue(Colors.LemonChiffon); } }
        public static Brush LightBlue { get { return brushes.GetValue(Colors.LightBlue); } }
        public static Brush LightCoral { get { return brushes.GetValue(Colors.LightCoral); } }
        public static Brush LightCyan { get { return brushes.GetValue(Colors.LightCyan); } }
        public static Brush LightGoldenrodYellow { get { return brushes.GetValue(Colors.LightGoldenrodYellow); } }
        public static Brush LightGray { get { return brushes.GetValue(Colors.LightGray); } }
        public static Brush LightGreen { get { return brushes.GetValue(Colors.LightGreen); } }
        public static Brush LightPink { get { return brushes.GetValue(Colors.LightPink); } }
        public static Brush LightSalmon { get { return brushes.GetValue(Colors.LightSalmon); } }
        public static Brush LightSeaGreen { get { return brushes.GetValue(Colors.LightSeaGreen); } }
        public static Brush LightSkyBlue { get { return brushes.GetValue(Colors.LightSkyBlue); } }
        public static Brush LightSlateGray { get { return brushes.GetValue(Colors.LightSlateGray); } }
        public static Brush LightSteelBlue { get { return brushes.GetValue(Colors.LightSteelBlue); } }
        public static Brush LightYellow { get { return brushes.GetValue(Colors.LightYellow); } }
        public static Brush Lime { get { return brushes.GetValue(Colors.Lime); } }
        public static Brush LimeGreen { get { return brushes.GetValue(Colors.LimeGreen); } }
        public static Brush Linen { get { return brushes.GetValue(Colors.Linen); } }
        public static Brush Magenta { get { return brushes.GetValue(Colors.Magenta); } }
        public static Brush Maroon { get { return brushes.GetValue(Colors.Maroon); } }
        public static Brush MediumAquamarine { get { return brushes.GetValue(Colors.MediumAquamarine); } }
        public static Brush MediumBlue { get { return brushes.GetValue(Colors.MediumBlue); } }
        public static Brush MediumOrchid { get { return brushes.GetValue(Colors.MediumOrchid); } }
        public static Brush MediumPurple { get { return brushes.GetValue(Colors.MediumPurple); } }
        public static Brush MediumSeaGreen { get { return brushes.GetValue(Colors.MediumSeaGreen); } }
        public static Brush MediumSlateBlue { get { return brushes.GetValue(Colors.MediumSlateBlue); } }
        public static Brush MediumSpringGreen { get { return brushes.GetValue(Colors.MediumSpringGreen); } }
        public static Brush MediumTurquoise { get { return brushes.GetValue(Colors.MediumTurquoise); } }
        public static Brush MediumVioletRed { get { return brushes.GetValue(Colors.MediumVioletRed); } }
        public static Brush MidnightBlue { get { return brushes.GetValue(Colors.MidnightBlue); } }
        public static Brush MintCream { get { return brushes.GetValue(Colors.MintCream); } }
        public static Brush MistyRose { get { return brushes.GetValue(Colors.MistyRose); } }
        public static Brush Moccasin { get { return brushes.GetValue(Colors.Moccasin); } }
        public static Brush NavajoWhite { get { return brushes.GetValue(Colors.NavajoWhite); } }
        public static Brush Navy { get { return brushes.GetValue(Colors.Navy); } }
        public static Brush OldLace { get { return brushes.GetValue(Colors.OldLace); } }
        public static Brush Olive { get { return brushes.GetValue(Colors.Olive); } }
        public static Brush OliveDrab { get { return brushes.GetValue(Colors.OliveDrab); } }
        public static Brush Orange { get { return brushes.GetValue(Colors.Orange); } }
        public static Brush OrangeRed { get { return brushes.GetValue(Colors.OrangeRed); } }
        public static Brush Orchid { get { return brushes.GetValue(Colors.Orchid); } }
        public static Brush PaleGoldenrod { get { return brushes.GetValue(Colors.PaleGoldenrod); } }
        public static Brush PaleGreen { get { return brushes.GetValue(Colors.PaleGreen); } }
        public static Brush PaleTurquoise { get { return brushes.GetValue(Colors.PaleTurquoise); } }
        public static Brush PaleVioletRed { get { return brushes.GetValue(Colors.PaleVioletRed); } }
        public static Brush PapayaWhip { get { return brushes.GetValue(Colors.PapayaWhip); } }
        public static Brush PeachPuff { get { return brushes.GetValue(Colors.PeachPuff); } }
        public static Brush Peru { get { return brushes.GetValue(Colors.Peru); } }
        public static Brush Pink { get { return brushes.GetValue(Colors.Pink); } }
        public static Brush Plum { get { return brushes.GetValue(Colors.Plum); } }
        public static Brush PowderBlue { get { return brushes.GetValue(Colors.PowderBlue); } }
        public static Brush Purple { get { return brushes.GetValue(Colors.Purple); } }
        public static Brush Red { get { return brushes.GetValue(Colors.Red); } }
        public static Brush RosyBrown { get { return brushes.GetValue(Colors.RosyBrown); } }
        public static Brush RoyalBlue { get { return brushes.GetValue(Colors.RoyalBlue); } }
        public static Brush SaddleBrown { get { return brushes.GetValue(Colors.SaddleBrown); } }
        public static Brush Salmon { get { return brushes.GetValue(Colors.Salmon); } }
        public static Brush SandyBrown { get { return brushes.GetValue(Colors.SandyBrown); } }
        public static Brush SeaGreen { get { return brushes.GetValue(Colors.SeaGreen); } }
        public static Brush SeaShell { get { return brushes.GetValue(Colors.SeaShell); } }
        public static Brush Sienna { get { return brushes.GetValue(Colors.Sienna); } }
        public static Brush Silver { get { return brushes.GetValue(Colors.Silver); } }
        public static Brush SkyBlue { get { return brushes.GetValue(Colors.SkyBlue); } }
        public static Brush SlateBlue { get { return brushes.GetValue(Colors.SlateBlue); } }
        public static Brush SlateGray { get { return brushes.GetValue(Colors.SlateGray); } }
        public static Brush Snow { get { return brushes.GetValue(Colors.Snow); } }
        public static Brush SpringGreen { get { return brushes.GetValue(Colors.SpringGreen); } }
        public static Brush SteelBlue { get { return brushes.GetValue(Colors.SteelBlue); } }
        public static Brush Tan { get { return brushes.GetValue(Colors.Tan); } }
        public static Brush Teal { get { return brushes.GetValue(Colors.Teal); } }
        public static Brush Thistle { get { return brushes.GetValue(Colors.Thistle); } }
        public static Brush Tomato { get { return brushes.GetValue(Colors.Tomato); } }
        public static Brush Transparent { get { return brushes.GetValue(Colors.Transparent); } }
        public static Brush Turquoise { get { return brushes.GetValue(Colors.Turquoise); } }
        public static Brush Violet { get { return brushes.GetValue(Colors.Violet); } }
        public static Brush Wheat { get { return brushes.GetValue(Colors.Wheat); } }
        public static Brush White { get { return brushes.GetValue(Colors.White); } }
        public static Brush WhiteSmoke { get { return brushes.GetValue(Colors.WhiteSmoke); } }
        public static Brush Yellow { get { return brushes.GetValue(Colors.Yellow); } }
        public static Brush YellowGreen { get { return brushes.GetValue(Colors.YellowGreen); } }

        private static readonly CacheDictionary<Color, Brush> brushes = new CacheDictionary<Color, Brush>(CreateBrush);

        private static Brush CreateBrush(Color color)
        {
            Brush brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}
