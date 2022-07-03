namespace StandingsHockey.Domain
{
    public class Standings
    {
        public string? NameTeam { get; set; }
        public int Points { get; set; }
        public int Wins { get; set; }
        public int WinsSO { get; set; }
        public int Losses { get; set; }
        public int LossesSO { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifferential { get; set; }



    }
}

