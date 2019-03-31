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

            return View(_context.SeasonDrivers.Where(s => s.SeasonTeam.Season.CurrentSeason == true)
                .Include(s => s.DriverResults).Include(s => s.Drivers).Include(s => s.SeasonTeam.Team)
                .ToList().OrderByDescending(s => s.Points));
        }

        public IActionResult TeamStandings()
        {
            ViewBag.rounds = _context.Races.Include(r => r.Track).ToList();
            ViewBag.drivers = _context.SeasonDrivers;

            return View(_context.SeasonTeams.Where(s => s.Season.CurrentSeason == true)
                .Include(t => t.Team).Include(t => t.SeasonDrivers).ThenInclude(s => s.DriverResults)
                .ToList().OrderByDescending(t => t.Points));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
