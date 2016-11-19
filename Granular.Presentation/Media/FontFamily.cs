using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace System.Windows.Media
{
    [TypeConverter(typeof(FontFamilyTypeConverter))]
    [Bridge.Reflectable(Bridge.MemberAccessibility.PublicInstanceProperty)]
    public class FontFamily
    {
        public static readonly FontFamily Default = new FontFamily(String.Empty);

        public string FamilyName { get { return FamilyNames.FirstOrDefault(); } }
        public IEnumerable<string> FamilyNames { get; private set; }

        public FontFamily(string familyName)
        {
            this.FamilyNames = new[] { familyName };
        }

        public FontFamily(IEnumerable<string> familyNames)
        {
            this.FamilyNames = familyNames;
        }

        public static FontFamily Parse(string value)
        {
            return new FontFamily(value.Split(',').Select(s => s.Trim()).ToArray());
        }
    }

    public class FontFamilyTypeConverter : ITypeConverter
    {
        public object ConvertFrom(XamlNamespaces namespaces, object value)
        {
            return FontFamily.Parse(value.ToString().Trim());
        }
    }
}
