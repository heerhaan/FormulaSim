using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            }
            return View(race);
        }

        public IActionResult AddEngines()
        {
            var engines = _context.Engines.ToList();
            var seasonengines = _context.SeasonEngine.ToList();
            var unusedengines = _context.Engines.ToList();

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
                _context.SeasonEngine.Add(seasonEngine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddEngines));
            }
            return View(seasonEngine);
        }

        public IActionResult AddTeams()
        {
            return View(_context.Teams.ToList());
        }

        public async Task<IActionResult> TeamToSeason()
        {
            return View();
        }
    }
}