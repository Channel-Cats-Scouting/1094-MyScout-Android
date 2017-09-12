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
        public static AllianceColors TabletColor;
        public static bool Loaded = false;

        public const string FileName = "Config.bin";
        public const byte MajorVersion = 1, MinorVersion = 0;

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
                // Read version number
                byte majorVersion = reader.ReadByte();
                byte minorVersion = reader.ReadByte();

                // Make sure the version number from this config file is supported
                if (!IO.IsVersionOK(majorVersion, minorVersion,
                    MajorVersion, MinorVersion))
                {
                    return false;
                }

                // Read config values
                TabletType = (TabletTypes)reader.ReadByte();
                TabletColor = (AllianceColors)reader.ReadByte();

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
                // Write version number
                writer.Write(MajorVersion);
                writer.Write(MinorVersion);

                // Write config values
                writer.Write((byte)TabletType);
                writer.Write((byte)TabletColor);

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

        public enum AllianceColors
        {
            Red, Blue
        }
    }
}