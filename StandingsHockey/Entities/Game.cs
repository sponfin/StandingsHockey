namespace StandingsHockey.Entities
{
    public partial class Game
    {
        public int Id { get; set; }

        public DateTime DateGame { get; set; }
        public string TimeGame { get; set; } = null!;
        public string ResultGame { get; set; } = null!;
        public int TourneyId { get; set; }
        public int TeamId1 { get; set; }
        public int TeamId2 { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public string Result1 { get; set; } = null!;
        public string Result2 { get; set; } = null!;
        public bool? IsSo { get; set; }

        public virtual Team TeamId1Navigation { get; set; } = null!;
        public virtual Team TeamId2Navigation { get; set; } = null!;
        public virtual Tourney Tourney { get; set; } = null!;
    }
}
