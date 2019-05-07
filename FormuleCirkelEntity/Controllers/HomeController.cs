using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

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

            ViewBag.rounds = _context.Races.Where(r => r.SeasonId == currentSeason.SeasonId).Include(r => r.Track).ToList();

            return View(standings);
        }

        public IActionResult TeamStandings()
        {
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

            ViewBag.rounds = _context.Races.Where(r => r.SeasonId == currentSeason.SeasonId).Include(r => r.Track).ToList();
            ViewBag.drivers = _context.SeasonDrivers.Where(s => s.SeasonId == currentSeason.SeasonId);

            return View(standings);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
