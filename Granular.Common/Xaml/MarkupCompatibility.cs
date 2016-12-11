namespace System.Windows.Markup
{
    public static class MarkupCompatibility
    {
        public const string NamespaceName = "http://schemas.openxmlformats.org/markup-compatibility/2006";

        public static readonly XamlName IgnorableDirective = new XamlName("Ignorable", NamespaceName);

        public static bool IsDirective(string namespaceName, string localName)
        {
            return namespaceName == IgnorableDirective.NamespaceName && localName == IgnorableDirective.LocalName;
        }
    }
}