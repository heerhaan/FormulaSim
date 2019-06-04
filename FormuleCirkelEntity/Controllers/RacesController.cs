using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Builders;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ResultGenerators;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FormuleCirkelEntity.Controllers
{
    public class RacesController : Controller
    {
        readonly FormulaContext _context;
        readonly RaceResultGenerator _resultGenerator;
        readonly RaceBuilder _raceBuilder;

        public RacesController(FormulaContext context, RaceResultGenerator resultGenerator, RaceBuilder raceBuilder)
        {
            _context = context;
            _resultGenerator = resultGenerator;
            _raceBuilder = raceBuilder;
        }

        [Route("Season/{id}/[Controller]/Add/")]
        public async Task<IActionResult> AddTracks(int? id)
        {
            var season = await _context.Seasons
                   .Include(s => s.Races)
                   .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTrackIds = season.Races.Select(r => r.TrackId);
            var unusedTracks = _context.Tracks.Where(t => !existingTrackIds.Contains(t.TrackId) && t.Archived == false).ToList();

            ViewBag.seasonId = id;
            return View(unusedTracks);
        }

        [HttpPost("Season/{id}/[Controller]/Add/")]
        public async Task<IActionResult> AddTracks(int? id, [Bind("TrackId")] Track track)
        {
            track = await _context.Tracks.SingleOrDefaultAsync(m => m.TrackId == track.TrackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .Include(s => s.Drivers)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (track == null || season == null)
                return NotFound();

            // Finds the last time track was used and uses same stintsetup as then
            var lastracemodel = _context.Races
                .LastOrDefault(lr => lr.Track.TrackId == track.TrackId);

            if(lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints.Values.ToList();
                var race = _raceBuilder
                .InitializeRace(track, season)
                .AddModifiedStints(stintlist)
                .GetResultAndRefresh();

                season.Races.Add(race);
                await _context.SaveChangesAsync();
            }
            else
            {
                var race = _raceBuilder
                .InitializeRace(track, season)
                .AddDefaultStints()
                .GetResultAndRefresh();

                season.Races.Add(race);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(AddTracks), new { id });
        }

        public IActionResult ModifyRace(int id, int trackId)
        {
            var model = new ModifyRaceModel
            {
                SeasonId = id,
                TrackId = trackId
            };

            // Finds the last time track was used and uses same stintsetup as then
            var lastracemodel = _context.Races
                .LastOrDefault(lr => lr.Track.TrackId == trackId);
            if (lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints.Values.ToList();
                model.RaceStints = stintlist;
            }

            var track = _context.Tracks.SingleOrDefault(m => m.TrackId == trackId);
            ViewBag.trackname = track.Name;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyRace(ModifyRaceModel raceModel)
        {
            if (raceModel == null)
                return NotFound();

            var track = _context.Tracks.SingleOrDefault(m => m.TrackId == raceModel.TrackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .Include(s => s.Drivers)
                .SingleOrDefaultAsync(s => s.SeasonId == raceModel.SeasonId);

            IList<Stint> stints = raceModel.RaceStints;

            var race = _raceBuilder
                .InitializeRace(track, season)
                .AddModifiedStints(stints)
                .GetResultAndRefresh();

            season.Races.Add(race);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddTracks", new { id = raceModel.SeasonId });
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
        
        [Route("Season/{id}/[Controller]/{raceId}/Preview")]
        public async Task<IActionResult> RacePreview(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            var teamSpecs = _context.SeasonTeams
                .Where(ts => ts.Season.SeasonId == id)
                .Where(ts => ts.Specification == race.Track.Specification)
                .Include(t => t.Team)
                .Distinct()
                .ToList();

            ViewBag.teamSpecs = teamSpecs;

            return View(race);
        }
        
        [HttpPost("Season/{id}/[Controller]/{raceId}/Start")]
        public async Task<IActionResult> RaceStart(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season.Drivers)
                .Include(r => r.DriverResults)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            if (!race.DriverResults.Any())
            {
                race = _raceBuilder
                    .Use(race)
                    .AddAllDrivers()
                    .GetResult();

                _context.DriverResults.AddRange(race.DriverResults);
                _context.SaveChanges();
            }
            
            return RedirectToAction("RaceWeekend", new { id, raceId });
        }


        [Route("Season/{id}/[Controller]/{raceId}/Weekend")]
        public async Task<IActionResult> RaceWeekend(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.Driver)
                .Include(r => r.Season.Teams)
                    .ThenInclude(d => d.Team)
                .Include(r => r.Track)
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
                .Include(r => r.DriverResults)
                    .ThenInclude(res => res.SeasonDriver.SeasonTeam.Engine)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            if (race.StintProgress == race.Stints.Count())
                return BadRequest();

            race.StintProgress++;
            var stint = race.Stints[race.StintProgress];

            // Calculate results for all drivers who have not been DSQ'd or DNF'd.
            foreach (var result in race.DriverResults.Where(d => d.Status == Status.Finished))
            {
                var stintResult = _resultGenerator.GetStintResult(result, stint);
                result.StintResults.Add(race.StintProgress, stintResult);
                result.Points = result.StintResults.Sum(sr => sr.Value ?? -999);

                // A null result indicates a DNF result.
                if (stintResult == null)
                    result.Status = Status.DNF;
            }

            var positionsList = _resultGenerator.GetPositionsBasedOnRelativePoints(race.DriverResults);

            foreach (var result in race.DriverResults)
                result.Position = positionsList[result.DriverResultId];

            _context.UpdateRange(race.DriverResults);
            await _context.SaveChangesAsync();

            // Clean up unneeded large reference properties to prevent them from being serialized and sent over HTTP.
            race.Season = null;
            race.Track = null;
            race.Stints = null;
            foreach(var dr in race.DriverResults)
            {
                dr.SeasonDriver = null;
                dr.Race = null;
            }
            return new JsonResult(race, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        public async Task<IActionResult> FinishRace(int seasonId, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.SeasonTeam)
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.Driver)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == seasonId);

            foreach (var result in race.DriverResults.Where(res => res.Status == Status.Finished))
            {
                int points = PointsEarned(result.Position);
                result.SeasonDriver.Points += points;
                result.SeasonDriver.SeasonTeam.Points += points;
                _context.UpdateRange(result.SeasonDriver, result.SeasonDriver.SeasonTeam);
            }

            var raceWinnerResult = race.DriverResults.OrderByDescending(res => res.Points).FirstOrDefault();
            race.Track.MostRecentWinner = raceWinnerResult.SeasonDriver.Driver;
            _context.Update(race.Track);
            _context.SaveChanges();

            return RedirectToAction("DriverStandings", "Home");
        }

        int PointsEarned(int pos)
        {
            int points = 0;

            switch (pos)
            {
                case 1:
                    points = 25;
                    break;
                case 2:
                    points = 18;
                    break;
                case 3:
                    points = 15;
                    break;
                case 4:
                    points = 12;
                    break;
                case 5:
                    points = 10;
                    break;
                case 6:
                    points = 8;
                    break;
                case 7:
                    points = 6;
                    break;
                case 8:
                    points = 5;
                    break;
                case 9:
                    points = 4;
                    break;
                case 10:
                    points = 3;
                    break;
                case 11:
                    points = 2;
                    break;
                case 12:
                    points = 1;
                    break;
            }

            return points;
        }

        [Route("Season/{id}/[Controller]/{raceId}/Qualifying")]
        public IActionResult Qualifying(int id, int raceId)
        {
            var race = _context.Races.Single(r => r.RaceId == raceId);
            var season = _context.Seasons.Single(s => s.SeasonId == id);
            ViewBag.race = race;
            ViewBag.season = season;
            return View();
        }

        [Route("Season/{id}/[Controller]/{raceId}/Qualifying/Update")]
        public async Task<IActionResult> UpdateQualifying(int id, int raceId, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return BadRequest();

            try
            {
                var race = await _context.Races
                    .Include(r => r.Season)
                    .Include(r => r.DriverResults)
                        .ThenInclude(dr => dr.SeasonDriver)
                            .ThenInclude(sd => sd.SeasonTeam)
                                .ThenInclude(st => st.Team)
                    .Include(r => r.DriverResults)
                        .ThenInclude(dr => dr.SeasonDriver)
                            .ThenInclude(sd => sd.Driver)
                    .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == id);
                var drivers = race.DriverResults
                    .Select(dr => dr.SeasonDriver)
                    .ToList();

                // Get the existing qualification results of the current race.
                var currentQualifyingResult = _context.Qualification.Where(q => q.RaceId == raceId).ToList();

                // If there are no qualifying results yet, initialize them.
                if (!currentQualifyingResult.Any())
                {
                    currentQualifyingResult.AddRange(GetQualificationsFromDrivers(drivers, raceId));
                }

                var driverLimit = GetQualifyingDriverLimit(source, race.Season);

                // Take the current result, then order descending to place highest score first, lowest score last. 
                // From the resulting ordered list, take the amount of drivers allowed to continue to the next qualifying round.
                var qualificationResultsToUpdate = currentQualifyingResult
                    .OrderBy(q => q.Position)
                    .Take(driverLimit);

                // Apply qualifying RNG on the drivers in the round.
                foreach (var qualificationResult in qualificationResultsToUpdate)
                {
                    var qualifyingDriver = drivers.Single(d => d.SeasonDriverId == qualificationResult.DriverId);
                    qualificationResult.Score = _resultGenerator.GetQualifyingResult(qualifyingDriver);
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

        IList<Qualification> GetQualificationsFromDrivers(IList<SeasonDriver> drivers, int raceId)
        {
            var result = new List<Qualification>();
            foreach (var driver in drivers.ToList())
            {
                result.Add(new Qualification()
                    {
                        DriverId = driver.SeasonDriverId,
                        RaceId = raceId,
                        TeamName = driver.SeasonTeam.Team.Abbreviation,
                        Colour = driver.SeasonTeam.Team.Colour,
                        Accent = driver.SeasonTeam.Team.Accent,
                        DriverName = driver.Driver.Name,
                        Score = 0
                    });
            }
            return result;
        }

        int GetQualifyingDriverLimit(string qualifyingStage, Season season)
        {
            if (qualifyingStage == "Q2")
                return season.QualificationRemainingDriversQ2;
            if (qualifyingStage == "Q3")
                return season.QualificationRemainingDriversQ3;
            return season.Drivers.Count;
        }

        [HttpPost]
        public IActionResult Return(int? id, int? raceId)
        {
            if(id == null || raceId == null)
            {
                return BadRequest();
            }

            IQueryable<Qualification> qualyresult = _context.Qualification.Where(q => q.RaceId == raceId);
            List<DriverResult> driverResults = _context.DriverResults.Where(d => d.RaceId == raceId).ToList();

            //Adds results from Qualification to Grid in DriverResults (Penalties may be applied here too)
            foreach(Qualification result in qualyresult)
            {
                DriverResult driver = driverResults.Single(d => d.RaceId == result.RaceId &&
                    d.SeasonDriverId == result.DriverId);

                if(driver == null)
                {
                    return StatusCode(500);
                }

                driver.Grid = result.Position.Value;
                driver.Position = result.Position.Value;
            }
            _context.UpdateRange(driverResults);
            _context.SaveChanges();

            return RedirectToAction("RaceWeekend", new { id, raceId });
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/round")]
        public async Task<IActionResult> MoveRound(int id, int raceId, [FromQuery] int direction)
        {
            if (direction != -1 && direction != 1)
                return BadRequest("Direction may only be 1 or -1.");

            var race = await _context.Races
                .Include(r => r.Season)
                    .ThenInclude(s => s.Races)
                .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == id);

            if (race == null)
                return NotFound();

            if (race.Season.State >= SeasonState.Progress)
                return BadRequest(new ErrorResult("SeasonInProgress", "Cannot move rounds while a season is in progress."));

            // Get the race to switch with by getting the race with the position above or below it, matching with the Direction parameter.
            var raceToSwitch = race.Season.Races.SingleOrDefault(r => r.Round == race.Round + direction);

            if (raceToSwitch == null)
                return BadRequest(new ErrorResult("InvalidRoundMove", $"Cannot move the {(direction == -1 ? "first" : "last")} round further than its current position."));

            (race.Round, raceToSwitch.Round) = (raceToSwitch.Round, race.Round);
            _context.UpdateRange(race, raceToSwitch);
            await _context.SaveChangesAsync();

            // Prepare race object for serialization
            race.DriverResults = null;
            race.Season = null;
            race.Track = null;
            race.Stints = null;
            raceToSwitch.DriverResults = null;
            raceToSwitch.Season = null;
            raceToSwitch.Track = null;
            raceToSwitch.Stints = null;
            return new JsonResult(new[] { race, raceToSwitch });
        }
    }
}