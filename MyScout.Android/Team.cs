namespace MyScout.Android
{
    public class Team
    {
        // Variables/Constants
        public object[] PreScoutingData; // TODO: Use this array
        public string Name, ID;

        // Constructors
        public Team(string name)
        {
            Name = name;
        }

        public Team(string name, string id)
        {
            Name = name;
            ID = id;
        }

        // Methods
        public override string ToString()
        {
            return (string.IsNullOrEmpty(ID)) ?
                Name : $"{Name} - {ID}";
        }
    }
}