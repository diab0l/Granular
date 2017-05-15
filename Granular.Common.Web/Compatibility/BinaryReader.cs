using System;
using System.Text;
using Bridge;

namespace System.IO
{
    public class BinaryReader : IDisposable
    {
        private MemoryStream memoryStream;

        public BinaryReader(MemoryStream memoryStream)
        {
            this.memoryStream = memoryStream;
        }

        public void Dispose()
        {
            //
        }

        public byte ReadByte()
        {
            return memoryStream.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            memoryStream.Read(bytes, 0, count);
            return bytes;
        }

        public int ReadInt32()
        {
            return ReadByte() | ReadByte() << 8 | ReadByte() << 16 | ReadByte() << 24;
        }

        public string ReadString()
        {
            int length = Read7BitEncodedInt();
            return BytesArrayToString(ReadBytes(length));
        }

        public int Read7BitEncodedInt()
        {
            int value = 0;

            for (int i = 0; i < 5; i++)
            {
                byte b = ReadByte();

                value = value | ((b & 0x7f) << (i * 7));

                if ((b & 0x80) == 0)
                {
                    break;
                }
                else if (i == 4)
                {
                    throw new Exception("Can't read int at current position");
                }
            }

            return value;
        }

        [Template("String.fromCharCode.apply(null, new Uint8Array({bytes}))")]
        private static extern string BytesArrayToString(byte[] bytes);
    }
}
