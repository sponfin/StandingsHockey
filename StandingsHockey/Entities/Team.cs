using System;
using System.Collections.Generic;

namespace StandingsHockey.Entities
{
    public partial class Team
    {
        public Team()
        {
            GameTeamId1Navigations = new HashSet<Game>();
            GameTeamId2Navigations = new HashSet<Game>();
        }

        public int Id { get; set; }
        public string TeamName { get; set; } = null!;
        public string? Logo { get; set; }

        public virtual ICollection<Game> GameTeamId1Navigations { get; set; }
        public virtual ICollection<Game> GameTeamId2Navigations { get; set; }
    }
}
