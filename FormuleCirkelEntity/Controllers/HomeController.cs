using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.DAL;
using Microsoft.EntityFrameworkCore;

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
            return View();
        }

        public IActionResult DriverStandings()
        {
            ViewBag.rounds = _context.Races.Include(r => r.Track).ToList();

            var currentSeason = _context.Seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

            var standings = _context.SeasonDrivers
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam.Team)
                .Where(s => s.SeasonId == currentSeason.SeasonId)
                .OrderByDescending(s => s.Points)
                .ToList();
            return View(standings);
        }

        public IActionResult TeamStandings()
        {
            ViewBag.rounds = _context.Races.Include(r => r.Track).ToList();
            ViewBag.drivers = _context.SeasonDrivers;

            var currentSeason = _context.Seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

            var standings = _context.SeasonTeams
                .Include(t => t.Team)
                .Include(t => t.SeasonDrivers)
                    .ThenInclude(s => s.DriverResults)
                .Where(s => s.SeasonId == currentSeason.SeasonId)
                .OrderByDescending(t => t.Points)
                .ToList();

            return View(standings);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
