using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Granular.Compatibility
{
    public static class String
    {
        public static string FromByteArray(byte[] data)
        {
            return DecodeBase64(data);
        }

        [Bridge.Template("decodeURIComponent(escape(String.fromCharCode.apply(null, {data})))")]
        private static extern string DecodeBase64(byte[] data);

        public static bool StartsWith(string s, char value)
        {
            return s.Length > 0 && s[0] == value;
        }

        public static bool EndsWith(string s, char value)
        {
            return s.Length > 0 && s[s.Length - 1] == value;
        }

        public static bool StartsWith(string s, string prefix)
        {
            if (prefix.Length > s.Length)
            {
                return false;
            }

            for (int i = 0; i < prefix.Length; i++)
            {
                if (s[i] != prefix[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool EndsWith(string s, string prefix)
        {
            if (prefix.Length > s.Length)
            {
                return false;
            }

            int startIndex = s.Length - prefix.Length;
            for (int i = 0; i < prefix.Length; i++)
            {
                if (s[startIndex + i] != prefix[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
