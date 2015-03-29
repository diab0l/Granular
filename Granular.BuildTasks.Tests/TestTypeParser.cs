using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xaml;

namespace Granular.BuildTasks.Tests
{
    public class TestTypeParser : ITypeParser
    {
        public bool TryParseTypeName(string localName, string namespaceName, out string typeName)
        {
            Type type;
            if (TypeParser.TryParseType(new XamlName(localName, namespaceName), out type))
            {
                typeName = type.FullName;
                return true;
            }

            typeName = null;
            return false;
        }
    }
}
