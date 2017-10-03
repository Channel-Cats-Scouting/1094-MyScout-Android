using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyScout.Android
{
    public class Event
    {
        // Variables/Constants
        public static Event Current;
        public List<Team> Teams = new List<Team>();
        public List<Round> Rounds = new List<Round>();
        public string Name, DataSetFileName;

        public int CurrentRoundIndex = -1, Index = -1;
        public Round CurrentRound
        {
            get
            {
                if (CurrentRoundIndex < 0 || CurrentRoundIndex >= Rounds.Count)
                    return null;

                return Rounds[CurrentRoundIndex];
            }
        }

        public static string EventsDirectory
        {
            get => Path.Combine(
                IO.ExternalDataDirectory, EventsDirName);
        }
        
        public const string EventsDirName = "Events",
            FileNamePrefix = "Event", Extension = ".event";

        protected const string Signature = "EVN";
        protected const byte Version = 1; // The highest-supported version of the format

        // Methods
        public static Event GetEventEntry(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                return GetEventEntry(fs);
            }
        }

        public static Event GetEventEntry(Stream fileStream)
        {
            // Read event header data
            var evnt = new Event();
            return (evnt.LoadHeaderData(fileStream)) ?
                evnt : null;
        }

        public void Load(int index)
        {
            // Get the event's file path
            Index = index;
            string filePath = Path.Combine(EventsDirectory,
                $"{FileNamePrefix}{index}{Extension}");

            // Load the event
            Load(filePath);
        }

        public void Load(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
            {
                Load(fs);
            }
        }

        public void Load(Stream fileStream)
        {
            // Header
            var reader = new ExtendedBinaryReader(
                fileStream, Encoding.UTF8, true);

            if (!LoadHeaderData(reader))
                return;

            // Teams
            int teamCount = reader.ReadInt32();
            Teams.Clear();

            for (int i = 0; i < teamCount; ++i)
            {
                Teams.Add(reader.ReadTeam());
            }

            // Rounds
            int roundCount = reader.ReadInt32();
            CurrentRoundIndex = reader.ReadInt32();

            Rounds.Clear();
            if (CurrentRoundIndex >= roundCount || CurrentRoundIndex < 0)
                CurrentRoundIndex = (roundCount - 1);

            for (int i = 0; i < roundCount; ++i)
            {
                Rounds.Add(ReadRoundData(reader, DataSet.Current));
            }

            reader.Close();
        }

        protected Round ReadRoundData(ExtendedBinaryReader reader, DataSet dataSet)
        {
            // Read per-round data for each team
            var round = new Round();
            for (int i = 0; i < Round.TeamCount; ++i)
            {
                // Read Team Index
                int teamIndex = reader.ReadInt32();
                if (teamIndex < 0) continue;

                var teamData = new TeamData(Teams[teamIndex]);

                // Read Autonomous Data
                if (reader.ReadBoolean())
                {
                    teamData.AutoData = new object[dataSet.RoundAutoData.Count];
                    for (int i2 = 0; i2 < dataSet.RoundAutoData.Count; ++i2)
                    {
                        teamData.AutoData[i2] = reader.ReadByType(
                            dataSet.RoundAutoData[i2].DataType);
                    }
                }

                // Read Tele-OP Data
                if (reader.ReadBoolean())
                {
                    teamData.TeleOPData = new object[dataSet.RoundTeleOPData.Count];
                    for (int i2 = 0; i2 < dataSet.RoundTeleOPData.Count; ++i2)
                    {
                        teamData.TeleOPData[i2] = reader.ReadByType(
                            dataSet.RoundTeleOPData[i2].DataType);
                    }
                }

                round.TeamData[i] = teamData;
            }

            return round;
        }

        public void Save()
        {
            // Create the Events directory if it does not exist
            string eventsDir = EventsDirectory; // This is so we don't keep calling the getter
            Directory.CreateDirectory(eventsDir);

            // Get the event's file path
            string filePath = Path.Combine(
                eventsDir, $"{FileNamePrefix}{Index}{Extension}");

            if (Index <= 0)
            {
                Index = 0;
                do
                {
                    filePath = Path.Combine(eventsDir,
                        $"{FileNamePrefix}{++Index}{Extension}");
                }
                while (File.Exists(filePath));
            }

            // Save the event
            Save(filePath);
        }

        public void Save(string filePath)
        {
            using (var fs = File.Create(filePath))
            {
                Save(fs);
            }
        }

        public void Save(Stream fileStream)
        {
            // Header
            var writer = new ExtendedBinaryWriter(
                fileStream, Encoding.UTF8, true);

            writer.WriteSignature(Signature);
            writer.Write(Version);
            writer.Write(Name);
            writer.Write(DataSetFileName);

            // Teams
            writer.Write(Teams.Count);
            foreach (var team in Teams)
            {
                writer.Write(team);
            }

            // Rounds
            writer.Write(Rounds.Count);
            writer.Write(CurrentRoundIndex);
            
            foreach (var round in Rounds)
            {
                WriteRoundData(writer, DataSet.Current, round);
            }

            writer.Close();
        }

        protected void WriteRoundData(ExtendedBinaryWriter writer,
            DataSet dataSet, Round round)
        {
            // Write per-round data for each team
            for (int i = 0; i < Round.TeamCount; ++i)
            {
                if (i >= round.TeamData.Length || round.TeamData[i] == null)
                {
                    writer.Write(-1);
                    continue;
                }

                // Write Team Index
                var teamData = round.TeamData[i];
                writer.Write(Teams.IndexOf(teamData.Team));

                // Write Autonomous Data
                bool hasAutoData = (teamData.AutoData != null);
                writer.Write(hasAutoData);

                if (hasAutoData)
                {
                    for (int i2 = 0; i2 < teamData.AutoData.Length; ++i2)
                    {
                        writer.WriteByType(teamData.AutoData[i2],
                            dataSet.RoundAutoData[i2].DataType);
                    }
                }

                // Write Tele-OP Data
                bool hasTeleOPData = (teamData.TeleOPData != null);
                writer.Write(hasTeleOPData);

                if (hasTeleOPData)
                {
                    for (int i2 = 0; i2 < teamData.TeleOPData.Length; ++i2)
                    {
                        writer.WriteByType(teamData.TeleOPData[i2],
                            dataSet.RoundTeleOPData[i2].DataType);
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        protected bool LoadHeaderData(Stream fileStream)
        {
            var reader = new ExtendedBinaryReader(
                fileStream, Encoding.UTF8, true);

            return LoadHeaderData(reader);
        }

        protected bool LoadHeaderData(ExtendedBinaryReader reader)
        {
            if (reader.ReadSignature() != Signature)
                return false;

            if (reader.ReadByte() > Version)
                return false;

            Name = reader.ReadString();
            DataSetFileName = reader.ReadString();
            return true;
        }
    }
}