namespace MyScout.Android
{
    public class Round
    {
        // Variables/Constants
        /// <summary>
        /// First 3 Teams are on the red alliance.
        /// The other 3 teams are on the blue alliance
        /// </summary>
        public TeamData[] TeamData = new TeamData[TeamCount];
        public const int TeamCount = 6;
    }

    public class TeamData
    {
        // Variables/Constants
        public object[] AutoData;
        public object[] TeleOPData;
        public Team Team;

        // Constructors
        public TeamData(Team team)
        {
            Team = team;
        }
    }
}