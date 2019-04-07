using System;
using System.Collections.Generic;
using System.Drawing;
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

        public IActionResult Index()
        {
            return View(_context.Seasons.ToList());
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await _context.Seasons
                .Include(s => s.Races)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            return View("Detail", season);
        }
        
        public async Task<IActionResult> AddTracks(int? id)
        {
            var season = await _context.Seasons
                   .Include(s => s.Races)
                   .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTrackIds = season.Races.Select(r => r.TrackId);
            var unusedTracks = _context.Tracks.Where(t => !existingTrackIds.Contains(t.TrackId)).ToList();

            ViewBag.seasonId = id;
            return View(unusedTracks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTracks(int? id, [Bind("TrackId")] Track track)
        {
            track = await _context.Tracks.SingleOrDefaultAsync(m => m.TrackId == track.TrackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (track == null || season == null)
                return NotFound();

            var race = new Race();
            race.Track = track;
            race.Name = track.Name;
            season.Races.Add(race);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddTracks), new { id });
        }

        //Methods for adding engines to season
        public IActionResult AddEngines()
        {
            var engines = _context.Engines.ToList();
            //Known problem: new season means that it will still remove past seasonengines from view
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
            var seasonteams = _context.SeasonTeams.Where(t => t.Season.CurrentSeason == true).ToList();
            var unusedteams = _context.Teams.ToList();

            ViewBag.season = seasonteams.Count();

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
            var summary = _context.SeasonDrivers.Include(s => s.Driver).Include(s => s.SeasonTeam.Team).Include(s => s.SeasonTeam.SeasonEngine.Engine).ToList();
            return View(summary.OrderByDescending(s => s.SeasonTeam.Team.TeamId));
        }
    }
}