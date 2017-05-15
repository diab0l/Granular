using System;
using Bridge;
using Granular.Compatibility.Linq;

namespace System.Text
{
    public abstract class Encoding
    {
        private class UTF8Encoding : Encoding
        {
            public override string GetString(byte[] bytes)
            {
                return BytesArrayToString(bytes);
            }

            [Template("String.fromCharCode.apply(null, new Uint8Array({bytes}))")]
            private static extern string BytesArrayToString(byte[] bytes);

            public override byte[] GetBytes(char[] chars)
            {
                return Enumerable.Cast<byte>(chars).ToArray();
            }
        }

        private class UnicodeEncoding : Encoding
        {
            public override string GetString(byte[] bytes)
            {
                ushort[] shorts = new ushort[bytes.Length / 2];
                for (int i = 0; i < shorts.Length; i++)
                {
                    shorts[i] = (ushort)(bytes[2 * i] | bytes[2 * i + 1] << 8);
                }

                return BytesArrayToString(shorts);
            }

            [Template("String.fromCharCode.apply(null, new Uint16Array({bytes}))")]
            private static extern string BytesArrayToString(ushort[] bytes);

            public override byte[] GetBytes(char[] chars)
            {
                byte[] bytes = new byte[chars.Length * 2];
                for (int i = 0; i < chars.Length; i++)
                {
                    bytes[2 * i] = (byte)(chars[i] & 0xff);
                    bytes[2 * i + 1] = (byte)(chars[i] & 0xff00 >> 8);
                }

                return bytes;
            }
        }

        public static readonly Encoding UTF8 = new UTF8Encoding();
        public static readonly Encoding Unicode = new UnicodeEncoding();

        public abstract string GetString(byte[] bytes);
        public abstract byte[] GetBytes(char[] chars);
    }
}
