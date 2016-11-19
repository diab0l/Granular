using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
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

            XamlMember nameDirective = element.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.NameDirective);

            if (nameDirective != null)
            {
                yield return new MemberDefinition((string)nameDirective.GetSingleValue(), elementTypeName);
            }

            foreach (XamlElement child in element.Values.OfType<XamlElement>().Concat(element.Members.SelectMany(member => member.Values.OfType<XamlElement>())))
            {
                string childTypeName = typeParser.ParseTypeName(child.Name);

                foreach (MemberDefinition member in GetMembers(child, childTypeName, typeParser))
                {
                    yield return member;
                }
            }
        }

        private static string GetClassFullName(XamlElement root)
        {
            XamlMember classDirective = root.Directives.FirstOrDefault(directive => directive.Name == XamlLanguage.ClassDirective);
            return classDirective != null ? (string)classDirective.GetSingleValue() : String.Empty;
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
