using Android.OS;
using System.IO;

namespace MyScout.Android
{
    public static class IO
    {
        // Variables/Constants
        public static string AppDirectory
        {
            get => System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
        }

        public static string ExternalDataDirectory
        {
            get => Path.Combine(
                    Environment.ExternalStorageDirectory.AbsolutePath, "MyScout");
        }

        public static string DataSetDirectory
        {
            get => Path.Combine(ExternalDataDirectory, "DataSets");
        }

        // Methods
        public static bool IsVersionOK(byte majorVersion, byte minorVersion,
            byte majorSupportedVersion, byte minorSupportedVersion)
        {
            // Get the version numbers as floats
            float version = GetVersionFloat(majorVersion, minorVersion);
            float supportedVesrion = GetVersionFloat(
                majorSupportedVersion, minorSupportedVersion);

            // Return whether or not it's ok
            return (version >= supportedVesrion);
        }

        public static float GetVersionFloat(byte majorVersion, byte minorVersion)
        {
            // Return the version number as a float
            return majorVersion + (minorVersion * 0.1f);
        }
    }
}