using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Builders;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Utility;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ResultGenerators;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FormuleCirkelEntity.Controllers
{
    public class RacesController : FormulaController
    {
        private readonly IRaceService _raceService;
        private readonly ISeasonService _seasonService;
        private readonly ITrackService _trackService;
        private readonly RaceResultGenerator _resultGenerator;
        private readonly RaceBuilder _raceBuilder;
        private static readonly Random rng = new Random();

        public RacesController(FormulaContext context, 
            UserManager<SimUser> userManager,
            IRaceService raceService,
            ISeasonService seasonService,
            ITrackService trackService,
            RaceResultGenerator raceResultGenerator, 
            RaceBuilder raceBuilder)
            : base(context, userManager)
        {
            _raceService = raceService;
            _seasonService = seasonService;
            _trackService = trackService;
            _resultGenerator = raceResultGenerator;
            _raceBuilder = raceBuilder;
        }

        // [ADVICE]: Consider developing a viewmodel for this method
        [Authorize(Roles = "Admin")]
        [Route("Season/{id}/[Controller]/Add/")]
        public async Task<IActionResult> AddTracks(int id)
        {
            // Gets the current season and returns a "wow cant see shizzle" when it isnt found
            var season = await _seasonService.GetSeasonById(id, true);
            if (season == null)
                return NotFound();

            var existingTrackIds = season.Races
                .Select(r => r.TrackId)
                .ToList();
            var unusedTracks = await _trackService.GetUnusedTracks(existingTrackIds);

            ViewBag.seasonId = id;
            return View(unusedTracks);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Season/{id}/[Controller]/Add/")]
        public async Task<IActionResult> AddTracks(int id, [Bind("TrackId")] int trackId)
        {
            var track = await _trackService.GetEntityById(trackId);
            var season = await _seasonService.GetSeasonById(id, true, true);
            if (track == null || season == null)
                return NotFound();

            // Finds the last time track was used and uses same stintsetup as then
            var lastracemodel = await _raceService.GetLastRace(season.ChampionshipId, trackId);
            Race race;
            if(lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints;
                race = _raceBuilder
                    .InitializeRace(track, season)
                    .AddModifiedStints(stintlist)
                    .GetResultAndRefresh();
            }
            else
            {
                race = _raceBuilder
                    .InitializeRace(track, season)
                    .AddDefaultStints()
                    .GetResultAndRefresh();
            }

            season.Races.Add(race);
            _seasonService.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddTracks), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ModifyRace(int id, int trackId)
        {
            var season = await _seasonService.GetSeasonById(id);
            var track = await _trackService.GetEntityById(trackId);
            var model = new RacesModifyRaceModel
            {
                SeasonId = id,
                TrackId = trackId,
                TrackName = track.Name,
            };
            // Finds the last time track was used and uses same stintsetup as then if it exists
            var lastracemodel = await _raceService.GetLastRace(season.ChampionshipId, trackId);
            if (lastracemodel != null)
            {
                var stintlist = lastracemodel.Stints;
                foreach (var stint in stintlist)
                    model.RaceStints.Add(stint);
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ModifyRace(RacesModifyRaceModel raceModel)
        {
            // Those gosh darn warnings against checking for nulls, but here we are
            if (raceModel == null)
                return NotFound();

            var track = await _trackService.GetEntityById(raceModel.TrackId);
            var season = await _seasonService.GetSeasonById(raceModel.SeasonId, true, true);
            // Add the created stints to the racebuilder so the race is correctly set up
            var race = _raceBuilder
                .InitializeRace(track, season)
                .AddModifiedStints(raceModel.RaceStints)
                .GetResultAndRefresh();

            season.Races.Add(race);
            _seasonService.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddTracks", new { id = raceModel.SeasonId });
        }

        [Route("Season/{id}/[Controller]/{raceId}")]
        public async Task<IActionResult> Race(int raceId)
        {
            var race = await _raceService.GetRaceByIdAsync(raceId, true);
            var trackSpec = await _trackService.GetTrackSpecification(race.TrackId);
            var currentSeason = await _seasonService.GetSeasonById(race.SeasonId);
            var queryDrivers = _raceService.GetDriverResultQuery();
            var drivers = await queryDrivers
                .IgnoreQueryFilters()
                .Where(dr => dr.RaceId == raceId)
                .Include(dr => dr.CurrTyre)
                .Include(dr => dr.StintResults)
                .Include(dr => dr.SeasonDriver)
                    .ThenInclude(sd => sd.Driver)
                .Include(dr => dr.SeasonDriver.SeasonTeam)
                    .ThenInclude(st => st.Engine)
                .OrderBy(dr => dr.Position)
                .ToListAsync();
            // Racetitle is created from the year, round number and name of the race. Eventually gets added to the viewmodel
            string raceTitle = $"{currentSeason.SeasonNumber} - Round {race.Round} - {race.Name}";
            RacesRaceModel viewmodel = new RacesRaceModel
            {
                RaceId = raceId,
                SeasonId = currentSeason.SeasonId,
                Weather = race.Weather.ToString(),
                PointsPerPosition = currentSeason.PointsPerPosition,
                MaxPos = currentSeason.PointsPerPosition.Keys.Max(),
                CountDrivers = drivers.Count,
                FullRaceTitle = raceTitle,
                ShowRaceButtons = (race.RaceState == RaceState.Race),
                IsAdmin = User.IsInRole(Constants.RoleAdmin)
            };
            // Fills the driverResults in the viewmodel and adds the total power ranking of the driver too
            foreach (var driver in drivers)
            {
                int modifiers = (driver.DriverRacePace + driver.ChassisRacePace + driver.EngineRacePace);
                viewmodel.DriverResults.Add(driver);
                viewmodel.Power.Add(Helpers.GetPowerDriver(driver.SeasonDriver, modifiers, trackSpec.ToString()));
            }
            // Adds the stints of the race to the viewmodel
            foreach (var stint in race.Stints)
                viewmodel.RaceStints.Add(stint);

            return View(viewmodel);
        }
        
        // [ADVICE]: Why isn't a viewmodel used here?
        [Route("Season/{id}/[Controller]/{raceId}/Preview")]
        public async Task<IActionResult> RacePreview(int raceId)
        {
            var race = await _context.Races
                .IgnoreQueryFilters()
                .Include(r => r.Stints)
                .Include(r => r.Season)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            var trackTraits = await _context.TrackTraits
                .Where(trt => trt.TrackId == race.TrackId)
                .Include(trt => trt.Trait)
                .ToListAsync();

            ViewBag.favourites = Favourites(race);
            ViewBag.tracktraits = trackTraits;

            return View(race);
        }

        // Gets the three favourites for that race.
        private List<SeasonTeam> Favourites(Race race)
        {
            var teams = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Where(st => st.SeasonId == race.SeasonId)
                .Include(st => st.Team)
                .Include(st => st.Engine)
                .AsEnumerable()
                .OrderByDescending(st => (st.Chassis + st.Engine.Power + Helpers.GetChassisBonus(Helpers.CreateTeamSpecDictionary(st), race.Track.Specification.ToString())))
                .Take(3)
                .ToList();

            return teams;
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/Start")]
        public async Task<IActionResult> RaceStart(int id, int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Stints)
                .Include(r => r.Season.Drivers)
                .Include(r => r.DriverResults)
                    .ThenInclude(r => r.StintResults)
                .Include(r => r.DriverResults)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            if (!race.DriverResults.Any())
            {
                // Checks if the user activating the race is the admin
                SimUser user = await _userManager.GetUserAsync(User);
                var result = await _userManager.IsInRoleAsync(user, Constants.RoleAdmin);
                if (result)
                {
                    race = _raceBuilder
                        .Use(race)
                        .AddAllDrivers()
                        .GetResult();

                    var driverTraits = await _context.DriverTraits
                        .Include(drt => drt.Trait)
                        .ToListAsync();
                    var teamTraits = await _context.TeamTraits
                        .Include(ttr => ttr.Trait)
                        .ToListAsync();
                    var trackTraits = await _context.TrackTraits
                        .Include(tet => tet.Trait)
                        .Where(trt => trt.TrackId == race.TrackId)
                        .ToListAsync();
                    var seasonTeams = await _context.SeasonTeams.Where(st => st.SeasonId == race.SeasonId).ToListAsync();
                    // Get all the possible strategies for this race
                    var strategies = await _context.Strategies
                        .Where(s => s.RaceLen == race.Stints.Count)
                        .Include(s => s.Tyres)
                            .ThenInclude(t => t.Tyre)
                        .ToListAsync();
                    // If the length is zero then creates a list consisting of the single, default strategy
                    if (strategies.Count == 0)
                        strategies = await _context.Strategies
                            .Where(s => s.StrategyId == 1)
                            .Include(s => s.Tyres)
                                .ThenInclude(t => t.Tyre)
                            .ToListAsync();
                    foreach (var driverRes in race.DriverResults)
                    {
                        // Gets the traits from the driver in the loop and sets them
                        var thisDriverTraits = driverTraits.Where(drt => drt.DriverId == driverRes.SeasonDriver.DriverId);
                        RaceService.SetDriverTraitMods(driverRes, thisDriverTraits);
                        // Gets the seasonteam of the driver in the loop
                        var thisDriverTeam = seasonTeams.First(st => st.SeasonDrivers.Contains(driverRes.SeasonDriver));
                        // Gets the traits from the team of the driver in the loop and sets them
                        var thisTeamTraits = teamTraits.Where(ttr => ttr.TeamId == thisDriverTeam.TeamId);
                        RaceService.SetTeamTraitMods(driverRes, thisTeamTraits);
                        // Sets the traits from the track to the driver in the loop
                        RaceService.SetTrackTraitMods(driverRes, trackTraits);

                        // Set a random strategy
                        int stratIndex = rng.Next(0, strategies.Count);
                        RaceService.SetRandomStrategy(driverRes, strategies[stratIndex]);
                    }
                    _context.DriverResults.AddRange(race.DriverResults);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Forbid();
                }
            }
            return RedirectToAction("RaceWeekend", new { id, raceId });
        }

        [Route("Season/{id}/[Controller]/{raceId}/Weekend")]
        public async Task<IActionResult> RaceWeekend(int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Season)
                .Include(r => r.Stints)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            var drivers = await _context.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.RaceId == raceId)
                .Include(dr => dr.Strategy)
                    .ThenInclude(dr => dr.Tyres)
                    .ThenInclude(dr => dr.Tyre)
                .Include(dr => dr.SeasonDriver)
                    .ThenInclude(sd => sd.Driver)
                    .ThenInclude(d => d.DriverTraits)
                    .ThenInclude(drt => drt.Trait)
                .Include(dr => dr.SeasonDriver.SeasonTeam)
                    .ThenInclude(st => st.Team)
                    .ThenInclude(t => t.TeamTraits)
                    .ThenInclude(tet => tet.Trait)
                .ToListAsync();

            // Checks if the user is authenticated and sends the list of owned team id's if that's the case
            // Other wise assigns an empty int list to prevent a nullreference in the view
            if (User.Identity.IsAuthenticated)
            {
                SimUser simuser = await _userManager.GetUserAsync(User);
                ViewBag.owneddrivers = simuser.Drivers;
            }
            else
                ViewBag.owneddrivers = new List<Driver>();

            RaceWeekendModel viewmodel = new RaceWeekendModel
            {
                Year = race.Season.SeasonNumber,
                Race = race,
                DriverResults = drivers
            };

            return View(viewmodel);
        }

        public async Task<IActionResult> ChangeStrategy(int driverId, int raceId)
        {
            var driverResult = await _context.DriverResults.FindAsync(driverId);
            var race = await _context.Races
                .Include(r => r.Stints)
                .FirstOrDefaultAsync(r => r.RaceId == raceId);
            if (driverResult is null || race is null)
                return NotFound();

            var availableStrategies = await _context.Strategies
                .Where(s => s.RaceLen == race.Stints.Count)
                .Include(s => s.Tyres)
                    .ThenInclude(st => st.Tyre)
                .ToListAsync();

            ViewBag.driverId = driverId;
            ViewBag.raceId = raceId;
            return View(availableStrategies);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStrategy(int driverId, int raceId, int strategyId)
        {
            var driverResult = await _context.DriverResults.FindAsync(driverId);
            var race = await _context.Races
                .FirstOrDefaultAsync(r => r.RaceId == raceId);
            var strategy = await _context.Strategies
                .Include(s => s.Tyres)
                    .ThenInclude(st => st.Tyre)
                .FirstOrDefaultAsync(s => s.StrategyId == strategyId);
            var currentTyre = strategy.Tyres.Single(t => t.StintNumberApplied == 1).Tyre;
            if (driverResult is null || strategy is null)
                return NotFound();

            driverResult.Strategy = strategy;
            driverResult.CurrTyre = currentTyre;
            driverResult.TyreLife = currentTyre.Pace;
            _context.Update(driverResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RaceWeekend), new { id = race.SeasonId, raceId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Season/{id}/[Controller]/{raceId}/Advance")]
        public async Task<IActionResult> AdvanceStint(int raceId)
        {
            var race = await _context.Races
                .Include(r => r.Stints)
                .Include(r => r.Track)
                .SingleOrDefaultAsync(r => r.RaceId == raceId);

            var driverResults = await _context.DriverResults
                .Where(dr => dr.RaceId == raceId)
                .Include(dr => dr.StintResults)
                .Include(dr => dr.CurrTyre)
                .Include(dr => dr.Strategy)
                    .ThenInclude(dr => dr.Tyres)
                    .ThenInclude(dr => dr.Tyre)
                .Include(dr => dr.SeasonDriver)
                .ToListAsync();

            var season = await _context.Seasons
                .AsNoTracking()
                .FirstAsync(s => s.SeasonId == race.SeasonId);

            var teams = await _context.SeasonTeams
                .AsNoTracking()
                .Where(st => st.SeasonId == season.SeasonId)
                .Include(st => st.Team)
                .Include(st => st.Engine)
                .ToListAsync();

            if (race.StintProgress == race.Stints.Count)
                return BadRequest();

            race.StintProgress++;
            var stint = race.Stints.Single(s => s.Number == race.StintProgress);

            // Calculate results for all drivers who have not been DSQ'd or DNF'd.
            foreach (var result in driverResults.Where(d => d.Status == Status.Finished))
            {
                var stintResult = result.StintResults.Single(sr => sr.Number == race.StintProgress);
                var currentTeam = teams.First(t => t.SeasonTeamId == result.SeasonDriver.SeasonTeamId);

                _resultGenerator.UpdateStintResult(stintResult, stint, result, currentTeam, race.Weather, race.Track.Specification, driverResults.Count, season.QualyBonus, season.PitMin, season.PitMax);
                
                // Driver isn't running anymore, which indicates that he DNFed
                if (stintResult.StintStatus == StintStatus.DriverDNF || stintResult.StintStatus == StintStatus.ChassisDNF)
                {
                    // RNG to determine the type of DNF.
                    int dnfvalue = rng.Next(1, 26);
                    if (dnfvalue == 25)
                    {
                        result.Status = Status.DSQ;
                        stintResult.Result += -2000;
                        if (stintResult.StintStatus == StintStatus.DriverDNF)
                            result.DSQCause = Helpers.RandomDriverDSQ();
                        else
                            result.DSQCause = Helpers.RandomChassisDSQ();
                    } 
                    else
                    {
                        result.Status = Status.DNF;
                        stintResult.Result += -1000;
                        if (stintResult.StintStatus == StintStatus.DriverDNF)
                            result.DNFCause = Helpers.RandomDriverDNF();
                        else
                            result.DNFCause = Helpers.RandomChassisDNF();
                    }
                }
                // It sums the score of all stintresults of a driver, could maybe also just add the current score up
                result.Points = result.StintResults.Sum(sr => sr.Result);
            }

            RaceResultGenerator.GetPositionsBasedOnRelativePoints(driverResults, race.StintProgress);
            _context.Update(race);
            _context.UpdateRange(driverResults);
            await _context.SaveChangesAsync();

            // Clear unneeded references so they won't be serialized and sent over
            foreach(var dr in driverResults)
            {
                dr.SeasonDriver = null;
                dr.Strategy = null;
            }
            return new JsonResult(driverResults, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost("Season/{id}/[Controller]/{raceId}/getResults")]
        public IActionResult GetResults(int raceId)
        {
            var driverResults = _context.DriverResults
                .IgnoreQueryFilters()
                .Where(res => res.RaceId == raceId)
                .Include(res => res.StintResults)
                .Include(res => res.SeasonDriver.Driver)
                .Include(res => res.SeasonDriver.SeasonTeam.Team)
                .OrderBy(res => res.SeasonDriver.SeasonTeam.Team.Abbreviation)
                .ToList();

            return new JsonResult(driverResults, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, NullValueHandling = NullValueHandling.Ignore });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> FinishRace(int seasonId, int raceId)
        {
            var race = await _context.Races
                .SingleOrDefaultAsync(r => r.RaceId == raceId && r.SeasonId == seasonId);

            var driverResults = await _context.DriverResults
                .Where(dr => dr.RaceId == raceId)
                .Include(dr => dr.SeasonDriver)
                    .ThenInclude(sd => sd.SeasonTeam)
                .ToListAsync();

            var season = await _context.Seasons
                .AsNoTracking()
                .FirstAsync(s => s.SeasonId == seasonId);

            foreach (var result in driverResults)
            {
                int points = 0;
                if (result.Position <= season.PointsPerPosition.Keys.Max() && result.Status == Status.Finished)
                {
                    points = season.PointsPerPosition[result.Position].Value;
                }
                if (result.Grid == 1)
                {
                    points += season.PolePoints;
                }
                result.SeasonDriver.Points += points;
                result.SeasonDriver.SeasonTeam.Points += points;
                _context.UpdateRange(result.SeasonDriver, result.SeasonDriver.SeasonTeam);
            }

            race.RaceState = RaceState.Finished;
            _context.Update(race);
            await _context.SaveChangesAsync();

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

        [Authorize(Roles = "Admin")]
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

        private static IList<Qualification> GetQualificationsFromDrivers(IList<SeasonDriver> drivers, int raceId)
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

        private static int GetQualifyingDriverLimit(string qualifyingStage, Season season)
        {
            if (qualifyingStage == "Q2")
                return season.QualificationRemainingDriversQ2;
            if (qualifyingStage == "Q3")
                return season.QualificationRemainingDriversQ3;
            return season.Drivers.Count;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Return(int seasonId, int raceId)
        {
            var race = await _context.Races
                .SingleAsync(r => r.RaceId == raceId);

            var previousRace = await _context.Races
                .AsNoTracking()
                .Include(r => r.DriverResults)
                .FirstOrDefaultAsync(r => r.SeasonId == seasonId && r.Round == (race.Round - 1));

            var qualyresults = await _context.Qualification
                .Where(q => q.RaceId == raceId)
                .ToListAsync();

            var seasonResults = await _context.DriverResults
                .Where(d => d.Race.SeasonId == seasonId)
                .ToListAsync();

            var raceResults = seasonResults
                .Where(sr => sr.RaceId == raceId)
                .ToList();

            //Adds results from Qualification to Grid in DriverResults (Penalties may be applied here too)
            foreach (Qualification result in qualyresults)
            {
                DriverResult driver = raceResults.Single(d => d.SeasonDriverId == result.DriverId);

                result.PenaltyPosition = result.Position;
                if (previousRace != null)
                {
                    DriverResult lastDriverResult = previousRace.DriverResults.FirstOrDefault(dr => dr.SeasonDriverId == result.DriverId);
                    // Checks if a driver should get a penalty
                    SetDriverPenalty(lastDriverResult, driver, result, seasonResults);
                }
            }

            // Orders the qualifications after applying penalties to positions, then assigns the grid position to it
            int assignedPosition = 1;

            foreach (var result in qualyresults.OrderBy(q => q.PenaltyPosition))
            {
                DriverResult driver = raceResults.Single(d => d.SeasonDriverId == result.DriverId);
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
            _context.UpdateRange(raceResults);
            await _context.SaveChangesAsync();

            return RedirectToAction("RaceWeekend", new { id = seasonId, raceId });
        }

        private void SetDriverPenalty(DriverResult lastDriverResult, DriverResult driver, 
            Qualification result, List<DriverResult> seasonResults)
        {
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
                    var currentDriverSeasonResults = seasonResults.Where(sr => sr.SeasonDriverId == result.DriverId);
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
                            int amountAccidents = (currentDriverSeasonResults.Where(d => d.DNFCause == DNFCause.Accident).Count());
                            if (amountAccidents > 2)
                            {
                                result.PenaltyPosition += 5.3;
                                driver.PenaltyReason = "+5 grid penalty for excessive amount of accidents";
                            }
                            break;
                        case DNFCause.Engine:
                            int amountEngines = (currentDriverSeasonResults.Where(d => d.DNFCause == DNFCause.Engine).Count());
                            if (amountEngines > 2)
                            {
                                result.PenaltyPosition += 10.9;
                                driver.PenaltyReason = "+10 grid penalty for excessive engines used";
                            }
                            break;
                        case DNFCause.Electrics:
                            int amountElectrics = (currentDriverSeasonResults.Where(d => d.DNFCause == DNFCause.Electrics).Count());
                            if (amountElectrics > 1)
                            {
                                result.PenaltyPosition += 5.1;
                                driver.PenaltyReason = "+5 penalty for excessive electrics used";
                            }
                            break;
                    }
                }
            }
        }

        [Authorize(Roles = "Admin")]
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
            race.DriverResults.Clear();
            race.Season = null;
            race.Track = null;
            race.Stints.Clear();
            raceToSwitch.DriverResults.Clear();
            raceToSwitch.Season = null;
            raceToSwitch.Track = null;
            raceToSwitch.Stints.Clear();
            return new JsonResult(new[] { race, raceToSwitch });
        }
    }
}