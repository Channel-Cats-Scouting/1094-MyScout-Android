﻿using System;

namespace MyScout.Android
{
    public static class IO
    {
        // Variables/Constants
        public static string AppDirectory
        {
            get
            {
                return Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
            }
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