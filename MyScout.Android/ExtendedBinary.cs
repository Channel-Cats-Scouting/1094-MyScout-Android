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
    }
}