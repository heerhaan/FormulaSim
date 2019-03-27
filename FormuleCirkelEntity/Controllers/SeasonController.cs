using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : Controller
    {
        private readonly FormulaContext _context;

        public SeasonController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Main()
        {
            return View(_context.Seasons.ToList());
        }
        
        [HttpPost]
        public async Task<IActionResult> Main([Bind("SeasonId")]Season season)
        {
            season.CurrentSeason = true;
            _context.Seasons.Add(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddTracks));
        }

        //Methods to adding circuits to season
        public IActionResult AddTracks()
        {
            var season = _context.Seasons.FirstOrDefault(s => s.CurrentSeason == true);
            var race = _context.Races.LastOrDefault(r => r.SeasonId == season.SeasonId);
            ViewBag.rounds = race.Round;

            var tracks = _context.Tracks.ToList();
            var races = _context.Races.ToList();
            var unusedtracks = _context.Tracks.ToList();

            foreach(var item in tracks)
            {
                foreach(var rees in races)
                {
                    if(item.TrackId == rees.TrackId)
                    {
                        unusedtracks.Remove(item);
                    }
                }
            }
            return View(unusedtracks);
        }
        
        public IActionResult AddRace(int? trackid)
        {
            if (trackid == null)
            {
                return NotFound();
            }
            var track = _context.Tracks
                .FirstOrDefault(m => m.TrackId == trackid);

            var season = _context.Seasons.FirstOrDefault(s => s.CurrentSeason == true);

            var race = _context.Races.LastOrDefault(r => r.SeasonId == season.SeasonId);
            if (track == null || season == null)
            {
                return NotFound();
            }

            ViewBag.TrackId = track.TrackId;
            ViewBag.SeasonId = season.SeasonId;

            if (race == null)
            {
                ViewBag.Round = 1;
            } else
            {
                ViewBag.Round = (race.Round + 1);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRace([Bind("RaceId,Round,Name,TrackId,SeasonId")]Race race)
        {
            if (ModelState.IsValid)
            {
                _context.Races.Add(race);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddTracks));
            } else
            {
                TempData["msg"] = "<script>alert('Race toevoegen mislukt!');</script>";
                return RedirectToAction(nameof(AddTracks));
            }
        }

        //Methods for adding engines to season
        public IActionResult AddEngines()
        {
            var engines = _context.Engines.ToList();
            var seasonengines = _context.SeasonEngines.ToList();
            var unusedengines = _context.Engines.ToList();

            ViewBag.season = _context.SeasonEngines.Count();

            foreach(var engine in engines)
            {
                foreach(var item in seasonengines)
                {
                    if(engine.EngineId == item.EngineId)
                    {
                        unusedengines.Remove(engine);
                    }
                }
            }
            return View(unusedengines);
        }

        public IActionResult EngineToSeason(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            ViewBag.id = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EngineToSeason(SeasonEngine seasonEngine)
        {
            if (ModelState.IsValid)
            {
                _context.SeasonEngines.Add(seasonEngine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddEngines));
            } else
            {
                TempData["msg"] = "<script>alert('Motor toevoegen mislukt!');</script>";
                return RedirectToAction(nameof(AddEngines));
            }
        }

        //Methods for adding teams to season
        public IActionResult AddTeams()
        {
            var teams = _context.Teams.ToList();
            var seasonteams = _context.SeasonTeams.ToList();
            var unusedteams = _context.Teams.ToList();

            ViewBag.season = _context.SeasonTeams.Count();

            foreach(var team in teams)
            {
                foreach(var item in seasonteams)
                {
                    if(team.TeamId == item.TeamId)
                    {
                        unusedteams.Remove(team);
                    }
                }
            }
            return View(unusedteams);
        }
        
        public IActionResult TeamToSeason(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var current = _context.Seasons.FirstOrDefault(s => s.CurrentSeason == true);

            ViewBag.id = id;
            ViewBag.current = current.SeasonId;

            var engines =
                (from e in _context.Engines
                 join s in _context.SeasonEngines on e.EngineId equals s.EngineId
                 select new { s.SeasonEngineId, e.Name });

            ViewBag.engines = new SelectList(engines, "SeasonEngineId", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TeamToSeason(SeasonTeam seasonTeam)
        {
            if (ModelState.IsValid)
            {
                _context.SeasonTeams.Add(seasonTeam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddTeams));
            } else
            {
                TempData["msg"] = "<script>alert('Team toevoegen mislukt!');</script>";
                return RedirectToAction(nameof(AddTeams));
            }
        }

        //Methods for adding drivers to season
        public IActionResult AddDrivers()
        {
            var drivers = _context.Drivers.ToList();
            var seasondrivers = _context.SeasonDrivers.ToList();
            var unentereddrivers = _context.Drivers.ToList();

            ViewBag.season = _context.SeasonDrivers.Count();

            foreach (var driver in drivers)
            {
                foreach (var item in seasondrivers)
                {
                    if (driver.DriverId == item.DriverId)
                    {
                        unentereddrivers.Remove(driver);
                    }
                }
            }
            return View(unentereddrivers);
        }

        public IActionResult DriverToSeason(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var current = _context.Seasons.FirstOrDefault(s => s.CurrentSeason == true);
            ViewBag.id = id;
            ViewBag.current = current.SeasonId;

            var teams = (
                from t in _context.Teams
                join s in _context.SeasonTeams on t.TeamId equals s.TeamId
                select new { s.SeasonTeamId, t.Name });
            ViewBag.teams = new SelectList(teams, "SeasonTeamId", "Name");

            return View();
        }

        [HttpPost]
        public IActionResult DriverToSeason(SeasonDriver seasonDriver)
        {
            if (ModelState.IsValid)
            {
                _context.SeasonDrivers.Add(seasonDriver);
                _context.SaveChanges();
                return RedirectToAction(nameof(AddDrivers));
            } else
            {
                TempData["msg"] = "<script>alert('Coureur toevoegen mislukt!');</script>";
                return RedirectToAction(nameof(AddDrivers));
            }
        }

        public IActionResult AddSummary()
        {
            var summary = _context.SeasonDrivers.Include(s => s.Drivers).Include(s => s.SeasonTeam.Team).Include(s => s.SeasonTeam.SeasonEngine.Engine).ToList();
            return View(summary.OrderByDescending(s => s.SeasonTeam.Team.TeamId));
        }
    }
}