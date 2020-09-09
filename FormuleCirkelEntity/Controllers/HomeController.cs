using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class HomeController : Controller
    {
        private readonly FormulaContext _context;

        public HomeController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            bool championship = false;
            string name = "Formula";
            if (_context.Championships.Any())
            {
                championship = true;
                var champname = _context.Championships.FirstOrDefault(c => c.ActiveChampionship);
                name = champname.ChampionshipName;
            }
            
            ViewBag.championship = championship;
            ViewBag.name = name;
            return View();
        }

        public IActionResult DriverStandings()
        {
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            if (seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

                ViewBag.lastpointpos = currentSeason.PointsPerPosition.Keys.Max();
                ViewBag.points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition);
                ViewBag.polepoint = currentSeason.PolePoints;
                ViewBag.seasonId = currentSeason.SeasonId;

                var standings = _context.SeasonDrivers
                    .Include(s => s.DriverResults)
                    .Include(s => s.Driver)
                    .Include(s => s.SeasonTeam.Team)
                    .Where(s => s.SeasonId == currentSeason.SeasonId)
                    .OrderByDescending(s => s.Points)
                    .ToList();

                ViewBag.rounds = _context.Races
                    .Where(r => r.SeasonId == currentSeason.SeasonId)
                    .Include(r => r.Track)
                    .OrderBy(r => r.Round).ToList();

                // Calculates the average finishing position
                List<AverageFinish> averageFinishes = new List<AverageFinish>();
                foreach (var driver in standings)
                {
                    List<double> positions = new List<double>();
                    double average = 0;
                    if (driver.DriverResults.Any())
                    {
                        foreach (var result in driver.DriverResults)
                        {
                            if (result.Status == Status.Finished)
                            {
                                positions.Add(result.Position);
                            }
                        }
                        if (positions.Any())
                        {
                            average = Math.Round((positions.Average()), 2);
                        }
                    }
                    AverageFinish avg = new AverageFinish()
                    {
                        Driver = driver,
                        Averagepos = average
                    };
                    averageFinishes.Add(avg);
                }
                ViewBag.averages = averageFinishes;

                return View(standings);
            } else
            {
                return RedirectToAction("Index", "Season");
            }
        }

        [ActionName("PastDriverStandings")]
        public IActionResult DriverStandings(int seasonId)
        {
            var currentSeason = _context.Seasons
                .Where(s => s.SeasonId == seasonId)
                .FirstOrDefault();

            int lastpoint = 10;
            if (currentSeason.PointsPerPosition.Count != 0)
            {
                lastpoint = currentSeason.PointsPerPosition.Keys.Max();
            }
            ViewBag.lastpointpos = lastpoint;
            ViewBag.points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition);
            ViewBag.polepoint = currentSeason.PolePoints;
            ViewBag.seasonId = seasonId;

            var standings = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam.Team)
                .Where(s => s.SeasonId == currentSeason.SeasonId)
                .OrderByDescending(s => s.Points)
                .ToList();

            // Calculates the average finishing position
            List<AverageFinish> averageFinishes = new List<AverageFinish>();
            foreach (var driver in standings)
            {
                List<double> positions = new List<double>();
                double average = 0;
                foreach (var result in driver.DriverResults)
                {
                    if (result.Status == Status.Finished)
                    {
                        positions.Add(result.Position);
                    }
                }
                if (positions.Any())
                {
                    average = Math.Round((positions.Average()), 2);
                }
                AverageFinish avg = new AverageFinish()
                {
                    Driver = driver,
                    Averagepos = average
                };
                averageFinishes.Add(avg);
            }
            ViewBag.averages = averageFinishes;

            ViewBag.rounds = _context.Races
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == currentSeason.SeasonId)
                .Include(r => r.Track)
                .OrderBy(r => r.Round).ToList();

            return View("DriverStandings", standings);
        }

        [HttpPost("[Controller]/{seasonId}/GetDriverGraphData")]
        public IActionResult GetDriverGraphData(int seasonId)
        {
            // Overweeg om dit te herschrijven dat hij de races pakt in plaats van de seizoenrijders, of vindt een manier om het zo te krijgen dat de racevolgorde klopt.
            var standings = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonId == seasonId)
                .OrderByDescending(sd => sd.Points)
                .Take(10)
                .Include(sd => sd.Driver)
                .Include(sd => sd.DriverResults)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(sd => sd.Team)
                .ToList();

            return new JsonResult(standings, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public IActionResult TeamStandings()
        {
            // Selects seasons from the currently activated championship
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            // Checks if there is any season is in progress, else return to season list
            if (seasons.Any(s => s.State == SeasonState.Progress))
            {
                var viewModel = new TeamStandingsModel() {
                    Locations = new List<string>()
                };

                var currentSeason = seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

                // Assigns the lowest position that scores points
                viewModel.LastPointPos = currentSeason.PointsPerPosition.Keys.Max();

                viewModel.SeasonTeams = _context.SeasonTeams
                    .Include(st => st.SeasonDrivers)
                    .Where(st => st.SeasonId == currentSeason.SeasonId)
                    .OrderByDescending(st => st.Points);

                var rounds = _context.Races
                    .Where(r => r.SeasonId == currentSeason.SeasonId)
                    .Include(r => r.Track)
                    .OrderBy(r => r.Round);

                foreach (var round in rounds)
                {
                    viewModel.Locations.Add(round.Track.Location.Substring(0,3).ToUpper(CultureInfo.CurrentCulture));
                }

                viewModel.Rounds = rounds.Select(r => r.RaceId);

                viewModel.DriverResults = _context.DriverResults
                    .Where(dr => dr.Race.SeasonId == currentSeason.SeasonId);

                ViewBag.points = JsonConvert.SerializeObject(currentSeason.PointsPerPosition);
                ViewBag.seasonId = currentSeason.SeasonId;
                ViewBag.polepoint = currentSeason.PolePoints;

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
            
        }

        [ActionName("PastTeamStandings")]
        public IActionResult TeamStandings(int seasonId)
        {
            var viewModel = new TeamStandingsModel()
            {
                Locations = new List<string>()
            };

            // Assigns the lowest position that scores points
            viewModel.LastPointPos = _context.Seasons.SingleOrDefault(s => s.SeasonId == seasonId).PointsPerPosition.Keys.Max();

            viewModel.SeasonTeams = _context.SeasonTeams
                .Include(st => st.SeasonDrivers)
                .Where(st => st.SeasonId == seasonId)
                .OrderByDescending(st => st.Points);

            var rounds = _context.Races
                .Where(r => r.SeasonId == seasonId)
                .Include(r => r.Track)
                .OrderBy(r => r.Round);

            foreach (var round in rounds)
            {
                viewModel.Locations.Add(round.Track.Location.Substring(0, 3).ToUpper(CultureInfo.CurrentCulture));
            }

            viewModel.Rounds = rounds.Select(r => r.RaceId);

            viewModel.DriverResults = _context.DriverResults
                .Where(dr => dr.Race.SeasonId == seasonId);

            return View("TeamStandings", viewModel);
        }

        [HttpPost("[Controller]/{seasonId}/GetTeamGraphData")]
        public IActionResult GetTeamGraphData(int seasonId)
        {
            var season = _context.Seasons.SingleOrDefault(s => s.SeasonId == seasonId);
            if (season == null)
                return null;

            var graphData = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Where(st => st.SeasonId == seasonId)
                .Include(st => st.SeasonDrivers)
                    .ThenInclude(sd => sd.DriverResults)
                .OrderBy(st => st.Name).ToList();

            return new JsonResult(graphData, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public IActionResult NextRace()
        {
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            if(seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .Include(r => r.Races)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

                var nextrace = currentSeason.Races
                    .OrderBy(r => r.Round)
                    .FirstOrDefault(r => r.StintProgress == 0);

                if (nextrace == null)
                {
                    return RedirectToAction("Index", "Season");
                }
                else
                {
                    return RedirectToAction("RacePreview", "Races", new { id = currentSeason.SeasonId, raceId = nextrace.RaceId });
                }
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class AverageFinish
    {
        public SeasonDriver Driver { get; set; }
        public double Averagepos { get; set; }
    }
}
