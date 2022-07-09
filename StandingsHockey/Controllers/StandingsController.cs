
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
            var teams = await GetTeamsNamesInTourney();
            return Ok(teams);
        }

        [HttpGet]
        public async Task<IActionResult> GetStandings()
        {
            var standings = await GetScore();
            var teamsGroupByScore = standings.GroupBy(x => x.Points);
            var teamsWithSameScore = teamsGroupByScore.Where(x => x.Count() > 1);
            var gamesInTurney = await GetGamesInTurney();

            foreach (var teams in teamsWithSameScore)
            {
                var queue = new Queue<Standings>(teams.Select(x => x));

                foreach (var team in teams)
                {

                }

                // int totalScoreSum = 0;

                // totalScoreSum = gamesInTurney
                //    .Where(x => x.NameTeam1 == teams)
                //    .Sum(x => x.Score1);

            }

            return Ok(standings);
        }

        public async Task<GamesInTourney[]> GetGamesInTurney()
        {
            var result = await _context
                .Games
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

        public async Task<string[]> GetTeamsNamesInTourney()
        {
            var gamesInTurney = await GetGamesInTurney();
            // var teamsNames = new HashSet<string>();

            var team1Name = gamesInTurney
                .Select(x => x.NameTeam1)
                .Where(x => x != null)
                .ToHashSet();
            var team2Name = gamesInTurney
                .Select(x => x.NameTeam2)
                .Where(x => x != null)
                .ToHashSet();

            team1Name.UnionWith(team2Name);
            return team1Name.ToArray(); // Возвращаем из HashSet массив
        }

        private int ResultToIntCount(string result)
        {
            bool isInt = int.TryParse(result, out var resultIntCount);
            if (isInt)
            {
                return resultIntCount;
            }
            else
            {
                if (result == "+")
                {
                    resultIntCount = 1;
                }
                else if (result == "-")
                {
                    resultIntCount = 0;
                }
                else
                {
                    throw new FormatException($"Result was incorrect {result}");
                }
            }

            return resultIntCount;
        }

        private int ResultToIntSum(string result)
        {
            int.TryParse(result, out var resultIntCountSum);
            return resultIntCountSum;
        }

        public async Task<List<Standings>> GetScore()
        {
            var gamesInTurney = await GetGamesInTurney();
            var teamsNames = await GetTeamsNamesInTourney();

            // var teams = teamsNames.Result; //Возвращаем значение из задачи метод Result если вызываем метод без await


            var result = new List<Standings>();

            foreach (var teamName in teamsNames)
            {
                int totalScoreSum = 0;

                totalScoreSum = gamesInTurney.Where(x => x.NameTeam1 == teamName).Sum(x => x.Score1);
                totalScoreSum += gamesInTurney.Where(x => x.NameTeam2 == teamName).Sum(x => x.Score2);

                int wins = 0;
                wins = gamesInTurney.Where(x => x.NameTeam1 == teamName).Count(x => ResultToIntCount(x.Result1) > ResultToIntCount(x.Result2) && x.IsSO == false);
                wins += gamesInTurney.Where(x => x.NameTeam2 == teamName).Count(x => ResultToIntCount(x.Result2) > ResultToIntCount(x.Result1) && x.IsSO == false);

                int winsSo = 0;
                winsSo = gamesInTurney.Where(x => x.NameTeam1 == teamName).Count(x => ResultToIntCount(x.Result1) > ResultToIntCount(x.Result2) && x.IsSO == true);
                winsSo += gamesInTurney.Where(x => x.NameTeam2 == teamName).Count(x => ResultToIntCount(x.Result2) > ResultToIntCount(x.Result1) && x.IsSO == true);

                int losses = 0;
                losses = gamesInTurney.Where(x => x.NameTeam1 == teamName).Count(x => ResultToIntCount(x.Result1) < ResultToIntCount(x.Result2) && x.IsSO == false);
                losses += gamesInTurney.Where(x => x.NameTeam2 == teamName).Count(x => ResultToIntCount(x.Result2) < ResultToIntCount(x.Result1) && x.IsSO == false);

                int lossesSo = 0;
                lossesSo = gamesInTurney.Where(x => x.NameTeam1 == teamName).Count(x => ResultToIntCount(x.Result1) < ResultToIntCount(x.Result2) && x.IsSO == true);
                lossesSo += gamesInTurney.Where(x => x.NameTeam2 == teamName).Count(x => ResultToIntCount(x.Result2) < ResultToIntCount(x.Result1) && x.IsSO == true);

                int goalsFor = 0;
                goalsFor = gamesInTurney.Where(x => x.NameTeam1 == teamName).Sum(x => ResultToIntSum(x.Result1));
                goalsFor += gamesInTurney.Where(x => x.NameTeam2 == teamName).Sum(x => ResultToIntSum(x.Result2));

                int goalsAgainst = 0;
                goalsAgainst = gamesInTurney.Where(x => x.NameTeam1 == teamName).Sum(x => ResultToIntSum(x.Result2));
                goalsAgainst += gamesInTurney.Where(x => x.NameTeam2 == teamName).Sum(x => ResultToIntSum(x.Result1));

                int goalDifferential = 0;
                goalDifferential = goalsFor - goalsAgainst;

                result.Add(new Standings()
                {
                    NameTeam = teamName,
                    Points = totalScoreSum,
                    Wins = wins,
                    WinsSO = winsSo,
                    Losses = losses,
                    LossesSO = lossesSo,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    GoalDifferential = goalDifferential,

                    // ResultSeason = _db.Seasons.Single(x => x.SeasonId == id),
                    // TotalScore = totalScoreSum,
                    // WinCount = _db.Gameses.Count(x => x.WinTeam.TeamId == tempListTeam1[i])
                });
            }

            result = result.OrderByDescending(x => x.Points).ToList();
            return result;
        }
    }
}
