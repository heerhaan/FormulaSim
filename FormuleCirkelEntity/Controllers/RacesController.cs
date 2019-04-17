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
        
        [Route("Season/{id}/[Controller]/{raceId}")]
        public async Task<IActionResult> Race(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season)
                .Include(r => r.Track)
                .Include(r => r.DriverResults)
                    .ThenInclude(res => res.SeasonDriver.Driver)
                .Include(r => r.DriverResults)
                    .ThenInclude(res => res.SeasonDriver.SeasonTeam.Team)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            return View(race);
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/Advance")]
        public async Task<IActionResult> AdvanceStint(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season)
                .Include(r => r.Track)
                .Include(r => r.DriverResults)
                    .ThenInclude(res => res.SeasonDriver.Driver)
                .Include(r => r.DriverResults)
                    .ThenInclude(res => res.SeasonDriver.SeasonTeam.Team)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            if (race.StintProgress == race.Stints.Count())
                return BadRequest();

            race.StintProgress++;
            foreach (var result in race.DriverResults)
            {
                result.StintResults.Add(race.StintProgress, rng.Next(1, 100));
                _context.Update(result);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Route("Season/{id}/[Controller]/{raceId}/Qualifying")]
        public IActionResult Qualifying()
        {
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

        IList<Qualification> GetQualificationsFromDrivers(IList<SeasonDriver> drivers)
        {
            var result = new List<Qualification>();
            foreach (var driver in drivers.ToList())
            {
                result.Add(new Qualification()
                {
                    DriverId = driver.SeasonDriverId,
                    RaceId = 1,
                    TeamName = driver.SeasonTeam.Team.Name,
                    DriverName = driver.Driver.Name,
                    Score = 0
                });
            }
            return result;
        }

        int GetQualifyingDriverLimit(string qualifyingStage)
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

        int GenerateDriverScore(SeasonDriver driver)
        {
            var result = 0;
            result += driver.Skill;
            result += driver.SeasonTeam.Chassis;
            result += rng.Next(0, 60);
            return result;
        }

        [HttpPost]
        public IActionResult Return()
        {
            //Also should save the result of Qualification to the Grid value of DriverResults (after penalties are applied?)
            return RedirectToAction("RaceWeekend", new { id = 1 });
        }
    }
}