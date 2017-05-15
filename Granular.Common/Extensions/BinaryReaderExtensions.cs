using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Granular.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static void SkipStrings(this BinaryReader binaryReader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                binaryReader.ReadString();
            }
        }
    }
}
