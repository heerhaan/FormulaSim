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
            return View(_context.Tracks.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> AddTracks(int? trackid)
        {
            if (trackid == null)
            {
                return NotFound();
            }
            var track = await _context.Tracks.FindAsync(trackid);
            if (track == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(AddRace), new { trackid });
        }

        public async Task<IActionResult> AddRace(int? trackid)
        {
            if (trackid == null)
            {
                return NotFound();
            }
            var track = await _context.Tracks
                .FirstOrDefaultAsync(m => m.TrackId == trackid);

            var season = await _context.Seasons.FirstOrDefaultAsync(s => s.CurrentSeason == true);

            var race = await _context.Races.LastOrDefaultAsync(r => r.SeasonId == season.SeasonId);
            if (track == null || season == null)
            {
                return NotFound();
            }

            if(race == null)
            {
                ViewBag.Round = 1;
            } else
            {
                ViewBag.Round = (race.Round + 1);
            }
            return View(track);
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
    }
}