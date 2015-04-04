using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Granular.Compatibility
{
    public static class String
    {
        public static bool IsNullOrWhitespace(string value)
        {
            return System.String.IsNullOrWhiteSpace(value);
        }

        public static string FromByteArray(byte[] data)
        {
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
