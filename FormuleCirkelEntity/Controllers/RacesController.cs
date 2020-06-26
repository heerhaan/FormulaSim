using System;
using System.Collections;
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
        public static readonly Random rng = new Random();

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
                .AsNoTracking()
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTrackIds = season.Races.Select(r => r.TrackId);
            var unusedTracks = _context.Tracks.Where(t => !existingTrackIds.Contains(t.Id)).OrderBy(t => t.Location).OrderBy(t => t.Location).ToList();

            ViewBag.seasonId = id;
            return View(unusedTracks);
        }

        [HttpPost("Season/{id}/[Controller]/Add/")]
        public async Task<IActionResult> AddTracks(int? id, [Bind("TrackId")] int trackId)
        {
            var track = await _context.Tracks.SingleOrDefaultAsync(m => m.Id == trackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .Include(s => s.Drivers)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (track == null || season == null)
                return NotFound();

            // Finds the last time track was used and uses same stintsetup as then
            var lastracemodel = _context.Races
                .Where(r => r.Season.ChampionshipId == season.ChampionshipId)
                .LastOrDefault(lr => lr.Track.Id == track.Id);

            if(lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints.Values.ToList();
                var race = _raceBuilder
                .InitializeRace(track, season)
                .AddModifiedStints(stintlist)
                .GetResultAndRefresh();

                race.Weather = RandomWeather();
                season.Races.Add(race);
                await _context.SaveChangesAsync();
            }
            else
            {
                var race = _raceBuilder
                .InitializeRace(track, season)
                .AddDefaultStints()
                .GetResultAndRefresh();

                race.Weather = RandomWeather();
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

            var season = _context.Seasons.SingleOrDefault(s => s.SeasonId == id);

            // Finds the last time track was used and uses same stintsetup as then
            var lastracemodel = _context.Races
                .Where(r => r.Season.ChampionshipId == season.ChampionshipId)
                .LastOrDefault(lr => lr.Track.Id == trackId);

            if (lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints.Values.ToList();
                model.RaceStints = stintlist;
            }

            var track = _context.Tracks.SingleOrDefault(m => m.Id == trackId);
            ViewBag.trackname = track.Name;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyRace(ModifyRaceModel raceModel)
        {
            if (raceModel == null)
                return NotFound();

            var track = _context.Tracks.SingleOrDefault(m => m.Id == raceModel.TrackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .Include(s => s.Drivers)
                .SingleOrDefaultAsync(s => s.SeasonId == raceModel.SeasonId);

            IList<Stint> stints = raceModel.RaceStints;

            var race = _raceBuilder
                .InitializeRace(track, season)
                .AddModifiedStints(stints)
                .GetResultAndRefresh();

            race.Weather = RandomWeather();
            season.Races.Add(race);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddTracks", new { id = raceModel.SeasonId });
        }

        public Weather RandomWeather()
        {
            int random = rng.Next(1, 21);
            Weather weather = Weather.Sunny;

            switch (random)
            {
                case int n when n <= 8:
                    weather = Weather.Sunny;
                    break;
                case int n when n > 8 && n <= 16:
                    weather = Weather.Overcast;
                    break;
                case int n when n > 16 && n <= 19:
                    weather = Weather.Rain;
                    break;
                case 20:
                    weather = Weather.Storm;
                    break;
            }

            return weather;
        }

        [Route("Season/{id}/[Controller]/{raceId}")]
        public async Task<IActionResult> Race(int id, int raceId)
        {
            var race = await _context.Races
                .IgnoreQueryFilters()
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
                .IgnoreQueryFilters()
                .Include(r => r.Season)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            ViewBag.favourites = Favourites(race);

            return View(race);
        }

        // Gets the three favourites for that race.
        private List<SeasonTeam> Favourites(Race race)
        {
            var teams = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Where(st => st.SeasonId == race.SeasonId)
                .Include(st => st.Team)
                .OrderByDescending(st => (st.Chassis + st.Engine.Power + GetChassisBonus(st, race.Track)))
                .Take(3)
                .ToList();

            return teams;
        }

        // To determine the bonus to a chassis a team gets to a specific track
        private int GetChassisBonus(SeasonTeam team, Track track)
        {
            int bonus = 0;
            Dictionary<string, int> specs = new Dictionary<string, int>
            {
                { "Topspeed", team.Topspeed },
                { "Acceleration", team.Acceleration },
                { "Stability", team.Stability },
                { "Handling", team.Handling }
            };

            var spec = (specs.SingleOrDefault(k => k.Key == track.Specification.ToString()));
            bonus = spec.Value;

            return bonus;
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/Start")]
        public async Task<IActionResult> RaceStart(int id, int raceId)
        {
            var race = await _context.Races
                .IgnoreQueryFilters()
                .Include(r => r.Season.Drivers)
                .Include(r => r.Season.Teams)
                .Include(r => r.DriverResults)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            if (!race.DriverResults.Any())
            {
                race = _raceBuilder
                    .Use(race)
                    .AddAllDrivers(race.Track)
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
                .IgnoreQueryFilters()
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.Driver)
                .Include(r => r.Season.Teams)
                    .ThenInclude(d => d.Team)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            ViewBag.year = race.Season.SeasonNumber;

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

            if (race.StintProgress == race.Stints.Count)
                return BadRequest();

            race.StintProgress++;
            var stint = race.Stints[race.StintProgress];
            var track = race.Track;

            // Calculate results for all drivers who have not been DSQ'd or DNF'd.
            foreach (var result in race.DriverResults.Where(d => d.Status == Status.Finished))
            {
                var stintResult = _resultGenerator.GetStintResult(result, stint, track, race);
                
                // A null or -1000 result indicates a non-finish.
                if (stintResult == null || stintResult.Value == -1000)
                {
                    // RNG to determine the type of DNF.
                    int dnfvalue = rng.Next(1, 26);
                    if (dnfvalue == 25)
                    {
                        result.Status = Status.DSQ;
                        result.DSQCause = RandomDSQCause(stintResult.HasValue);
                    } else
                    {
                        result.Status = Status.DNF;
                        result.DNFCause = RandomDNFCause(stintResult.HasValue);
                    }
                    if (stintResult.HasValue)
                        stintResult = null;
                }

                result.StintResults.Add(race.StintProgress, stintResult);
                result.Points = result.StintResults.Sum(sr => sr.Value ?? -999);
                if (result.DSQCause != DSQCause.None)
                    result.Points += -999;
            }

            var positionsList = _resultGenerator.GetPositionsBasedOnRelativePoints(race.DriverResults);

            foreach (var result in race.DriverResults)
            {
                result.Position = positionsList.OrderedResults[result.DriverResultId];
                result.Points = positionsList.DriverResults.SingleOrDefault(dr => dr.SeasonDriverId == result.SeasonDriverId).Points;
            }

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

        DNFCause RandomDNFCause(bool driverDNF)
        {
            int random = rng.Next(1, 101);
            DNFCause cause = DNFCause.None;

            if (driverDNF)
            {
                switch (random)
                {
                    case int n when n <= 16:
                        cause = DNFCause.Damage;
                        break;
                    case int n when n > 16 && n <= 44:
                        cause = DNFCause.Collision;
                        break;
                    case int n when n > 44 && n <= 92:
                        cause = DNFCause.Accident;
                        break;
                    case int n when n > 92:
                        cause = DNFCause.Puncture;
                        break;
                }
            }
            else
            {
                switch (random)
                {
                    case int n when n <= 48:
                        cause = DNFCause.Engine;
                        break;
                    case int n when n > 48 && n <= 78:
                        cause = DNFCause.Electrics;
                        break;
                    case int n when n > 78 && n <= 84:
                        cause = DNFCause.Exhaust;
                        break;
                    case int n when n > 84 && n <= 86:
                        cause = DNFCause.Clutch;
                        break;
                    case int n when n > 86 && n <= 96:
                        cause = DNFCause.Hydraulics;
                        break;
                    case int n when n > 96 && n <= 98:
                        cause = DNFCause.Wheel;
                        break;
                    case int n when n > 98:
                        cause = DNFCause.Brakes;
                        break;
                }
            }

            return cause;
        }

        DSQCause RandomDSQCause(bool driverDNF)
        {
            
            DSQCause cause = DSQCause.None;

            if (driverDNF)
            {
                int random = rng.Next(1, 11);
                if (random < 9)
                    cause = DSQCause.Illegal;
                else
                    cause = DSQCause.Fuel;
            }
            else
            {
                cause = DSQCause.Dangerous;
            }

            return cause;
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/getResults")]
        public IActionResult GetResults(int id, int raceId)
        {
            var driverResults = _context.DriverResults
                .IgnoreQueryFilters()
                .Where(res => res.RaceId == raceId)
                .Include(res => res.SeasonDriver.Driver)
                .Include(res => res.SeasonDriver.SeasonTeam.Team)
                .OrderBy(res => res.SeasonDriver.SeasonTeam.Team.Abbreviation).ToList();

            return new JsonResult(driverResults, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        public async Task<IActionResult> FinishRace(int seasonId, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season)
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.SeasonTeam)
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                        .ThenInclude(sd => sd.Driver)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == seasonId);

            foreach (var result in race.DriverResults)
            {
                int points = 0;
                if (result.Position <= race.Season.PointsPerPosition.Keys.Max() && result.Status == Status.Finished)
                {
                    points = race.Season.PointsPerPosition[result.Position].Value;
                }
                if (result.Grid == 1)
                {
                    points += race.Season.PolePoints;
                }
                result.SeasonDriver.Points += points;
                result.SeasonDriver.SeasonTeam.Points += points;
                _context.UpdateRange(result.SeasonDriver, result.SeasonDriver.SeasonTeam);
            }

            race.RaceState = RaceState.Finished;
            _context.Update(race);
            _context.Update(race.Track);
            _context.SaveChanges();

            return RedirectToAction("DriverStandings", "Home");
        }

        [Route("Season/{id}/[Controller]/{raceId}/Qualifying")]
        public IActionResult Qualifying(int id, int raceId)
        {
            var race = _context.Races.Single(r => r.RaceId == raceId);

            race.RaceState = RaceState.Qualifying;
            _context.Update(race);
            _context.SaveChanges();

            var season = _context.Seasons.Single(s => s.SeasonId == id);
            ViewBag.race = race;
            ViewBag.season = season;
            return View();
        }

        [Route("Season/{id}/[Controller]/{raceId}/Qualifying/Update")]
        public async Task<IActionResult> UpdateQualifying(int id, int raceId, string source, bool secondRun)
        {
            if (string.IsNullOrWhiteSpace(source))
                return BadRequest();

            try
            {
                var race = await _context.Races
                    .Include(r => r.Season)
                    .Include(r => r.Track)
                    .Include(r => r.DriverResults)
                        .ThenInclude(dr => dr.SeasonDriver)
                            .ThenInclude(sd => sd.SeasonTeam)
                                .ThenInclude(st => st.Team)
                    .Include(r => r.DriverResults)
                        .ThenInclude(dr => dr.SeasonDriver)
                            .ThenInclude(sd => sd.SeasonTeam)
                                .ThenInclude(sd => sd.Engine)
                    .Include(r => r.DriverResults)
                        .ThenInclude(dr => dr.SeasonDriver)
                            .ThenInclude(sd => sd.Driver)
                    .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == id)
                    ;

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
                    int qualypace = qualifyingDriver.DriverResults.Single(dr => dr.RaceId == race.RaceId).QualyMod;
                    var previousScore = qualificationResult.Score;
                    qualificationResult.Score = _resultGenerator.GetQualifyingResult(qualifyingDriver, race.Season.QualificationRNG, race.Track, qualypace);

                    if (secondRun)
                    {
                        if (previousScore > qualificationResult.Score)
                            qualificationResult.Score = previousScore;
                    }
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
                        TeamName = driver.SeasonTeam.Name,
                        Colour = driver.SeasonTeam.Colour,
                        Accent = driver.SeasonTeam.Accent,
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
            var resultcontext = _context.DriverResults;

            IQueryable<Qualification> qualyresult = _context.Qualification.Where(q => q.RaceId == raceId);
            List<DriverResult> driverResults = resultcontext.Where(d => d.RaceId == raceId).ToList();
            var currentSeasonResults = resultcontext.Where(d => d.SeasonDriver.SeasonId == id).Include(c => c.Race).ToList();
            var race = _context.Races.Single(r => r.RaceId == raceId);

            List<Qualification> qualifications = new List<Qualification>();
            //Adds results from Qualification to Grid in DriverResults (Penalties may be applied here too)
            foreach (Qualification result in qualyresult)
            {
                DriverResult driver = driverResults.Single(d => d.RaceId == result.RaceId &&
                    d.SeasonDriverId == result.DriverId);

                DriverResult lastDriverResult = currentSeasonResults.OrderBy(s => s.Race.Round)
                    .LastOrDefault(d => d.SeasonDriverId == result.DriverId && d.Race.RaceState == RaceState.Finished);

                result.PenaltyPosition = result.Position;
                // Checks if a driver should get a penalty
                if (lastDriverResult != null)
                {
                    // If the driver was disqualified, then he will always start last.
                    if (lastDriverResult.Status == Status.DSQ)
                    {
                        result.PenaltyPosition += 99;
                        switch (lastDriverResult.DSQCause)
                        {
                            case DSQCause.Illegal:
                                driver.PenaltyReason = "Illegal car in last race.";
                                break;
                            case DSQCause.Fuel:
                                driver.PenaltyReason = "Exceeded fuel limits last race.";
                                break;
                            case DSQCause.Dangerous:
                                driver.PenaltyReason = "Dangerous driving last race.";
                                break;
                        }
                    }
                    else if (lastDriverResult.Status == Status.DNF)
                    {
                        switch (lastDriverResult.DNFCause)
                        {
                            case DNFCause.Collision:
                                int random = rng.Next(1, 6);
                                if (random < 3)
                                {
                                    result.PenaltyPosition += 3.2;
                                    driver.PenaltyReason = "+3 grid penalty for causing a collision.";
                                }
                                break;
                            case DNFCause.Accident:
                                int amountAccidents = (currentSeasonResults.Where(d => d.DNFCause == DNFCause.Accident && d.SeasonDriverId == driver.SeasonDriverId).Count());
                                if (amountAccidents > 2)
                                {
                                    result.PenaltyPosition += 5.3;
                                    driver.PenaltyReason = "+5 grid penalty for excessive amount of accidents";
                                }
                                break;
                            case DNFCause.Engine:
                                int amountEngines = (currentSeasonResults.Where(d => d.DNFCause == DNFCause.Engine && d.SeasonDriverId == driver.SeasonDriverId).Count());
                                if (amountEngines > 2)
                                {
                                    result.PenaltyPosition += 10.9;
                                    driver.PenaltyReason = "+10 grid penalty for excessive engines used";
                                }
                                break;
                            case DNFCause.Electrics:
                                int amountElectrics = (currentSeasonResults.Where(d => d.DNFCause == DNFCause.Electrics && d.SeasonDriverId == driver.SeasonDriverId).Count());
                                if (amountElectrics > 1)
                                {
                                    result.PenaltyPosition += 5.1;
                                    driver.PenaltyReason = "+5 penalty for excessive electrics used";
                                }
                                break;
                        }
                    }
                }
                qualifications.Add(result);
            }

            // Orders the qualifications after applying penalties to positions, then assigns the grid position to it
            int assignedPosition = 1;

            foreach (var result in qualifications.OrderBy(q => q.PenaltyPosition))
            {
                DriverResult driver = driverResults.Single(d => d.RaceId == result.RaceId &&
                    d.SeasonDriverId == result.DriverId);

                if (driver == null)
                {
                    return StatusCode(500);
                }

                driver.Grid = assignedPosition;
                driver.Position = assignedPosition;

                assignedPosition++;
            }

            race.RaceState = RaceState.Race;
            _context.Update(race);
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
                .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == id)
                ;

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