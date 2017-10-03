using System;
using System.Text;
using System.IO;
using Android.Util;

namespace MyScout.Android
{
    public class ExtendedBinaryReader : BinaryReader
    {
        // Constructors
        public ExtendedBinaryReader(Stream input) : base(input, Encoding.UTF8, false) { }
        public ExtendedBinaryReader(Stream input, Encoding encoding,
            bool leaveOpen = false) : base(input, encoding, leaveOpen) { }

        // Methods
        public virtual string ReadSignature(int length = 3)
        {
            var sig = new char[length];
            Read(sig, 0, length);
            return new string(sig);
        }

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

        public virtual object ReadByType(Type type)
        {
            // Only supports types that DataPoints support
            if (type == typeof(bool))
            {
                return ReadBoolean();
            }
            else if (type == typeof(int))
            {
                return ReadInt32();
            }
            else if (type == typeof(float))
            {
                return ReadSingle();
            }
            else if (type == typeof(double))
            {
                return ReadDouble();
            }
            else if (type == typeof(string))
            {
                return ReadString();
            }

            return null;
        }
    }

    public class ExtendedBinaryWriter : BinaryWriter
    {
        // Constructors
        public ExtendedBinaryWriter(Stream output) : base(output, Encoding.UTF8, false) { }
        public ExtendedBinaryWriter(Stream output, Encoding encoding,
            bool leaveOpen = false) : base(output, encoding, leaveOpen) { }

        // Methods
        public virtual void WriteSignature(string sig)
        {
            Write(sig.ToCharArray());
        }

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

        public virtual void WriteByType(object data, Type type)
        {
            // Only supports types that DataPoints support
            if (type == typeof(bool))
            {
                Write((bool)data);
            }
            else if (type == typeof(int))
            {
                Write((int)data);
            }
            else if (type == typeof(float))
            {
                Write((float)data);
            }
            else if (type == typeof(double))
            {
                Write((double)data);
            }
            else if (type == typeof(string))
            {
                Write((string)data);
            }
        }
    }
}