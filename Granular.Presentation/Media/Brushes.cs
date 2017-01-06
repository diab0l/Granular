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
        public static SolidColorBrush AliceBlue { get { return brushes.GetValue(Colors.AliceBlue); } }
        public static SolidColorBrush AntiqueWhite { get { return brushes.GetValue(Colors.AntiqueWhite); } }
        public static SolidColorBrush Aqua { get { return brushes.GetValue(Colors.Aqua); } }
        public static SolidColorBrush Aquamarine { get { return brushes.GetValue(Colors.Aquamarine); } }
        public static SolidColorBrush Azure { get { return brushes.GetValue(Colors.Azure); } }
        public static SolidColorBrush Beige { get { return brushes.GetValue(Colors.Beige); } }
        public static SolidColorBrush Bisque { get { return brushes.GetValue(Colors.Bisque); } }
        public static SolidColorBrush Black { get { return brushes.GetValue(Colors.Black); } }
        public static SolidColorBrush BlanchedAlmond { get { return brushes.GetValue(Colors.BlanchedAlmond); } }
        public static SolidColorBrush Blue { get { return brushes.GetValue(Colors.Blue); } }
        public static SolidColorBrush BlueViolet { get { return brushes.GetValue(Colors.BlueViolet); } }
        public static SolidColorBrush Brown { get { return brushes.GetValue(Colors.Brown); } }
        public static SolidColorBrush BurlyWood { get { return brushes.GetValue(Colors.BurlyWood); } }
        public static SolidColorBrush CadetBlue { get { return brushes.GetValue(Colors.CadetBlue); } }
        public static SolidColorBrush Chartreuse { get { return brushes.GetValue(Colors.Chartreuse); } }
        public static SolidColorBrush Chocolate { get { return brushes.GetValue(Colors.Chocolate); } }
        public static SolidColorBrush Coral { get { return brushes.GetValue(Colors.Coral); } }
        public static SolidColorBrush CornflowerBlue { get { return brushes.GetValue(Colors.CornflowerBlue); } }
        public static SolidColorBrush Cornsilk { get { return brushes.GetValue(Colors.Cornsilk); } }
        public static SolidColorBrush Crimson { get { return brushes.GetValue(Colors.Crimson); } }
        public static SolidColorBrush Cyan { get { return brushes.GetValue(Colors.Cyan); } }
        public static SolidColorBrush DarkBlue { get { return brushes.GetValue(Colors.DarkBlue); } }
        public static SolidColorBrush DarkCyan { get { return brushes.GetValue(Colors.DarkCyan); } }
        public static SolidColorBrush DarkGoldenrod { get { return brushes.GetValue(Colors.DarkGoldenrod); } }
        public static SolidColorBrush DarkGray { get { return brushes.GetValue(Colors.DarkGray); } }
        public static SolidColorBrush DarkGreen { get { return brushes.GetValue(Colors.DarkGreen); } }
        public static SolidColorBrush DarkKhaki { get { return brushes.GetValue(Colors.DarkKhaki); } }
        public static SolidColorBrush DarkMagenta { get { return brushes.GetValue(Colors.DarkMagenta); } }
        public static SolidColorBrush DarkOliveGreen { get { return brushes.GetValue(Colors.DarkOliveGreen); } }
        public static SolidColorBrush DarkOrange { get { return brushes.GetValue(Colors.DarkOrange); } }
        public static SolidColorBrush DarkOrchid { get { return brushes.GetValue(Colors.DarkOrchid); } }
        public static SolidColorBrush DarkRed { get { return brushes.GetValue(Colors.DarkRed); } }
        public static SolidColorBrush DarkSalmon { get { return brushes.GetValue(Colors.DarkSalmon); } }
        public static SolidColorBrush DarkSeaGreen { get { return brushes.GetValue(Colors.DarkSeaGreen); } }
        public static SolidColorBrush DarkSlateBlue { get { return brushes.GetValue(Colors.DarkSlateBlue); } }
        public static SolidColorBrush DarkSlateGray { get { return brushes.GetValue(Colors.DarkSlateGray); } }
        public static SolidColorBrush DarkTurquoise { get { return brushes.GetValue(Colors.DarkTurquoise); } }
        public static SolidColorBrush DarkViolet { get { return brushes.GetValue(Colors.DarkViolet); } }
        public static SolidColorBrush DeepPink { get { return brushes.GetValue(Colors.DeepPink); } }
        public static SolidColorBrush DeepSkyBlue { get { return brushes.GetValue(Colors.DeepSkyBlue); } }
        public static SolidColorBrush DimGray { get { return brushes.GetValue(Colors.DimGray); } }
        public static SolidColorBrush DodgerBlue { get { return brushes.GetValue(Colors.DodgerBlue); } }
        public static SolidColorBrush Firebrick { get { return brushes.GetValue(Colors.Firebrick); } }
        public static SolidColorBrush FloralWhite { get { return brushes.GetValue(Colors.FloralWhite); } }
        public static SolidColorBrush ForestGreen { get { return brushes.GetValue(Colors.ForestGreen); } }
        public static SolidColorBrush Fuchsia { get { return brushes.GetValue(Colors.Fuchsia); } }
        public static SolidColorBrush Gainsboro { get { return brushes.GetValue(Colors.Gainsboro); } }
        public static SolidColorBrush GhostWhite { get { return brushes.GetValue(Colors.GhostWhite); } }
        public static SolidColorBrush Gold { get { return brushes.GetValue(Colors.Gold); } }
        public static SolidColorBrush Goldenrod { get { return brushes.GetValue(Colors.Goldenrod); } }
        public static SolidColorBrush Gray { get { return brushes.GetValue(Colors.Gray); } }
        public static SolidColorBrush Green { get { return brushes.GetValue(Colors.Green); } }
        public static SolidColorBrush GreenYellow { get { return brushes.GetValue(Colors.GreenYellow); } }
        public static SolidColorBrush Honeydew { get { return brushes.GetValue(Colors.Honeydew); } }
        public static SolidColorBrush HotPink { get { return brushes.GetValue(Colors.HotPink); } }
        public static SolidColorBrush IndianRed { get { return brushes.GetValue(Colors.IndianRed); } }
        public static SolidColorBrush Indigo { get { return brushes.GetValue(Colors.Indigo); } }
        public static SolidColorBrush Ivory { get { return brushes.GetValue(Colors.Ivory); } }
        public static SolidColorBrush Khaki { get { return brushes.GetValue(Colors.Khaki); } }
        public static SolidColorBrush Lavender { get { return brushes.GetValue(Colors.Lavender); } }
        public static SolidColorBrush LavenderBlush { get { return brushes.GetValue(Colors.LavenderBlush); } }
        public static SolidColorBrush LawnGreen { get { return brushes.GetValue(Colors.LawnGreen); } }
        public static SolidColorBrush LemonChiffon { get { return brushes.GetValue(Colors.LemonChiffon); } }
        public static SolidColorBrush LightBlue { get { return brushes.GetValue(Colors.LightBlue); } }
        public static SolidColorBrush LightCoral { get { return brushes.GetValue(Colors.LightCoral); } }
        public static SolidColorBrush LightCyan { get { return brushes.GetValue(Colors.LightCyan); } }
        public static SolidColorBrush LightGoldenrodYellow { get { return brushes.GetValue(Colors.LightGoldenrodYellow); } }
        public static SolidColorBrush LightGray { get { return brushes.GetValue(Colors.LightGray); } }
        public static SolidColorBrush LightGreen { get { return brushes.GetValue(Colors.LightGreen); } }
        public static SolidColorBrush LightPink { get { return brushes.GetValue(Colors.LightPink); } }
        public static SolidColorBrush LightSalmon { get { return brushes.GetValue(Colors.LightSalmon); } }
        public static SolidColorBrush LightSeaGreen { get { return brushes.GetValue(Colors.LightSeaGreen); } }
        public static SolidColorBrush LightSkyBlue { get { return brushes.GetValue(Colors.LightSkyBlue); } }
        public static SolidColorBrush LightSlateGray { get { return brushes.GetValue(Colors.LightSlateGray); } }
        public static SolidColorBrush LightSteelBlue { get { return brushes.GetValue(Colors.LightSteelBlue); } }
        public static SolidColorBrush LightYellow { get { return brushes.GetValue(Colors.LightYellow); } }
        public static SolidColorBrush Lime { get { return brushes.GetValue(Colors.Lime); } }
        public static SolidColorBrush LimeGreen { get { return brushes.GetValue(Colors.LimeGreen); } }
        public static SolidColorBrush Linen { get { return brushes.GetValue(Colors.Linen); } }
        public static SolidColorBrush Magenta { get { return brushes.GetValue(Colors.Magenta); } }
        public static SolidColorBrush Maroon { get { return brushes.GetValue(Colors.Maroon); } }
        public static SolidColorBrush MediumAquamarine { get { return brushes.GetValue(Colors.MediumAquamarine); } }
        public static SolidColorBrush MediumBlue { get { return brushes.GetValue(Colors.MediumBlue); } }
        public static SolidColorBrush MediumOrchid { get { return brushes.GetValue(Colors.MediumOrchid); } }
        public static SolidColorBrush MediumPurple { get { return brushes.GetValue(Colors.MediumPurple); } }
        public static SolidColorBrush MediumSeaGreen { get { return brushes.GetValue(Colors.MediumSeaGreen); } }
        public static SolidColorBrush MediumSlateBlue { get { return brushes.GetValue(Colors.MediumSlateBlue); } }
        public static SolidColorBrush MediumSpringGreen { get { return brushes.GetValue(Colors.MediumSpringGreen); } }
        public static SolidColorBrush MediumTurquoise { get { return brushes.GetValue(Colors.MediumTurquoise); } }
        public static SolidColorBrush MediumVioletRed { get { return brushes.GetValue(Colors.MediumVioletRed); } }
        public static SolidColorBrush MidnightBlue { get { return brushes.GetValue(Colors.MidnightBlue); } }
        public static SolidColorBrush MintCream { get { return brushes.GetValue(Colors.MintCream); } }
        public static SolidColorBrush MistyRose { get { return brushes.GetValue(Colors.MistyRose); } }
        public static SolidColorBrush Moccasin { get { return brushes.GetValue(Colors.Moccasin); } }
        public static SolidColorBrush NavajoWhite { get { return brushes.GetValue(Colors.NavajoWhite); } }
        public static SolidColorBrush Navy { get { return brushes.GetValue(Colors.Navy); } }
        public static SolidColorBrush OldLace { get { return brushes.GetValue(Colors.OldLace); } }
        public static SolidColorBrush Olive { get { return brushes.GetValue(Colors.Olive); } }
        public static SolidColorBrush OliveDrab { get { return brushes.GetValue(Colors.OliveDrab); } }
        public static SolidColorBrush Orange { get { return brushes.GetValue(Colors.Orange); } }
        public static SolidColorBrush OrangeRed { get { return brushes.GetValue(Colors.OrangeRed); } }
        public static SolidColorBrush Orchid { get { return brushes.GetValue(Colors.Orchid); } }
        public static SolidColorBrush PaleGoldenrod { get { return brushes.GetValue(Colors.PaleGoldenrod); } }
        public static SolidColorBrush PaleGreen { get { return brushes.GetValue(Colors.PaleGreen); } }
        public static SolidColorBrush PaleTurquoise { get { return brushes.GetValue(Colors.PaleTurquoise); } }
        public static SolidColorBrush PaleVioletRed { get { return brushes.GetValue(Colors.PaleVioletRed); } }
        public static SolidColorBrush PapayaWhip { get { return brushes.GetValue(Colors.PapayaWhip); } }
        public static SolidColorBrush PeachPuff { get { return brushes.GetValue(Colors.PeachPuff); } }
        public static SolidColorBrush Peru { get { return brushes.GetValue(Colors.Peru); } }
        public static SolidColorBrush Pink { get { return brushes.GetValue(Colors.Pink); } }
        public static SolidColorBrush Plum { get { return brushes.GetValue(Colors.Plum); } }
        public static SolidColorBrush PowderBlue { get { return brushes.GetValue(Colors.PowderBlue); } }
        public static SolidColorBrush Purple { get { return brushes.GetValue(Colors.Purple); } }
        public static SolidColorBrush Red { get { return brushes.GetValue(Colors.Red); } }
        public static SolidColorBrush RosyBrown { get { return brushes.GetValue(Colors.RosyBrown); } }
        public static SolidColorBrush RoyalBlue { get { return brushes.GetValue(Colors.RoyalBlue); } }
        public static SolidColorBrush SaddleBrown { get { return brushes.GetValue(Colors.SaddleBrown); } }
        public static SolidColorBrush Salmon { get { return brushes.GetValue(Colors.Salmon); } }
        public static SolidColorBrush SandyBrown { get { return brushes.GetValue(Colors.SandyBrown); } }
        public static SolidColorBrush SeaGreen { get { return brushes.GetValue(Colors.SeaGreen); } }
        public static SolidColorBrush SeaShell { get { return brushes.GetValue(Colors.SeaShell); } }
        public static SolidColorBrush Sienna { get { return brushes.GetValue(Colors.Sienna); } }
        public static SolidColorBrush Silver { get { return brushes.GetValue(Colors.Silver); } }
        public static SolidColorBrush SkyBlue { get { return brushes.GetValue(Colors.SkyBlue); } }
        public static SolidColorBrush SlateBlue { get { return brushes.GetValue(Colors.SlateBlue); } }
        public static SolidColorBrush SlateGray { get { return brushes.GetValue(Colors.SlateGray); } }
        public static SolidColorBrush Snow { get { return brushes.GetValue(Colors.Snow); } }
        public static SolidColorBrush SpringGreen { get { return brushes.GetValue(Colors.SpringGreen); } }
        public static SolidColorBrush SteelBlue { get { return brushes.GetValue(Colors.SteelBlue); } }
        public static SolidColorBrush Tan { get { return brushes.GetValue(Colors.Tan); } }
        public static SolidColorBrush Teal { get { return brushes.GetValue(Colors.Teal); } }
        public static SolidColorBrush Thistle { get { return brushes.GetValue(Colors.Thistle); } }
        public static SolidColorBrush Tomato { get { return brushes.GetValue(Colors.Tomato); } }
        public static SolidColorBrush Transparent { get { return brushes.GetValue(Colors.Transparent); } }
        public static SolidColorBrush Turquoise { get { return brushes.GetValue(Colors.Turquoise); } }
        public static SolidColorBrush Violet { get { return brushes.GetValue(Colors.Violet); } }
        public static SolidColorBrush Wheat { get { return brushes.GetValue(Colors.Wheat); } }
        public static SolidColorBrush White { get { return brushes.GetValue(Colors.White); } }
        public static SolidColorBrush WhiteSmoke { get { return brushes.GetValue(Colors.WhiteSmoke); } }
        public static SolidColorBrush Yellow { get { return brushes.GetValue(Colors.Yellow); } }
        public static SolidColorBrush YellowGreen { get { return brushes.GetValue(Colors.YellowGreen); } }

        private static readonly CacheDictionary<Color, SolidColorBrush> brushes = CacheDictionary<Color, SolidColorBrush>.Create(CreateBrush);

        private static SolidColorBrush CreateBrush(Color color)
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush(color);
            solidColorBrush.Freeze();
            return solidColorBrush;
        }
    }
}
