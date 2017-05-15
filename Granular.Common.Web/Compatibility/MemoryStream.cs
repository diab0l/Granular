using System;

namespace System.IO
{
    public enum SeekOrigin { Begin, Current, End }

    public abstract class Stream : IDisposable
    {
        public abstract int Length { get; }

        public virtual void Dispose()
        {
            //
        }

        public abstract void CopyTo(MemoryStream memoryStream);
    }

    public class MemoryStream : Stream
    {
        private byte[] data;

        public int Position { get; private set; }
        public override int Length { get { return data.Length; } }

        public MemoryStream() :
            this(null)
        {
            //
        }

        public MemoryStream(byte[] data)
        {
            this.data = data;
        }

        public void Read(byte[] buffer, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] = data[Position + i];
            }

            Position += length;
        }

        public byte ReadByte()
        {
            byte value = data[Position];
            Position++;
            return value;
        }

        public override void CopyTo(MemoryStream memoryStream)
        {
            memoryStream.data = data;
        }

        public byte[] ToArray()
        {
            return data;
        }

        public void Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length + offset;
                    break;
            }
        }
    }
}
