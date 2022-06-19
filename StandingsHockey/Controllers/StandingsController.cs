
using Microsoft.AspNetCore.Mvc;

namespace StandingsHockey.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StandingsController : ControllerBase
    {
        private readonly standingsContext _context;
        //private readonly IMapper _mapper;

        public StandingsController(standingsContext context)
        {
            _context = context;
            //_mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetStandings()
        {
            var games = await GetGames();
            //var result = _mapper.Map<StandingsContract[]>(games);


            return Ok(games);
            //return Ok(await _context.Games.Include(t=>t.TeamId1).ToArrayAsync());

        }

        [HttpGet("V2")] // Будет путь Standings/V2
        public async Task<IActionResult> GetStandingsV2()
        {
            var teams = await _context.Teams.ToDictionaryAsync(x => x.Id);
            var games = await _context.Games
                .Where(x => !string.IsNullOrWhiteSpace(x.Result1) && !string.IsNullOrWhiteSpace(x.Result2))
                .Select(x => new { x.TeamId1, x.TeamId2, x.Result1, x.Result2 })
                .Take(10)
                .ToArrayAsync();

            var result = games
                .Join(
                    teams,
                    g => g.TeamId1,
                    t => t.Key,
                    (g, t) => new { Name1 = t.Value.TeamName, Result1 = g.Result1, Result2 = g.Result2, g.TeamId2 })
                .Join(
                     teams,
                    g => g.TeamId2,
                    t => t.Key,
                    (g, t) => new { Name2 = t.Value.TeamName, g.Result1, g.Result2, g.Name1 });

            return Ok(result);
            //return Ok(await _context.Games.Include(t=>t.TeamId1).ToArrayAsync());

        }

        [HttpGet("V3")] // Будет путь Standings/V3
        public async Task<IActionResult> GetStandingsV3()
        {
            var teams = _context.Teams;
            var games = _context.Games
                .Where(x => !string.IsNullOrWhiteSpace(x.Result1) && !string.IsNullOrWhiteSpace(x.Result2))
                .Select(x => new { x.TeamId1, x.TeamId2, x.Result1, x.Result2 })
                .Take(10);


            var result = games
                .Join(
                    teams,
                    g => g.TeamId1,
                    t => t.Id,
                    (g, t) => new { Name1 = t.TeamName, Result1 = g.Result1, Result2 = g.Result2, g.TeamId2 })
                .Join(
                     teams,
                    g => g.TeamId2,
                    t => t.Id,
                    (g, t) => new { Name2 = t.TeamName, g.Result1, g.Result2, g.Name1 });

            var query = result.ToQueryString();


            return Ok(await result.ToArrayAsync());
            //return Ok(await _context.Games.Include(t=>t.TeamId1).ToArrayAsync());

        }


        public async Task<Domain.Standings[]> GetGames()
        {
            var games = await _context.Games

                //.Where(x => !string.IsNullOrWhiteSpace(x.Res1) && !string.IsNullOrWhiteSpace(x.Res2))
                .Where(x => x.Tourney.TourneyName == "НПХЛ-ОКХЛ 2020-2021 САРАТОВСКАЯ ОБЛ. (ДЕБЮТАНТ)")
                .Include(x => x.TeamId1Navigation)
                .Include(x => x.TeamId2Navigation)
                .Include(x => x.Tourney)

                //.Take(10)

                .Select(x => new Domain.Standings
                {
                    Name1 = x.TeamId1Navigation.TeamName,
                    Name2 = x.TeamId2Navigation.TeamName,
                    //Result1 = int.Parse(x.Res1),
                    //Result2 = int.Parse(x.Res2),
                    Result1 = x.Result1,
                    Result2 = x.Result2,
                    IsSO = x.IsSo,
                    Score1 = x.Score1,
                    Score2 = x.Score2,
                    Tourney = x.Tourney.TourneyName
                })
            .ToArrayAsync();

            return games;
        }
    }
}
