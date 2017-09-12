namespace MyScout.Android
{
    public class Round
    {
        // Variables/Constants
        public TeamData[] TeamData = new TeamData[6];
    }

    public class TeamData
    {
        // Variables/Constants
        public object[] AutoData;
        public object[] TeleOPData;
        public int TeamIndex;

        // Constructors
        public TeamData(int teamIndex, DataSet dataSet)
        {
            TeamIndex = teamIndex;
            AutoData = new object[dataSet.RoundAutoData.Count];
            TeleOPData = new object[dataSet.RoundTeleOPData.Count];
        }
    }
}