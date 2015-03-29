using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.BuildTasks
{
    public class KnownTypesParser : ITypeParser
    {
        private static readonly string[] PresentationTypesName = new[]
        {
            "System.Windows.ResourceDictionary"
        };

        public bool TryParseTypeName(string localName, string namespaceName, out string typeName)
        {
            string delimitedLocalName = String.Format(".{0}", localName);

            if (namespaceName == "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
            {
                typeName = PresentationTypesName.FirstOrDefault(presentationTypeName => presentationTypeName.EndsWith(delimitedLocalName));
                return typeName != null;
            }

            typeName = String.Empty;
            return false;
        }
    }
}
