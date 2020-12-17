﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using FormuleCirkelEntity.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : FormulaController
    {
        private static readonly Random rng = new Random();

        public SeasonController(FormulaContext context, 
            UserManager<SimUser> userManager)
            : base(context, userManager)
        { }

        public async Task<IActionResult> Index()
        {
            var seasons = await _context.Seasons
                .IgnoreQueryFilters()
                .Where(s => s.Championship.ActiveChampionship)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(s => s.Team)
                .OrderByDescending(s => s.SeasonNumber)
                .ToListAsync();

            var championship = _context.Championships.FirstOrDefault(s => s.ActiveChampionship);
            if (championship is null)
                ViewBag.championship = "NaN";
            else
                ViewBag.championship = championship.ChampionshipName;

            var champs = await _context.Championships
                .ToListAsync();

            ViewBag.champs = champs;

            return View(seasons);
        }

        [ActionName("ChampionshipSelect")]
        public async Task<IActionResult> Index(int championshipId)
        {
            var seasons = await _context.Seasons
                .IgnoreQueryFilters()
                .Where(s => s.ChampionshipId == championshipId)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(s => s.Team)
                .OrderByDescending(s => s.SeasonNumber)
                .ToListAsync();

            var championship = _context.Championships.FirstOrDefault(s => s.ChampionshipId == championshipId);
            if (championship is null)
                ViewBag.championship = "NaN";
            else
                ViewBag.championship = championship.ChampionshipName;

            var champs = await _context.Championships
                .ToListAsync();

            ViewBag.champs = champs;

            return View("Index", seasons);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var season = new Season();
            var championship = await _context.Championships.FirstAsync(c => c.ActiveChampionship);
            if (championship is null)
                return NotFound();

            season.Championship = championship;
            await _context.AddAsync(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDefault(int? id)
        {
            // gets the current and previous season in this championship
            var season = await _context.Seasons
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var lastSeason = _context.Seasons
                .Include(s => s.Races)
                    .ThenInclude(r => r.Stints)
                .Include(s => s.Championship)
                .ToList()
                .LastOrDefault(s => s.State == SeasonState.Finished && s.ChampionshipId == season.ChampionshipId);

            if (lastSeason != null)
            {
                foreach (var pointPosition in lastSeason.PointsPerPosition)
                    season.PointsPerPosition.Add(pointPosition);
                season.PolePoints = lastSeason.PolePoints;
                season.QualificationRemainingDriversQ2 = lastSeason.QualificationRemainingDriversQ2;
                season.QualificationRemainingDriversQ3 = lastSeason.QualificationRemainingDriversQ3;
                season.QualificationRNG = lastSeason.QualificationRNG;
                season.QualyBonus = lastSeason.QualyBonus;
                season.SeasonNumber = (lastSeason.SeasonNumber + 1);

                // Adds the previous season races if there aren't any added yet.
                if (season.Races.Count == 0)
                {
                    foreach (var race in lastSeason.Races)
                    {
                        Race newRace = new Race
                        {
                            Season = season,
                            Name = race.Name,
                            Round = race.Round,
                            TrackId = race.TrackId,
                            Weather = Helpers.RandomWeather(),
                            RaceState = RaceState.Concept
                        };
                        foreach (var stint in race.Stints)
                            newRace.Stints.Add(stint);
                        season.Races.Add(newRace);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Start(int seasonId)
        {
            // First sets a few settings of the season
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == seasonId);
            if (season is null)
                return NotFound();

            season.SeasonStart = DateTime.Now;
            season.State = SeasonState.Progress;
            if (season.PointsPerPosition.Count == 0)
            {
                SeasonService.AddDefaultPoints(season);
            }
            _context.Update(season);

            // From here on it's all about setting the driver results for each race, NEEDS TO BE TESTED THOROUGHLY
            var driverTraits = await _context.DriverTraits
                .Include(drt => drt.Trait)
                .ToListAsync();
            var teamTraits = await _context.TeamTraits
                .Include(ttr => ttr.Trait)
                .ToListAsync();
            var trackTraits = await _context.TrackTraits
                .Include(tet => tet.Trait)
                .ToListAsync();
            var seasonTeams = await _context.SeasonTeams
                .Where(st => st.SeasonId == seasonId)
                .ToListAsync();
            // Get all the possible strategies for this race
            var strategies = await _context.Strategies
                .Include(s => s.Tyres)
                    .ThenInclude(t => t.Tyre)
                .ToListAsync();
            // Get all the races from this season
            var races = await _context.Races
                .Where(r => r.SeasonId == seasonId)
                .Include(r => r.Stints)
                .Include(r => r.Season.Drivers)
                .Include(r => r.DriverResults)
                    .ThenInclude(r => r.StintResults)
                .ToListAsync();

            foreach (var race in races)
            {
                RaceService.AddSeasonDriversToRace(race, race.Season.Drivers);

                var raceStrategies = strategies.Where(s => s.RaceLen == race.Stints.Count).ToList();
                var raceTrackTraits = trackTraits.Where(t => t.TrackId == race.TrackId).ToList();
                // If the length is zero then creates a list consisting of the single, default strategy
                if (!raceStrategies.Any())
                    raceStrategies = strategies.Where(s => s.StrategyId == 1).ToList();
                // Iterate through the created driverresults so that the modifications can also be set
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
                    RaceService.SetTrackTraitMods(driverRes, raceTrackTraits);
                    // Set a random strategy
                    int stratIndex = rng.Next(0, raceStrategies.Count);
                    RaceService.SetRandomStrategy(driverRes, raceStrategies[stratIndex]);
                }
                _context.DriverResults.AddRange(race.DriverResults);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Finish(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            if (season is null)
                return NotFound();

            season.State = SeasonState.Finished;
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await _context.Seasons
                .IgnoreQueryFilters()
                .Include(s => s.Races)
                    .ThenInclude(r => r.Track)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var drivers = await _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(sd => sd.Driver)
                .Where(sd => sd.SeasonId == id)
                .ToListAsync();

            var teams = await _context.SeasonTeams
                .IgnoreQueryFilters()
                .Include(t => t.Team)
                .Include(t => t.Engine)
                .Where(st => st.SeasonId == id)
                .ToListAsync();

            if (season == null)
                return NotFound();

            var seasonmodel = new SeasonDetailModel
            {
                Season = season,
                SeasonDrivers = drivers,
                SeasonTeams = teams
            };
            return View(nameof(Detail), seasonmodel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRace(int? id, int seasonId)
        {
            var race = await _context.Races.SingleOrDefaultAsync(s => s.RaceId == id);
            var season = await _context.Seasons.Include(s => s.Races).SingleOrDefaultAsync(s => s.SeasonId == seasonId);
            if (race == null)
                return NotFound();

            var stints = await _context.Stints.Where(s => s.RaceId == race.RaceId).ToListAsync();
            season.Races.Remove(race);
            int round = 0;
            foreach (var seasonRace in season.Races.OrderBy(r => r.Round))
            {
                round++;
                seasonRace.Round = round;
            }

            _context.RemoveRange(stints);
            _context.Remove(race);
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        public async Task<IActionResult> SeasonStats(int? id)
        {
            var season = await _context.Seasons
                .AsNoTracking()
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season is null)
                return NotFound();

            ViewBag.seasonId = id;
            ViewBag.number = season.SeasonNumber;

            var seasondrivers = _context.SeasonDrivers
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonId == id)
                .Include(sd => sd.Driver)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Team)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Engine)
                .OrderByDescending(sd => (sd.Skill + sd.SeasonTeam.Chassis + sd.SeasonTeam.Engine.Power + (((int)sd.DriverStatus) * -2) + 2))
                .ToList();

            return View(seasondrivers);
        }

        [Route("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season is null)
                return NotFound();

            return View(new SeasonSettingsViewModel(season));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id, [Bind] SeasonSettingsViewModel settingsModel)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season is null || settingsModel is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                season.SeasonNumber = settingsModel.SeasonNumber;
                season.QualificationRNG = settingsModel.QualificationRNG;
                season.QualificationRemainingDriversQ2 = settingsModel.QualificationRemainingDriversQ2;
                season.QualificationRemainingDriversQ3 = settingsModel.QualificationRemainingDriversQ3;
                season.QualyBonus = settingsModel.QualyBonus;
                season.PitMin = settingsModel.PitMin;
                season.PitMax = settingsModel.PitMax;
                season.PolePoints = settingsModel.PolePoints;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
            }

            return View(settingsModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPoints(int id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            List<int> points = new List<int>();
            foreach (var value in season.PointsPerPosition.Values)
                points.Add(value.Value);

            var model = new SeasonSetPointsModel
            {
                SeasonId = id,
                SeasonNumber = season.SeasonNumber,
                Points = points
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetPoints(SeasonSetPointsModel model)
        {
            if (model is null)
                return NotFound();

            var season = await _context.Seasons.SingleAsync(s => s.SeasonId == model.SeasonId);
            Dictionary<int, int?> pairs = new Dictionary<int, int?>();
            int position = 1;
            foreach (var points in model.Points)
            {
                pairs.Add(position, points);
                position++;
            }

            foreach (var pair in pairs)
                season.PointsPerPosition.Add(pair);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Settings), new { id = model.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Teams/Add")]
        public async Task<IActionResult> AddTeams(int? id)
        {
            var seasons = await _context.Seasons
                .Where(s => s.State == SeasonState.Draft || s.State == SeasonState.Progress)
                .Include(s => s.Teams)
                .ToListAsync();

            if (seasons is null)
                return NotFound();

            List<int> existingTeamIds = new List<int>();
            foreach (var season in seasons)
            {
                existingTeamIds.AddRange(season.Teams.Select(t => t.TeamId));
            }
            var unregisteredTeams = await _context.Teams
                .Where(t => t.Archived == false)
                .Where(t => !existingTeamIds.Contains(t.Id))
                .OrderBy(t => t.Abbreviation)
                .ToListAsync();

            ViewBag.seasonId = id;
            return View(unregisteredTeams);
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Teams/Add/{globalTeamId}")]
        public async Task<IActionResult> AddTeam(int? id, int? globalTeamId)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId);

            if (season is null || globalTeam is null)
                return NotFound();

            var engines = await _context.Engines
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();

            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            ViewBag.seasonId = id;

            var seasonTeam = new SeasonTeam
            {
                Team = globalTeam,
                Season = season
            };

            // Adds last previous used values from team as default
            var allSeasonTeams = await _context.SeasonTeams.ToListAsync();
            var lastTeam = allSeasonTeams.LastOrDefault(lt => lt.TeamId == globalTeamId);

            if (lastTeam != null)
            {
                seasonTeam.Name = lastTeam.Name;
                seasonTeam.Principal = lastTeam.Principal;
                seasonTeam.Colour = lastTeam.Colour;
                seasonTeam.Accent = lastTeam.Accent;
                seasonTeam.Chassis = lastTeam.Chassis;
                seasonTeam.Reliability = lastTeam.Reliability;
                seasonTeam.EngineId = lastTeam.EngineId;
                seasonTeam.Topspeed = lastTeam.Topspeed;
                seasonTeam.Acceleration = lastTeam.Acceleration;
                seasonTeam.Handling = lastTeam.Handling;
            }

            return View("AddOrUpdateTeam", seasonTeam);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Teams/Add/{globalTeamId}")]
        public async Task<IActionResult> AddTeam(int id, int? globalTeamId, [Bind] SeasonTeam seasonTeam)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId);

            if (season is null || globalTeam is null || seasonTeam is null)
                return NotFound();

            if (season.Teams.Select(d => d.TeamId).Contains(seasonTeam.TeamId))
                return UnprocessableEntity();

            if (ModelState.IsValid)
            {
                // Set the Season and global Driver again as these are not bound in the view.
                seasonTeam.SeasonId = id;
                seasonTeam.TeamId = globalTeamId ?? throw new ArgumentNullException(nameof(globalTeamId));

                // Persist the new SeasonDriver and return to AddDrivers page.
                await _context.AddAsync(seasonTeam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddTeams), new { id });
            }
            else
            {
                var engines = await _context.Engines
                    .Where(e => e.Archived == false)
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync();

                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
                return View("AddOrUpdateTeam", seasonTeam);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var team = season.Teams.SingleOrDefault(t => t.SeasonTeamId == teamId);

            if (season is null || team is null)
                return NotFound();

            var engines = _context.Engines
                .Where(e => e.Archived == false)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            ViewBag.seasonId = id;
            return View("AddOrUpdateTeam", team);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId, [Bind] SeasonTeam updatedTeam)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var team = season.Teams.SingleOrDefault(d => d.SeasonTeamId == teamId);

            if (season is null || team is null || updatedTeam is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                team.Name = updatedTeam.Name;
                team.Principal = updatedTeam.Principal;
                team.Colour = updatedTeam.Colour;
                team.Accent = updatedTeam.Accent;
                team.Chassis = updatedTeam.Chassis;
                team.Topspeed = updatedTeam.Topspeed;
                team.Acceleration = updatedTeam.Acceleration;
                team.Handling = updatedTeam.Handling;
                team.Reliability = updatedTeam.Reliability;
                team.EngineId = updatedTeam.EngineId;
                _context.Update(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var engines = await _context.Engines
                    .Where(e => e.Archived == false)
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync();

                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
                return View("AddOrUpdateDriver", team);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Drivers/Add")]
        public async Task<IActionResult> AddDrivers(int? id)
        {
            var seasons = await _context.Seasons
                .Where(s => s.State == SeasonState.Draft || s.State == SeasonState.Progress)
                .Include(s => s.Drivers)
                .ToListAsync();

            if (seasons is null)
                return NotFound();

            List<int> existingDriverIds = new List<int>();
            foreach (var season in seasons)
            {
                existingDriverIds.AddRange(season.Drivers.Select(d => d.DriverId));
            }
            var unregisteredDrivers = await _context.Drivers
                .Where(d => d.Archived == false)
                .Where(d => !existingDriverIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.seasonId = id;
            var result = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            ViewBag.year = result.SeasonNumber;
            return View(unregisteredDrivers);
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int id, int globalDriverId)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId);

            if (season is null || globalDriver is null)
                return NotFound();

            var teams = season.Teams
                .Select(t => new { t.SeasonTeamId, t.Name })
                .ToList();

            ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
            ViewBag.seasonId = id;

            var seasonDriver = new SeasonDriver
            {
                Driver = globalDriver,
                Season = season
            };

            // Adds last previous used values from driver as default
            var allSeasonDrivers = await _context.SeasonDrivers.ToListAsync();
            var lastDriver = allSeasonDrivers
                .LastOrDefault(ld => ld.DriverId == globalDriverId);

            if (lastDriver != null)
            {
                seasonDriver.Skill = lastDriver.Skill;
                seasonDriver.Reliability = lastDriver.Reliability;
                seasonDriver.DriverStatus = lastDriver.DriverStatus;
            }

            return View("AddOrUpdateDriver", seasonDriver);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int id, int? globalDriverId, [Bind] SeasonDriver seasonDriver)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId);

            if (season is null || globalDriver is null || seasonDriver is null)
                return NotFound();

            if (season.Drivers.Select(d => d.DriverId).Contains(seasonDriver.DriverId))
                return UnprocessableEntity();

            if (ModelState.IsValid)
            {
                // Set the Season and global Driver again as these are not bound in the view.
                seasonDriver.SeasonId = id;
                seasonDriver.DriverId = globalDriverId ?? throw new ArgumentNullException(nameof(globalDriverId));

                // Persist the new SeasonDriver and return to AddDrivers page.
                await _context.AddAsync(seasonDriver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AddDrivers), new { id });
            }
            else
            {
                var teams = season.Teams
                    .Select(t => new { t.SeasonTeamId, t.Name })
                    .ToList();

                ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
                return View("AddOrUpdateDriver", seasonDriver);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Drivers/Update/{driverId}")]
        public async Task<IActionResult> UpdateDriver(int id, int? driverId)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(sd => sd.Driver)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season is null || driver is null)
                return NotFound();

            var teams = season.Teams
                .Select(t => new { t.SeasonTeamId, t.Name })
                .ToList();

            ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
            ViewBag.seasonId = id;
            return View("AddOrUpdateDriver", driver);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Drivers/Update/{driverId}")]
        public async Task<IActionResult> UpdateDriver(int id, int? driverId, [Bind] SeasonDriver updatedDriver)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var races = await _context.Races
                .AsNoTracking()
                .Where(r => r.SeasonId == id && r.RaceState == RaceState.Concept)
                .Include(r => r.Stints)
                .ToListAsync();

            var driver = await _context.SeasonDrivers
                .Include(sd => sd.Driver)
                .SingleOrDefaultAsync(d => d.SeasonDriverId == driverId);

            if (season is null || driver is null || updatedDriver is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                driver.SeasonTeamId = updatedDriver.SeasonTeamId;
                driver.Reliability = updatedDriver.Reliability;
                driver.Skill = updatedDriver.Skill;
                driver.DriverStatus = updatedDriver.DriverStatus;
                if (driver.Dropped)
                    driver.Dropped = false;
                _context.Update(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var teams = season.Teams
                    .Select(t => new { t.SeasonTeamId, t.Name })
                    .ToList();

                ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
                return View("AddOrUpdateDriver", driver);
            }
        }

        [Route("[Controller]/{id}/Driver/Penaltylist/")]
        public async Task<IActionResult> PenaltyList(int id)
        {
            var drivers = await _context.SeasonDrivers
                .Where(s => s.SeasonId == id)
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)               
                .OrderBy(s => s.SeasonTeam.Name)
                .ToListAsync();

            return View(drivers);
        }

        public async Task<IActionResult> DriverDev(int id)
        {
            DriverDevModel viewmodel = new DriverDevModel();
            var season = await _context.Seasons.FirstAsync(s => s.SeasonId == id);
            var championship = await _context.Championships
                .Include(c => c.AgeDevRanges)
                .Include(c => c.SkillDevRanges)
                .SingleOrDefaultAsync(c => c.ActiveChampionship);

            if (season is null || championship is null)
                return NotFound();

            viewmodel.SeasonId = season.SeasonId;
            viewmodel.Year = season.SeasonNumber;
            foreach (var skillrange in championship.SkillDevRanges)
                viewmodel.SkillDevRanges.Add(skillrange);
            foreach (var agerange in championship.AgeDevRanges)
                viewmodel.AgeDevRanges.Add(agerange);

            viewmodel.SeasonDrivers = await _context.SeasonDrivers
                .Where(s => s.SeasonId == id && s.Dropped == false)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Name)
                .ToListAsync();

            return View(viewmodel);
        }

        private int GetCurrentYear(int seasonId)
        {
            var season = _context.Seasons.FirstOrDefault(s => s.SeasonId == seasonId);
            return season.SeasonNumber;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveDriverDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = await _context.Seasons
                .FirstOrDefaultAsync(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship);

            var drivers = await _context.SeasonDrivers
                .Where(s => s.SeasonId == seasonId.SeasonId && s.Dropped == false)
                .ToListAsync();

            if (drivers is null || dev is null)
                return NotFound();

            foreach (var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Skill = driverdev.Newdev;
            }
            _context.UpdateRange(drivers);
            await _context.SaveChangesAsync();
            return RedirectToAction("DriverDev", new { id = seasonId.SeasonId });
        }

        public async Task<IActionResult> TeamDev(int id)
        {
            ViewBag.seasonId = id;

            return View(await _context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveTeamDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = await _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefaultAsync();

            var teams = await _context.SeasonTeams
                .Where(s => s.SeasonId == seasonId.SeasonId)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync();

            if (teams is null || dev is null)
                return NotFound();

            foreach (var teamdev in dev)
            {
                var team = teams.First(t => t.SeasonTeamId == teamdev.Id);
                team.Chassis = teamdev.Newdev;
            }
            _context.UpdateRange(teams);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeamDev", new { id = seasonId.SeasonId });
        }

        public IActionResult EngineDev(int id)
        {
            ViewBag.seasonId = id;

            var teams = _context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(st => st.Engine)
                .ToList();

            var engines = teams
                .GroupBy(e => e.Engine)
                .Select(e => e.First())
                .Select(e => e.Engine)
                .ToList();

            return View(engines);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveEngineDev([FromBody]IEnumerable<GetDev> dev)
        {
            var engines = await _context.Engines.ToListAsync();
            foreach (var enginedev in dev)
            {
                var engine = engines.First(e => e.Id == enginedev.Id);
                engine.Power = enginedev.Newdev;
            }
            _context.UpdateRange(engines);
            await _context.SaveChangesAsync();

            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefault();

            return RedirectToAction("EngineDev", new { id = seasonId.SeasonId });
        }

        public IActionResult TeamReliabilityDev(int id)
        {
            ViewBag.seasonId = id;

            return View(_context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Abbreviation)
                .ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveTeamReliabilityDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefault();

            var teams = await _context.SeasonTeams
                .Where(s => s.SeasonId == seasonId.SeasonId)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync();

            foreach (var teamdev in dev)
            {
                var team = teams.First(t => t.SeasonTeamId == teamdev.Id);
                team.Reliability = teamdev.Newdev;
            }
            _context.UpdateRange(teams);
            await _context.SaveChangesAsync();

            return RedirectToAction("TeamReliabilityDev", new { id = seasonId.SeasonId });
        }

        public IActionResult DriverReliabilityDev(int id)
        {
            ViewBag.seasonId = id;
            ViewBag.year = GetCurrentYear(id);

            return View(_context.SeasonDrivers
                .Where(s => s.SeasonId == id && s.Dropped == false)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Abbreviation)
                .ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveDriverReliabilityDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = await _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefaultAsync();

            if (dev is null)
                return RedirectToAction("DriverReliabilityDev", new { id = seasonId.SeasonId });

            var drivers = await _context.SeasonDrivers
                .Where(s => s.SeasonId == seasonId.SeasonId && s.Dropped == false)
                .ToListAsync();

            foreach (var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Reliability = driverdev.Newdev;
            }
            _context.UpdateRange(drivers);
            await _context.SaveChangesAsync();

            return RedirectToAction("DriverReliabilityDev", new { id = seasonId.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("Driver/Drop/{driverId}")]
        public async Task<IActionResult> DropDriverFromTeam(int seasonId, int driverId)
        {
            var driver = await _context.SeasonDrivers
                .SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverId);

            if (driver is null)
                return NotFound();

            driver.Dropped = true;
            _context.Update(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        public async Task<IActionResult> QualifyingBattle(int seasonId)
        {
            var races = await _context.Races
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId)
                .Include(r => r.DriverResults)
                .ToListAsync();

            var drivers = await _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId)
                .Select(sd => sd.SeasonDriverId)
                .ToListAsync();

            var teams = await _context.SeasonTeams
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId)
                .Include(st => st.SeasonDrivers)
                    .ThenInclude(sd => sd.Driver)
                .Include(st => st.Team)
                .ToListAsync();

            Dictionary<int, int> Battles = new Dictionary<int, int>();
            foreach (var driver in drivers)
                Battles.Add(driver, 0);

            foreach (var race in races)
            {
                foreach (var team in teams)
                {
                    var qualyWinner = race.DriverResults
                        .Where(dr => team.SeasonDrivers.Contains(dr.SeasonDriver))
                        .OrderBy(o => o.Grid)
                        .FirstOrDefault();

                    if (qualyWinner is null)
                        continue;

                    Battles[qualyWinner.SeasonDriverId] += 1;
                }
            }
            return View(new QualifyingBattleModel { QualyBattles = Battles, Teams = teams });
        }
    }

    public class GetDev
    {
        public int Id { get; set; }
        public int Newdev { get; set; }
    }
}
