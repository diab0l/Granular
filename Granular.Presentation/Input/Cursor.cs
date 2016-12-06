using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Granular.Collections;
using System.Windows.Markup;
using System.Reflection;

namespace System.Windows.Input
{
    public enum CursorType
    {
        None,
        No,
        Arrow,
        AppStarting,
        Cross,
        Help,
        IBeam,
        SizeAll,
        SizeNESW,
        SizeNS,
        SizeNWSE,
        SizeWE,
        UpArrow,
        Wait,
        Hand,
        Pen,
        ScrollNS,
        ScrollWE,
        ScrollAll,
        ScrollN,
        ScrollS,
        ScrollW,
        ScrollE,
        ScrollNW,
        ScrollNE,
        ScrollSW,
        ScrollSE,
        ArrowCD,
    }

    [TypeConverter(typeof(CursorTypeConverter))]
    public sealed class Cursor
    {
        public CursorType CursorType { get; private set; }

        public ImageSource ImageSource { get; private set; }
        public Point Hotspot { get; private set; }

        internal Cursor(CursorType cursorType) :
            this(cursorType, null, null)
        {
            //
        }

        public Cursor(ImageSource imageSource, Point hotspot = null) :
            this(CursorType.None, imageSource, hotspot)
        {
            //
        }

        private Cursor(CursorType cursorType, ImageSource imageSource, Point hotspot)
        {
            this.CursorType = cursorType;
            this.ImageSource = imageSource;
            this.Hotspot = hotspot;
        }

        public override string ToString()
        {
            return String.Format("Cursor({0})", ImageSource != null ? ImageSource.ToString() : CursorType.ToString());
        }
    }

    public static class Cursors
    {
        private static readonly CacheDictionary<CursorType, Cursor> cursors = new CacheDictionary<CursorType, Cursor>(cursorType => new Cursor(cursorType));

        public static Cursor None { get { return cursors.GetValue(CursorType.None); } }
        public static Cursor No { get { return cursors.GetValue(CursorType.No); } }
        public static Cursor Arrow { get { return cursors.GetValue(CursorType.Arrow); } }
        public static Cursor AppStarting { get { return cursors.GetValue(CursorType.AppStarting); } }
        public static Cursor Cross { get { return cursors.GetValue(CursorType.Cross); } }
        public static Cursor Help { get { return cursors.GetValue(CursorType.Help); } }
        public static Cursor IBeam { get { return cursors.GetValue(CursorType.IBeam); } }
        public static Cursor SizeAll { get { return cursors.GetValue(CursorType.SizeAll); } }
        public static Cursor SizeNESW { get { return cursors.GetValue(CursorType.SizeNESW); } }
        public static Cursor SizeNS { get { return cursors.GetValue(CursorType.SizeNS); } }
        public static Cursor SizeNWSE { get { return cursors.GetValue(CursorType.SizeNWSE); } }
        public static Cursor SizeWE { get { return cursors.GetValue(CursorType.SizeWE); } }
        public static Cursor UpArrow { get { return cursors.GetValue(CursorType.UpArrow); } }
        public static Cursor Wait { get { return cursors.GetValue(CursorType.Wait); } }
        public static Cursor Hand { get { return cursors.GetValue(CursorType.Hand); } }
        public static Cursor Pen { get { return cursors.GetValue(CursorType.Pen); } }
        public static Cursor ScrollNS { get { return cursors.GetValue(CursorType.ScrollNS); } }
        public static Cursor ScrollWE { get { return cursors.GetValue(CursorType.ScrollWE); } }
        public static Cursor ScrollAll { get { return cursors.GetValue(CursorType.ScrollAll); } }
        public static Cursor ScrollN { get { return cursors.GetValue(CursorType.ScrollN); } }
        public static Cursor ScrollS { get { return cursors.GetValue(CursorType.ScrollS); } }
        public static Cursor ScrollW { get { return cursors.GetValue(CursorType.ScrollW); } }
        public static Cursor ScrollE { get { return cursors.GetValue(CursorType.ScrollE); } }
        public static Cursor ScrollNW { get { return cursors.GetValue(CursorType.ScrollNW); } }
        public static Cursor ScrollNE { get { return cursors.GetValue(CursorType.ScrollNE); } }
        public static Cursor ScrollSW { get { return cursors.GetValue(CursorType.ScrollSW); } }
        public static Cursor ScrollSE { get { return cursors.GetValue(CursorType.ScrollSE); } }
        public static Cursor ArrowCD { get { return cursors.GetValue(CursorType.ArrowCD); } }
    }

    public class CursorTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, Uri sourceUri, object value)
        {
            if (value is string)
            {
                PropertyInfo propertyInfo = typeof(Cursors).GetProperty((string)value, BindingFlags.Static | BindingFlags.Public);
                if (propertyInfo != null)
                {
                    return propertyInfo.GetValue(null, new object[0]);
                }
            }

            throw new Granular.Exception("Can't convert \"{0}\" to Cursor", value);
        }
    }
}
