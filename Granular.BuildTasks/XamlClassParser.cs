using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;
using Granular.Extensions;

namespace Granular.BuildTasks
{
    public static class XamlClassParser
    {
        public static ClassDefinition Parse(XamlElement element, ITypeParser typeParser)
        {
            string fullName = GetClassFullName(element);

            if (fullName.IsNullOrEmpty())
            {
                return null;
            }

            string baseTypeName = typeParser.ParseTypeName(element.Name);

            IEnumerable<MemberDefinition> members = GetMembers(element, baseTypeName, typeParser).ToArray();

            return new ClassDefinition(GetTypeName(fullName), GetTypeNamespace(fullName), baseTypeName, members);
        }

        private static IEnumerable<MemberDefinition> GetMembers(XamlElement element, string elementTypeName, ITypeParser typeParser)
        {
            if (elementTypeName == "System.Windows.ResourceDictionary")
            {
                yield break;
            }

            XamlAttribute nameAttribute = element.Attributes.FirstOrDefault(attribute => attribute.Name == XamlLanguage.NameDirective);

            if (nameAttribute != null)
            {
                yield return new MemberDefinition((string)nameAttribute.Value, elementTypeName);
            }

            foreach (XamlElement child in element.Children)
            {
                string childTypeName = typeParser.ParseTypeName(child.Name.IsMemberName ? child.Name.ContainingTypeName : child.Name);

                foreach (MemberDefinition member in GetMembers(child, childTypeName, typeParser))
                {
                    yield return member;
                }
            }
        }

        private static string GetClassFullName(XamlElement root)
        {
            XamlAttribute classAttribute = root.Attributes.FirstOrDefault(attribute => attribute.Name == XamlLanguage.ClassDirective);
            return classAttribute != null ? (string)classAttribute.Value : String.Empty;
        }

        private static string GetTypeName(string typeFullName)
        {
            int typeSeparatorIndex = typeFullName.LastIndexOf(".");
            return typeFullName.Substring(typeSeparatorIndex + 1);
        }

        private static string GetTypeNamespace(string typeFullName)
        {
            int typeSeparatorIndex = typeFullName.LastIndexOf(".");
            return typeSeparatorIndex != -1 ? typeFullName.Substring(0, typeSeparatorIndex) : String.Empty;
        }
    }
}
