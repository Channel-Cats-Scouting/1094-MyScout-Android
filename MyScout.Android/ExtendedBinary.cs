using System.Text;
using System.IO;

namespace MyScout.Android
{
    public class ExtendedBinaryReader : BinaryReader
    {
        // Constructors
        public ExtendedBinaryReader(Stream input) : base(input, Encoding.UTF8, true) { }
        public ExtendedBinaryReader(Stream input, Encoding encoding,
            bool leaveOpen = true) : base(input, encoding, leaveOpen) { }

        // Methods
        public virtual Team ReadTeam()
        {
            string name = ReadString();
            string id = ReadString();
            return new Team(name, id);
        }

        public virtual byte[] ReadInChunks(int length, int chunkSize)
        {
            // Read the data through several smaller pieces
            byte[] data = new byte[length];
            int bytes = 0;

            while (bytes < length)
            {
                int size = ((length - bytes) >= chunkSize) ?
                    chunkSize : length - bytes;

                int received = Read(data, bytes, size);
                bytes += received;
            }

            return data;
        }
    }

    public class ExtendedBinaryWriter : BinaryWriter
    {
        // Constructors
        public ExtendedBinaryWriter(Stream output) : base(output, Encoding.UTF8, true) { }
        public ExtendedBinaryWriter(Stream output, Encoding encoding,
            bool leaveOpen = true) : base(output, encoding, leaveOpen) { }

        // Methods
        public virtual void Write(Team team)
        {
            Write(team.Name);
            Write(team.ID);
        }

        public virtual void WriteInChunks(byte[] data, int chunkSize)
        {
            // Write the given data in several smaller pieces
            int bytes = 0, dataLength = data.Length;
            while (bytes < dataLength)
            {
                int size = ((bytes + chunkSize) <= dataLength) ?
                    chunkSize : dataLength - bytes;

                Write(data, bytes, size);
                bytes += size;
            }
        }
    }
}