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
            
            return RedirectToAction("RaceWeekend", new { id });
        }

        public IActionResult RaceWeekend(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var race = _context.Races.FirstOrDefault(r => r.RaceId == id);
            ViewBag.track = race.Track;
            ViewBag.race = race;
            ViewBag.id = id;

            return View(_context.SeasonDrivers.Include(s => s.Driver).Include(t => t.SeasonTeam).ThenInclude(t => t.Team)
                .ToList());
        }

        public IActionResult Q1()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRacingDrivers(string source)
        {
            if(source == null)
            {
                return null;
            }

            int result = 0;
            int position = 1;
            var qualy = new List<Qualification>();

            var drivers = _context.SeasonDrivers.Include(s => s.Driver).Include(t => t.SeasonTeam).ThenInclude(t => t.Team)
                .Include(d => d.SeasonDriverId).Select(d => new
                {
                    id = d.SeasonDriverId,
                    team = d.SeasonTeam.Team.Name,
                    name = d.Driver.Name,
                    skill = d.Skill,
                    chassis = d.SeasonTeam.Chassis
                }).ToList();

            foreach(var driver in drivers.ToList())
            {
                switch (source)
                {
                    case "Q1":
                        break;
                    case "Q2":
                        foreach (var item in _context.Qualification.Where(q => q.RaceId == 1))
                        {
                            if (driver.id == item.DriverId)
                            {
                                if (item.Position > 3)
                                {
                                    drivers.Remove(driver);
                                }
                            }
                        }
                        break;
                    case "Q3":
                        foreach (var item in _context.Qualification.Where(q => q.RaceId == 1))
                        {
                            if (driver.id == item.DriverId)
                            {
                                if (item.Position > 2)
                                {
                                    drivers.Remove(driver);
                                }
                            }
                        }
                        break;

                }
            }
            foreach (var driver in drivers.ToList())
            {
                result = driver.skill;
                result += driver.chassis;
                result += rng.Next(0, 60);
                qualy.Add(new Qualification()
                {
                    DriverId = driver.id,
                    RaceId = 1,
                    TeamName = driver.team,
                    DriverName = driver.name,
                    Score = result
                });
            }
            qualy = qualy.OrderByDescending(q => q.Score).ToList();
            foreach (var driver in qualy)
            {
                driver.Position = position;
                position++;
            }
            
            UpdateQualy(source, qualy);

            return Json(qualy);
        }

        private void UpdateQualy(string source, List<Qualification> qualy)
        {
            if (source == "Q1")
            {
                foreach (var driver in qualy)
                {
                    _context.Qualification.Add(driver);
                }
            }
            else if (source == "Q2" || source == "Q3")
            {
                foreach (var driver in qualy)
                {
                    try
                    {
                        //var related = _context.Qualification.FirstOrDefault(q => q.RaceId == 1 && q.DriverId == driver.DriverId);
                        //driver.QualyId = related.QualyId;
                        _context.Qualification.Update(driver);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            _context.SaveChanges();
        }
        
        public IActionResult Race()
        {
            return View();
        }
    }
}