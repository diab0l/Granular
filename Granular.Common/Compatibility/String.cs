using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Granular.Compatibility
{
    public static class String
    {
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

        public static bool StartsWith(string s, char value)
        {
            return s.Length > 0 && s[0] == value;
        }

        public static bool EndsWith(string s, char value)
        {
            return s.Length > 0 && s[s.Length - 1] == value;
        }

        public static bool StartsWith(string s, string value)
        {
            return s.StartsWith(value);
        }

        public static bool EndsWith(string s, string value)
        {
            return s.EndsWith(value);
        }
    }
}
