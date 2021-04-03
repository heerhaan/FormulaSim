using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class HomeController : FormulaController
    {
        private readonly IChampionshipService _championships;
        private readonly ISeasonService _seasons;
        private readonly IRaceService _races;
        private readonly ISeasonDriverService _seasonDrivers;
        private readonly ISeasonTeamService _seasonTeams;

        public HomeController(FormulaContext context,
            UserManager<SimUser> userManager,
            IChampionshipService championshipService,
            ISeasonService seasonService,
            IRaceService raceService,
            ISeasonDriverService seasonDriverService,
            ISeasonTeamService seasonTeamService)
            : base(context, userManager)
        {
            _championships = championshipService;
            _seasons = seasonService;
            _races = raceService;
            _seasonDrivers = seasonDriverService;
            _seasonTeams = seasonTeamService;
        }

        public async Task<IActionResult> Index(string message = null)
        {
            bool championship = false;
            string name = "Formula";
            var championships = await _championships.GetChampionships();
            if (championships.Count > 0)
            {
                championship = true;
                var champname = championships.FirstOrDefault(c => c.ActiveChampionship);
                name = champname.ChampionshipName;
            }

            var viewModel = new HomeViewModel
            {
                ChampionshipName = name,
                Message = message,
                ChampExists = championship
            };
            return View(viewModel);
        }

        public async Task<IActionResult> DriverStandings()
        {
            var season = await _seasons.FindActiveSeason();
            if (season != null)
            {
                var drivers = await _seasonDrivers.GetRankedSeasonDrivers(season.SeasonId, true, true);
                var rounds = await _races.GetOrderedRaces(season.SeasonId, true);
                var tracks = rounds.Select(res => res.Track);

                var viewmodel = new HomeDriverStandingsModel
                {
                    SeasonDrivers = drivers,
                    Averages = _seasonDrivers.CalculateDriverAverages(drivers),
                    Tracks = tracks,
                    Rounds = rounds.Select(r => r.RaceId),
                    SeasonId = season.SeasonId,
                    Year = season.SeasonNumber,
                    LastPointPos = season.PointsPerPosition.Keys.Max(),
                    Points = JsonConvert.SerializeObject(season.PointsPerPosition),
                    PolePoints = season.PolePoints
                };
                return View(viewmodel);
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
        }

        [ActionName("PastDriverStandings")]
        public async Task<IActionResult> DriverStandings(int seasonId)
        {
            var season = await _seasons.GetSeasonById(seasonId);
            var drivers = await _seasonDrivers.GetRankedSeasonDrivers(season.SeasonId, true, true);
            var rounds = await _races.GetOrderedRaces(seasonId, true);
            var tracks = rounds.Select(res => res.Track);

            var viewmodel = new HomeDriverStandingsModel
            {
                SeasonDrivers = drivers,
                Averages = _seasonDrivers.CalculateDriverAverages(drivers),
                Tracks = tracks,
                Rounds = rounds.Select(r => r.RaceId),
                SeasonId = season.SeasonId,
                Year = season.SeasonNumber,
                LastPointPos = season.PointsPerPosition.Keys.Max(),
                Points = JsonConvert.SerializeObject(season.PointsPerPosition),
                PolePoints = season.PolePoints
            };

            return View("DriverStandings", viewmodel);
        }

        [HttpPost("[Controller]/{seasonId}/GetDriverGraphData")]
        public async Task<IActionResult> GetDriverGraphData(int seasonId)
        {
            var query = _seasonDrivers.GetQueryable();
            var standings = await query.IgnoreQueryFilters().AsNoTracking()
                .Where(sd => sd.SeasonId == seasonId)
                .OrderByDescending(sd => sd.Points)
                .Take(10)
                .Include(sd => sd.Driver)
                .Include(sd => sd.DriverResults)
                .Include(sd => sd.SeasonTeam)
                .ToListAsync();

            return new JsonResult(standings, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public async Task<IActionResult> TeamStandings()
        {
            // Selects seasons from the currently activated championship
            var season = await _seasons.FindActiveSeason();

            // Checks if there is any season is in progress, else return to season list
            if (season != null)
            {
                var rounds = await _races.GetOrderedRaces(season.SeasonId, true);
                var tracks = rounds.Select(res => res.Track);

                var viewModel = new HomeTeamStandingsModel
                {
                    // Assigns the lowest position that scores points
                    LastPointPos = season.PointsPerPosition.Keys.Max(),
                    SeasonTeams = await _seasonTeams.GetRankedSeasonTeams(season.SeasonId, true),
                    Tracks = tracks,
                    Rounds = rounds.Select(r => r.RaceId),
                    DriverResults = await Context.DriverResults
                        .Where(dr => dr.Race.SeasonId == season.SeasonId)
                        .ToListAsync(),
                    SeasonId = season.SeasonId,
                    Year = season.SeasonNumber,
                    Points = JsonConvert.SerializeObject(season.PointsPerPosition),
                    PolePoints = season.PolePoints
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
        }

        [ActionName("PastTeamStandings")]
        public async Task<IActionResult> TeamStandings(int seasonId)
        {
            var season = await _seasons.GetSeasonById(seasonId);
            var rounds = await _races.GetOrderedRaces(seasonId, true);
            var tracks = rounds.Select(res => res.Track);

            var viewModel = new HomeTeamStandingsModel
            {
                // Assigns the lowest position that scores points
                LastPointPos = season.PointsPerPosition.Keys.Max(),
                SeasonTeams = await _seasonTeams.GetRankedSeasonTeams(seasonId, true),
                Tracks = tracks,
                Rounds = rounds.Select(r => r.RaceId),
                DriverResults = await Context.DriverResults
                    .Where(dr => dr.Race.SeasonId == seasonId)
                    .ToListAsync(),
                SeasonId = season.SeasonId,
                Year = season.SeasonNumber,
                Points = JsonConvert.SerializeObject(season.PointsPerPosition),
                PolePoints = season.PolePoints
            };

            return View("TeamStandings", viewModel);
        }

        public async Task<IActionResult> NextRace()
        {
            var season = await _seasons.FindActiveSeason(true);
            var nextrace = _races.FindNextRace(season);

            if (nextrace is null)
            {
                return RedirectToAction("Index", "Season");
            }
            else
            {
                return RedirectToAction("RacePreview", "Races", new { id = season.SeasonId, raceId = nextrace.RaceId });
            }
        }

        public IActionResult AboutMe()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
