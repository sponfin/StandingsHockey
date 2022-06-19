using System;
using System.Collections.Generic;

namespace StandingsHockey.Entities
{
    public partial class Tourney
    {
        public Tourney()
        {
            Games = new HashSet<Game>();
        }

        public int Id { get; set; }
        public string TourneyName { get; set; } = null!;
        public int PointsWin { get; set; }
        public int PointsLose { get; set; }
        public int PointsWinOverTime { get; set; }
        public int PontsLoseOverTime { get; set; }
        public int PointsTie { get; set; }

        public virtual ICollection<Game> Games { get; set; }
    }
}
