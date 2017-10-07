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
    }
}