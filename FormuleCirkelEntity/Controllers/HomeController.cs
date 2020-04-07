using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
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
            var season = _context.Seasons.SingleOrDefault(s => s.SeasonId == seasonId);
            if (season == null)
                return null;

            // Overweeg om dit te herschrijven dat hij de races pakt in plaats van de seizoenrijders, of vindt een manier om het zo te krijgen dat de racevolgorde klopt.
            var standings = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(sd => sd.Driver)
                .Include(sd => sd.DriverResults)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(sd => sd.Team)
                .Where(sd => sd.SeasonId == seasonId)
                .OrderBy(sd => sd.SeasonTeam.Team.Abbreviation)
                .ToList();

            return new JsonResult(standings, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        public IActionResult TeamStandings()
        {
            var seasons = _context.Seasons.Where(s => s.Championship.ActiveChampionship);

            if (seasons.Any(s => s.State == SeasonState.Progress))
            {
                var currentSeason = seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

                ViewBag.lastpointpos = currentSeason.PointsPerPosition.Keys.Max();

                var standings = _context.SeasonTeams
                    .Include(t => t.SeasonDrivers)
                        .ThenInclude(s => s.DriverResults)
                    .Where(s => s.SeasonId == currentSeason.SeasonId)
                    .OrderByDescending(t => t.Points)
                    .ToList();

                ViewBag.rounds = _context.Races
                    .Where(r => r.SeasonId == currentSeason.SeasonId)
                    .Include(r => r.Track)
                    .OrderBy(r => r.Round).ToList();
                ViewBag.drivers = _context.SeasonDrivers.Where(s => s.SeasonId == currentSeason.SeasonId);

                return View(standings);
            }
            else
            {
                return RedirectToAction("Index", "Season");
            }
            
        }

        [ActionName("PastTeamStandings")]
        public IActionResult TeamStandings(int seasonId)
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

            var standings = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Include(t => t.SeasonDrivers)
                    .ThenInclude(s => s.DriverResults)
                .Where(s => s.SeasonId == currentSeason.SeasonId)
                .OrderByDescending(t => t.Points)
                .ToList();

            ViewBag.rounds = _context.Races
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == currentSeason.SeasonId)
                .Include(r => r.Track)
                .OrderBy(r => r.Round).ToList();
            ViewBag.drivers = _context.SeasonDrivers.Where(s => s.SeasonId == currentSeason.SeasonId);

            return View("TeamStandings", standings);
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
