using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace MyScout.Android
{
    public class DataSet
    {
        // Variables/Constants
        public static DataSet Current;
        public static string CurrentFileName;

        public List<DataPoint> PreScoutingData = new List<DataPoint>();
        public List<DataPoint> RoundAutoData = new List<DataPoint>();
        public List<DataPoint> RoundTeleOPData = new List<DataPoint>();
        public string Name, Author;

        public const float MaxSupportedVersion = 1.0f;

        // Methods
        public void Load(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                Load(fs, Path.GetFileNameWithoutExtension(filePath));
            }
        }

        public void Load(Stream fileStream, string defaultName = null)
        {
            // Get root attributes
            var xml = XDocument.Load(fileStream);
            var root = xml.Root;

            var nameAttr = root.Attribute("name");
            var versionAttr = root.Attribute("version");
            var authorAttr = root.Attribute("author");

            // Check DataSet version
            if (versionAttr != null && float.TryParse(versionAttr.Value, out float ver))
            {
                // Return if the loaded DataSet's version is unsupported
                if (ver > MaxSupportedVersion) return;
            }

            // Assign name and author variables
            Name = (nameAttr == null) ? defaultName : nameAttr.Value;
            Author = authorAttr?.Value;

            // Load Pre-Scouting Data
            var preScoutData = root.Element("PreScoutData");
            ReadDataPoints(preScoutData, PreScoutingData);

            // Get Round Data elements
            var roundData = root.Element("RoundData");
            if (roundData == null) return;

            // Load Autonomous Round Data
            var autoRoundData = roundData.Element("Autonomous");
            ReadDataPoints(autoRoundData, RoundAutoData);

            // Load Tele-OP Round Data
            var teleOPRoundData = roundData.Element("TeleOp");
            ReadDataPoints(teleOPRoundData, RoundTeleOPData);
        }

        protected void ReadDataPoints(XElement element, List<DataPoint> pointList)
        {
            if (element == null) return;
            foreach (var point in element.Elements("Data"))
            {
                // Get attributes from XML
                var nameAttr = point.Attribute("name");
                var typeAttr = point.Attribute("type");
                if (nameAttr == null || typeAttr == null) continue;

                // Get data type from string in XML
                var type = GetDataType(typeAttr.Value);
                if (type == null || string.IsNullOrEmpty(nameAttr.Value)) continue;

                // Make data point from loaded information and add it to the given list
                pointList.Add(new DataPoint(nameAttr.Value, type));
            }
        }

        public static Type GetDataType(string type)
        {
            // Returns a System.Type from a string
            // Supports booleans, strings, and some primitive number types
            // Nothing else is needed for DataPoints
            switch (type.ToLower())
            {
                case "bool":
                case "boolean":
                    return typeof(bool);

                case "int":
                case "integer":
                case "int32":
                case "sint32":
                    return typeof(int);

                case "float":
                case "single":
                    return typeof(float);

                case "double":
                    return typeof(double);

                case "string":
                    return typeof(string);
            }

            return null;
        }

        public void FillPreScoutGUI(LinearLayout layout)
        {
            FillLinearLayout(layout, PreScoutingData);
        }

        public void FillAutonomousGUI(LinearLayout layout)
        {
            FillLinearLayout(layout, RoundAutoData);
        }

        public void FillTeleOPGUI(LinearLayout layout)
        {
            FillLinearLayout(layout, RoundTeleOPData);
        }

        protected void FillLinearLayout(
            LinearLayout layout, List<DataPoint> points)
        {
            var context = layout.Context;
            foreach (var point in points)
            {
                layout.AddView(point.GetGUIWidget(context));
            }
        }
    }
}