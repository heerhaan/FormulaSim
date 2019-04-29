using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RacePreview()
        {
            var currentSeason = _context.Seasons
                .Where(s => s.SeasonStart != null && s.State == SeasonState.Progress)
                .OrderBy(s => s.SeasonStart)
                .FirstOrDefault();

            var nextrace = _context.Races
                .Include(r => r.Track)
                .Where(r => r.SeasonId == currentSeason.SeasonId)
                .OrderBy(r => r.Round)
                .FirstOrDefault(r => r.DriverResults != null);

            return View(nextrace);
        }

        [HttpPost]
        public IActionResult RacePreview(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var drivers = _context.SeasonDrivers;
            var driverresults = new List<DriverResult>();

            if (_context.DriverResults.Any(d => d.RaceId != id))
            {
                foreach (var driver in drivers)
                {
                    var result = new DriverResult();
                    try
                    {
                        result.RaceId = id.GetValueOrDefault();
                        result.SeasonDriverId = driver.SeasonDriverId;
                    }
                    catch (Exception e)
                    {
                        TempData["msg"] = "<script>alert('Race preview mislukt!');</script>";
                        return RedirectToAction(nameof(RacePreview));
                    }
                    driverresults.Add(result);
                }
            }
            else
            {
                return RedirectToAction("RaceWeekend", new { id });
            }

            _context.DriverResults.AddRange(driverresults);
            _context.SaveChanges();

            return RedirectToAction("RaceWeekend", new { id });
        }

        public IActionResult RaceWeekend(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var race = _context.Races
                .Where(s => s.SeasonId == id)
                .FirstOrDefault(r => r.RaceId == id);

            ViewBag.track = race.Track;
            ViewBag.race = race;

            var result = _context.DriverResults.FirstOrDefault(r => r.RaceId == id);

            if (result.Grid == 0)
            {
                ViewBag.check = true;
            }
            else
            {
                ViewBag.check = false;
            }

            //SeasonDrivers should be ordered based on DriverResults Gridposition
            var drivers = _context.SeasonDrivers
                .Where(s => s.Season.SeasonId == id)
                .Include(s => s.Driver)
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .ToList();

            return View(drivers);
        }

        public IActionResult Qualifying(int? id)
        {
            var race = _context.Races.Single(r => r.RaceId == id);
            ViewBag.race = race;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRacingDrivers(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return BadRequest();

            try
            {
                // Get all drivers of the season.
                var drivers = _context.SeasonDrivers
                .Include(s => s.Driver)
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .ToList();

                // Get the existing qualification results of the current race.
                var currentQualifyingResult = _context.Qualification.Where(q => q.RaceId == 1).ToList();

                // If there are no qualifying results yet, initialize them.
                if (!currentQualifyingResult.Any())
                {
                    currentQualifyingResult.AddRange(GetQualificationsFromDrivers(drivers));
                }

                var driverLimit = GetQualifyingDriverLimit(source);

                // Take the current result, then order descending to place highest score first, lowest score last.
                // From the resulting ordered list, take the amount of drivers allowed to continue to the next qualifying round.
                var qualificationResultsToUpdate = currentQualifyingResult
                    .OrderBy(q => q.Position)
                    .Take(driverLimit);

                // Apply qualifying RNG on the drivers in the round.
                foreach (var qualificationResult in qualificationResultsToUpdate)
                {
                    var qualifyingDriver = drivers.Single(d => d.SeasonDriverId == qualificationResult.DriverId);
                    qualificationResult.Score = GenerateDriverScore(qualifyingDriver);
                }

                var qualificationResults = qualificationResultsToUpdate.OrderByDescending(q => q.Score).ToList();

                // Using a for loop, use the loop index to set the position.
                for (int i = 0; i < qualificationResultsToUpdate.Count(); i++)
                {
                    var resultToUpdate = qualificationResults[i];
                    resultToUpdate.Position = i + 1;
                }

                // Update everything and save.
                _context.UpdateRange(qualificationResultsToUpdate);
                await _context.SaveChangesAsync();
                return new JsonResult(qualificationResultsToUpdate);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        private IList<Qualification> GetQualificationsFromDrivers(IList<SeasonDriver> drivers)
        {
            var result = new List<Qualification>();
            foreach (var driver in drivers.ToList())
            {
                //TODO: dynamically add the next Race
                result.Add(new Qualification()
                {
                    DriverId = driver.SeasonDriverId,
                    RaceId = 1,
                    TeamName = driver.SeasonTeam.Team.Name,
                    Colour = driver.SeasonTeam.Team.Colour,
                    Accent = driver.SeasonTeam.Team.Accent,
                    DriverName = driver.Driver.Name,
                    Score = 0
                });
            }
            return result;
        }

        private int GetQualifyingDriverLimit(string qualifyingStage)
        {
            //Limits should be flexible in accordance to entered drivers.
            const int Q1_LIMIT = 10;
            const int Q2_LIMIT = 6;
            const int Q3_LIMIT = 4;

            if (qualifyingStage == "Q2")
                return Q2_LIMIT;
            if (qualifyingStage == "Q3")
                return Q3_LIMIT;
            return Q1_LIMIT;
        }

        private int GenerateDriverScore(SeasonDriver driver)
        {
            var result = 0;
            result += driver.Skill;
            result += driver.SeasonTeam.Chassis;
            result += rng.Next(0, 60);
            return result;
        }

        [HttpPost]
        public IActionResult Return(int? id)
        {
            if (id == null) { return BadRequest(); }

            var qualyresult = _context.Qualification.Where(q => q.RaceId == id);
            var driverResults = _context.DriverResults.Where(d => d.RaceId == id).ToList();

            //Adds results from Qualification to Grid in DriverResults (Penalties may be applied here too)
            foreach (var result in qualyresult)
            {
                var driver = driverResults.Single(d => d.RaceId == result.RaceId && d.SeasonDriverId == result.DriverId);

                if (driver == null) { return StatusCode(500); }

                driver.Grid = result.Position.Value;
            }
            _context.UpdateRange(driverResults);
            _context.SaveChanges();

            return RedirectToAction("RaceWeekend", new { id });
        }

        public IActionResult Race()
        {
            return View();
        }
    }
}