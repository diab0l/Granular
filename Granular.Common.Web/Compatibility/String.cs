using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
    }
}
