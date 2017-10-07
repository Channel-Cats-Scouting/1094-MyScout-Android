using System.Collections.Generic;
using System.IO;

namespace MyScout.Android
{
    public static class Config
    {
        // Variables/Constants
        public static List<string> RegisteredDevices = new List<string>();
        public static string FilePath
        {
            get
            {
                // Gets the full path to the config file
                return Path.Combine(
                    IO.AppDirectory, FileName);
            }
        }

        public static TabletTypes TabletType;
        public static bool Loaded = false;

        public const string FileName = "Config.bin";
        private const string Signature = "CFG";
        private const byte Version = 1;

        // Methods
        public static bool Load()
        {
            // Return if the config file doesn't exist
            string filePath = FilePath; // This is so we don't call the FilePath getter twice
            if (!File.Exists(filePath)) return false;

            // Open a stream to the config file and read it's contents
            using (var fileStream = File.OpenRead(filePath))
            using (var reader = new ExtendedBinaryReader(fileStream))
            {
                // Read header
                string sig = reader.ReadSignature(3);
                if (sig != Signature)
                    return false;

                byte version = reader.ReadByte();
                if (Version < version)
                    return false;

                // Read config values
                TabletType = (TabletTypes)reader.ReadByte();

                // Read registered devices
                ushort devicesCount = reader.ReadUInt16();
                RegisteredDevices.Clear();

                for (int i = 0; i < devicesCount; ++i)
                {
                    RegisteredDevices.Add(reader.ReadString());
                }
            }

            Loaded = true;
            return true;
        }

        public static void Save()
        {
            // Open a stream and write the config file
            using (var fileStream = File.OpenWrite(FilePath))
            using (var writer = new ExtendedBinaryWriter(fileStream))
            {
                // Write header
                writer.WriteSignature(Signature);
                writer.Write(Version);

                // Write config values
                writer.Write((byte)TabletType);

                // Write registered devices
                writer.Write((ushort)RegisteredDevices.Count);
                foreach (string registeredDevice in RegisteredDevices)
                {
                    writer.Write(registeredDevice);
                }
            }
        }

        // Other
        public enum TabletTypes
        {
            Scout, ScoutMaster
        }
    }
}