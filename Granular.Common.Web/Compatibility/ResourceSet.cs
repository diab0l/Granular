using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Granular.Extensions;

namespace System.Resources
{
    public class ResourceSet : IDisposable
    {
        private class ResourceEntry
        {
            public string Name { get; }
            public int Offset { get; }

            public ResourceEntry(string name, int offset)
            {
                this.Name = name;
                this.Offset = offset;
            }
        }

        private const int Int32Size = 4;

        private MemoryStream memoryStream;
        private BinaryReader binaryReader;

        private List<ResourceEntry> resourceEntries;

        public ResourceSet(Stream stream)
        {
            memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            binaryReader = new BinaryReader(memoryStream);

            memoryStream.Seek(0, SeekOrigin.Begin);

            uint header = (uint)binaryReader.ReadInt32();
            if (header != 0xBEEFCACE)
            {
                throw new Granular.Exception($"Invalid ResourceSet header ({header})");
            }

            int version = binaryReader.ReadInt32();
            if (version != 1)
            {
                throw new Granular.Exception($"Unsupported ResourceSet version ({version})");
            }

            int bytesToSkip = binaryReader.ReadInt32();
            memoryStream.Seek(bytesToSkip, SeekOrigin.Current);

            int version2 = binaryReader.ReadInt32();

            int resourcesCount = binaryReader.ReadInt32();
            int typesCount = binaryReader.ReadInt32();

            // types name
            binaryReader.SkipStrings(typesCount);

            int paddingCount = 7 - (int)(memoryStream.Position + 7) % 8;
            memoryStream.Seek(paddingCount, SeekOrigin.Current);

            // hash
            memoryStream.Seek(resourcesCount * Int32Size, SeekOrigin.Current);

            // offset
            memoryStream.Seek(resourcesCount * Int32Size, SeekOrigin.Current);

            int dataOrigin = binaryReader.ReadInt32();

            resourceEntries = new List<ResourceEntry>();

            for (int i = 0; i < resourcesCount; i++)
            {
                string name = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(binaryReader.ReadString().ToCharArray()));
                int offset = binaryReader.ReadInt32();

                resourceEntries.Add(new ResourceEntry(name, dataOrigin + offset));
            }
        }

        public void Dispose()
        {
            binaryReader.Dispose();
            memoryStream.Dispose();
        }

        public object GetObject(string name)
        {
            for (int i = 0; i < resourceEntries.Count; i++)
            {
                ResourceEntry entry = resourceEntries[i];
                if (entry.Name != name)
                {
                    continue;
                }

                memoryStream.Seek(entry.Offset, SeekOrigin.Begin);

                int resourceTypeCode = binaryReader.Read7BitEncodedInt();
                if (resourceTypeCode != 0x21) // ResourceTypeCode.Stream
                {
                    throw new Granular.Exception($"Unsupported ResourceTypeCode ({resourceTypeCode})");
                }

                int length = binaryReader.ReadInt32();
                byte[] buffer = new byte[length];
                memoryStream.Read(buffer, 0, length);

                return new MemoryStream(buffer);
            }

            return null;
        }
    }
}
