
using Microsoft.AspNetCore.Mvc;
using StandingsHockey.Domain;

namespace StandingsHockey.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly standingsContext _context;


        public StandingsController(standingsContext context)
        {
            _context = context;

        }



        [HttpGet]
        [Route("Games")]
        public async Task<IActionResult> GetGames()
        {

            var gamesByTurney = await GetGamesInTurney();
            return Ok(gamesByTurney);
        }


        [HttpGet]
        [Route("Teams")]
        public async Task<IActionResult> GetTeams()
        {

            var teams = await GetTeamsInTourney();
            return Ok(teams);
        }

        [HttpGet]
        public async Task<IActionResult> GetStandings()
        {

            var standings = await GetScore();
            return Ok(standings);
        }



        public async Task<GamesInTourney[]> GetGamesInTurney()
        {
            var result = await _context.Games

                .Where(x => x.Tourney.TourneyName == "НПХЛ-ОКХЛ 2020-2021 САРАТОВСКАЯ ОБЛ. (ДЕБЮТАНТ)")
                .Include(x => x.TeamId1Navigation)
                .Include(x => x.TeamId2Navigation)
                .Include(x => x.Tourney)
                .Select(x => new GamesInTourney
                {
                    DateGame = x.DateGame.ToString("dd.MM.yyyy"),
                    NameTeam1 = x.TeamId1Navigation.TeamName,
                    NameTeam2 = x.TeamId2Navigation.TeamName,
                    Result1 = x.Result1,
                    Result2 = x.Result2,
                    IsSO = x.IsSo,
                    Score1 = x.Score1,
                    Score2 = x.Score2,
                    Tourney = x.Tourney.TourneyName
                })
            .ToArrayAsync();

            return result;
        }

        public async Task<string[]> GetTeamsInTourney()
        {
            var gamesInTurney = await GetGamesInTurney();
            var teamsSet = new HashSet<string>();

            var team1List = gamesInTurney.Select(x => x.NameTeam1).ToList();
            var team2List = gamesInTurney.Select(x => x.NameTeam2).ToList();

            for (int i = 0; i < team1List.Count; i++)
            {
                if (team1List[i] != null)
                    teamsSet.Add(team1List[i]);

            }
            for (int i = 0; i < team2List.Count; i++)
            {
                if (team1List[i] != null)
                    teamsSet.Add(team2List[i]);

            }
            string[]? teams = teamsSet.ToArray(); // Возвращаем из HashSet массив
            return teams;
        }


        public async Task<List<Standings>> GetScore()
        {
            var gamesInTurney = await GetGamesInTurney();
            var teamsSet = await GetTeamsInTourney();
            //var teams = teamsSet.Result; //Возвращаем значение из задачи метод Result если вызываем метод без await
            var teams = teamsSet;

            var result = new List<Standings>();

            for (int i = 0; i < teams.Count(); i++)
            {
                int totalScoreSum = 0;

                totalScoreSum = gamesInTurney.Where(x => x.NameTeam1 == teams[i]).Sum(x => x.Score1);
                totalScoreSum += gamesInTurney.Where(x => x.NameTeam2 == teams[i]).Sum(x => x.Score2);


                result.Add(new Standings()
                {
                    NameTeam = teams[i],
                    Score = totalScoreSum
                    //ResultSeason = _db.Seasons.Single(x => x.SeasonId == id),
                    //TotalScore = totalScoreSum,
                    //WinCount = _db.Gameses.Count(x => x.WinTeam.TeamId == tempListTeam1[i])
                });
            }
            result = result.OrderByDescending(x => x.Score).ToList();
            return result;
        }
    }
}
