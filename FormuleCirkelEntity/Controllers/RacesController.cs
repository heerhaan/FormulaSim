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
    public class RacesController : Controller
    {
        private readonly FormulaContext _context;
        private static readonly Random rng = new Random();

        public RacesController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult RacePreview()
        {
            var nextrace = _context.Races.Where(r => r.Season.CurrentSeason == true)
                .Include(r => r.Track).ToList().OrderBy(r => r.Round).FirstOrDefault(r => r.DriverResults == null);
            return View(nextrace);
        }

        [HttpPost]
        public IActionResult RacePreview(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var drivers = _context.SeasonDrivers;
            var driverresults = new List<DriverResult>();

            foreach (var driver in drivers)
            {
                var result = new DriverResult();
                try
                {
                    result.RaceId = id.GetValueOrDefault();
                    result.SeasonDriverId = driver.SeasonDriverId;
                } catch(Exception e)
                {
                    TempData["msg"] = "<script>alert('Race toevoegen mislukt!');</script>";
                    return RedirectToAction(nameof(RacePreview));
                }
                driverresults.Add(result);
            }

            _context.DriverResults.AddRange(driverresults);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(RaceWeekend));
        }

        public IActionResult RaceWeekend()
        {
            return View(_context.SeasonDrivers.Include(s => s.Drivers).Include(t => t.SeasonTeam).ThenInclude(t => t.Team)
                .ToList());
        }

        public IActionResult Q1()
        {
            return View(_context.SeasonDrivers.Include(s => s.Drivers).Include(t => t.SeasonTeam).ThenInclude(t => t.Team)
                .Include(s => s.DriverResults).ToList());
        }

        [HttpPost]
        public IActionResult Q1(SeasonDriver seasonDriver)
        {
            return View(_context.SeasonDrivers.Include(s => s.Drivers).Include(t => t.SeasonTeam).ThenInclude(t => t.Team)
                .ToList());
        }

        public IActionResult Q2()
        {
            return View();
        }

        public IActionResult Q3()
        {
            return View();
        }

        public int QualyCalc(int id)
        {
            int result = 0;
            var driver = _context.SeasonDrivers.Where(s => s.SeasonDriverId == id)
                .Include(s => s.SeasonTeam).FirstOrDefault();
            result += driver.Skill;
            result += driver.SeasonTeam.Chassis;
            result += rng.Next(0, 60);
            return result;
        }

        public IActionResult Race()
        {
            return View();
        }
    }
}