﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
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
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.Builders;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : FormulaController
    {
        private readonly ISeasonService _seasons;
        private readonly ITrackService _tracks;
        private readonly RaceBuilder _raceBuilder;

        public SeasonController(FormulaContext context,
            UserManager<SimUser> userManager,
            ISeasonService seasons,
            ITrackService tracks,
            RaceBuilder raceBuilder)
            : base(context, userManager)
        {
            _seasons = seasons;
            _tracks = tracks;
            _raceBuilder = raceBuilder;
        }

        public async Task<IActionResult> Index()
        {
            var seasons = await Context.Seasons
                .IgnoreQueryFilters()
                .Where(s => s.Championship.ActiveChampionship)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(s => s.Team)
                .OrderByDescending(s => s.SeasonNumber)
                .ToListAsync();

            var championship = Context.Championships.FirstOrDefault(s => s.ActiveChampionship);
            if (championship is null)
                ViewBag.championship = "NaN";
            else
                ViewBag.championship = championship.ChampionshipName;

            var champs = await Context.Championships
                .ToListAsync();

            ViewBag.champs = champs;

            return View(seasons);
        }

        [ActionName("ChampionshipSelect")]
        public async Task<IActionResult> Index(int championshipId)
        {
            var seasons = await Context.Seasons
                .IgnoreQueryFilters()
                .Where(s => s.ChampionshipId == championshipId)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(s => s.Team)
                .OrderByDescending(s => s.SeasonNumber)
                .ToListAsync();

            var championship = Context.Championships.FirstOrDefault(s => s.ChampionshipId == championshipId);
            if (championship is null)
                ViewBag.championship = "NaN";
            else
                ViewBag.championship = championship.ChampionshipName;

            var champs = await Context.Championships
                .ToListAsync();

            ViewBag.champs = champs;

            return View("Index", seasons);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var season = new Season();
            var championship = await Context.Championships.FirstAsync(c => c.ActiveChampionship);
            if (championship is null)
                return NotFound();

            season.Championship = championship;
            await _seasons.Add(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddDefault(int? id)
        {
            // gets the current and previous season in this championship
            var season = await Context.Seasons
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var lastSeason = Context.Seasons
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
                season.PitMax = lastSeason.PitMax;
                season.PitMin = lastSeason.PitMin;

                // Adds the previous season races if there aren't any added yet.
                if (season.Races.Count == 0)
                {
                    foreach (var race in lastSeason.Races.OrderBy(res => res.Round))
                    {
                        var track = await _tracks.GetTrackById(race.TrackId);
                        var newStints = new List<Stint>();
                        foreach (var oldStint in race.Stints.OrderBy(res => res.Number))
                        {
                            var newStint = new Stint
                            {
                                Number = oldStint.Number,
                                ApplyDriverLevel = oldStint.ApplyDriverLevel,
                                ApplyChassisLevel = oldStint.ApplyChassisLevel,
                                ApplyEngineLevel = oldStint.ApplyEngineLevel,
                                ApplyQualifyingBonus = oldStint.ApplyQualifyingBonus,
                                ApplyReliability = oldStint.ApplyReliability,
                                RNGMaximum = oldStint.RNGMaximum,
                                RNGMinimum = oldStint.RNGMinimum
                            };
                            newStints.Add(newStint);
                        }
                        var newRace = _raceBuilder
                            .InitializeRace(track, season)
                            .AddModifiedStints(newStints)
                            .GetResultAndRefresh();

                        season.Races.Add(newRace);
                    }
                }
                _seasons.Update(season);
                await _seasons.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Start(int? id)
        {
            var season = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            if (season is null)
                return NotFound();

            season.SeasonStart = DateTime.Now;
            season.State = SeasonState.Progress;
            if (season.PointsPerPosition.Count == 0)
            {
                // Default assigned points per position
                season.PointsPerPosition.Add(1, 25);
                season.PointsPerPosition.Add(2, 18);
                season.PointsPerPosition.Add(3, 15);
                season.PointsPerPosition.Add(4, 12);
                season.PointsPerPosition.Add(5, 10);
                season.PointsPerPosition.Add(6, 8);
                season.PointsPerPosition.Add(7, 6);
                season.PointsPerPosition.Add(8, 5);
                season.PointsPerPosition.Add(9, 4);
                season.PointsPerPosition.Add(10, 3);
                season.PointsPerPosition.Add(11, 2);
                season.PointsPerPosition.Add(12, 1);
            }
            Context.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Finish(int? id)
        {
            var season = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            if (season is null)
                return NotFound();

            season.State = SeasonState.Finished;
            Context.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await Context.Seasons
                .IgnoreQueryFilters()
                .Include(s => s.Races)
                    .ThenInclude(r => r.Track)
                .Include(s => s.Races)
                    .ThenInclude(r => r.Stints)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var drivers = await Context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(sd => sd.Driver)
                .Where(sd => sd.SeasonId == id)
                .ToListAsync();

            var teams = await Context.SeasonTeams
                .IgnoreQueryFilters()
                .Include(t => t.Team)
                .Include(t => t.Engine)
                .Include(t => t.Rubber)
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
            var race = await Context.Races.SingleOrDefaultAsync(s => s.RaceId == id);
            var season = await Context.Seasons.Include(s => s.Races).SingleOrDefaultAsync(s => s.SeasonId == seasonId);
            if (race == null)
                return NotFound();

            var stints = await Context.Stints.Where(s => s.RaceId == race.RaceId).ToListAsync();
            season.Races.Remove(race);
            int round = 0;
            foreach (var seasonRace in season.Races.OrderBy(r => r.Round))
            {
                round++;
                seasonRace.Round = round;
            }

            Context.RemoveRange(stints);
            Context.Remove(race);
            Context.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        public async Task<IActionResult> SeasonStats(int? id)
        {
            var season = await Context.Seasons
                .AsNoTracking()
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season is null)
                return NotFound();

            ViewBag.seasonId = id;
            ViewBag.number = season.SeasonNumber;

            var seasondrivers = Context.SeasonDrivers
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
        public async Task<IActionResult> Settings(int id, string statusmessage = null)
        {
            var season = await Context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season is null)
                return NotFound();

            ViewBag.statusmessage = statusmessage;
            return View(new SeasonSettingsViewModel(season));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id, [Bind] SeasonSettingsViewModel settingsModel)
        {
            var season = await Context.Seasons
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
                await _seasons.SaveChangesAsync();
                return RedirectToAction(nameof(Settings), new { id = season.SeasonId, statusmessage = "Settings succesfully saved" });
            }

            return View(settingsModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPoints(int id)
        {
            var season = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            var model = new SeasonSetPointsModel
            {
                SeasonId = id,
                SeasonNumber = season.SeasonNumber
            };
            foreach (var value in season.PointsPerPosition.Values)
                model.Points.Add(value.Value);

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetPoints(SeasonSetPointsModel model)
        {
            if (model is null)
                return NotFound();

            var season = await Context.Seasons.SingleAsync(s => s.SeasonId == model.SeasonId);
            // Clear the dictionary if it contains any values
            if (season.PointsPerPosition.Count > 0)
                season.PointsPerPosition.Clear();

            Dictionary<int, int?> pairs = new Dictionary<int, int?>();
            int position = 1;
            foreach (var points in model.Points)
            {
                pairs.Add(position, points);
                position++;
            }

            foreach (var pair in pairs)
                season.PointsPerPosition.Add(pair);

            _seasons.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Settings), new { id = model.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Teams/Add")]
        public async Task<IActionResult> AddTeams(int? id)
        {
            var seasons = await Context.Seasons
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
            var unregisteredTeams = await Context.Teams
                .Where(t => !t.Archived)
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
            var season = await Context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalTeam = await Context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId);

            if (season is null || globalTeam is null)
                return NotFound();

            var engines = await Context.Engines
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();
            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            var rubbers = await Context.Rubbers
                .Select(r => new { r.RubberId, r.Name })
                .ToListAsync();
            ViewBag.rubbers = new SelectList(rubbers, nameof(Rubber.RubberId), nameof(Rubber.Name));
            ViewBag.seasonId = id;

            var seasonTeam = new SeasonTeam
            {
                Team = globalTeam,
                Season = season
            };

            // Adds last previous used values from team as default
            var allSeasonTeams = await Context.SeasonTeams.ToListAsync();
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
                seasonTeam.RubberId = lastTeam.RubberId;
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
            var season = await Context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var globalTeam = await Context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId);

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
                await Context.AddAsync(seasonTeam);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(AddTeams), new { id });
            }
            else
            {
                var engines = await Context.Engines
                    .Where(e => !e.Archived)
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync();
                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
                var rubbers = await Context.Rubbers
                    .Select(r => new { r.RubberId, r.Name })
                    .ToListAsync();
                ViewBag.rubbers = new SelectList(rubbers, nameof(Rubber.RubberId), nameof(Rubber.Name));
                return View("AddOrUpdateTeam", seasonTeam);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId)
        {
            var season = await Context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var team = season.Teams.SingleOrDefault(t => t.SeasonTeamId == teamId);

            if (season is null || team is null)
                return NotFound();

            var engines = Context.Engines
                .Where(e => !e.Archived)
                .Select(t => new { t.Id, t.Name })
                .ToList();

            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            var rubbers = await Context.Rubbers
                    .Select(r => new { r.RubberId, r.Name })
                    .ToListAsync();
            ViewBag.rubbers = new SelectList(rubbers, nameof(Rubber.RubberId), nameof(Rubber.Name));
            ViewBag.seasonId = id;
            return View("AddOrUpdateTeam", team);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId, [Bind] SeasonTeam updatedTeam)
        {
            var season = await Context.Seasons
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
                team.RubberId = updatedTeam.RubberId;
                Context.Update(team);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var engines = await Context.Engines
                    .Where(e => !e.Archived)
                    .Select(t => new { t.Id, t.Name })
                    .ToListAsync();

                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
                var rubbers = await Context.Rubbers
                    .Select(r => new { r.RubberId, r.Name })
                    .ToListAsync();
                ViewBag.rubbers = new SelectList(rubbers, nameof(Rubber.RubberId), nameof(Rubber.Name));
                return View("AddOrUpdateDriver", team);
            }
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Drivers/Add")]
        public async Task<IActionResult> AddDrivers(int? id)
        {
            var seasons = await Context.Seasons
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
            var unregisteredDrivers = await Context.Drivers
                .Where(d => !d.Archived)
                .Where(d => !existingDriverIds.Contains(d.Id))
                .OrderBy(d => d.Name)
                .ToListAsync();

            ViewBag.seasonId = id;
            var result = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            ViewBag.year = result.SeasonNumber;
            return View(unregisteredDrivers);
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int id, int globalDriverId)
        {
            var season = await Context.Seasons
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await Context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId);

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
            var allSeasonDrivers = await Context.SeasonDrivers.ToListAsync();
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
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await Context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId);

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
                await Context.AddAsync(seasonDriver);
                await Context.SaveChangesAsync();
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
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(s => s.Driver)
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
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            var driver = season.Drivers
                .SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season is null || driver is null || updatedDriver is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                driver.SeasonTeamId = updatedDriver.SeasonTeamId;
                driver.Reliability = updatedDriver.Reliability;
                driver.Skill = updatedDriver.Skill;
                driver.DriverStatus = updatedDriver.DriverStatus;
                driver.Dropped = false;
                Context.Update(driver);
                await Context.SaveChangesAsync();
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
            var drivers = await Context.SeasonDrivers
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)
                .Where(s => s.SeasonId == id)
                .OrderBy(s => s.SeasonTeam.Name)
                .ToListAsync();

            return View(drivers);
        }

        public async Task<IActionResult> DriverDev(int id)
        {
            DriverDevModel viewmodel = new DriverDevModel();
            var season = await Context.Seasons.FirstAsync(s => s.SeasonId == id);
            var championship = await Context.Championships
                .Include(c => c.AgeDevRanges)
                .Include(c => c.SkillDevRanges)
                .SingleOrDefaultAsync(c => c.ActiveChampionship);

            if (season is null || championship is null)
                return NotFound();

            viewmodel.SeasonId = season.SeasonId;
            viewmodel.Year = season.SeasonNumber;
            foreach (var skillrange in championship.SkillDevRanges.OrderBy(res => res.ValueKey))
                viewmodel.SkillDevRanges.Add(skillrange);
            foreach (var agerange in championship.AgeDevRanges.OrderBy(res => res.ValueKey))
                viewmodel.AgeDevRanges.Add(agerange);

            viewmodel.SeasonDrivers = await Context.SeasonDrivers
                .Where(s => s.SeasonId == id && !s.Dropped)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Abbreviation)
                .ToListAsync();

            return View(viewmodel);
        }

        private int GetCurrentYear(int seasonId)
        {
            var season = Context.Seasons.FirstOrDefault(s => s.SeasonId == seasonId);
            return season.SeasonNumber;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveDriverDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = await Context.Seasons
                .FirstOrDefaultAsync(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship);

            var drivers = await Context.SeasonDrivers
                .Where(s => s.SeasonId == seasonId.SeasonId && !s.Dropped)
                .ToListAsync();

            if (drivers is null || dev is null)
                return NotFound();

            foreach (var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Skill = driverdev.Newdev;
            }
            Context.UpdateRange(drivers);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(DriverDev), new { id = seasonId.SeasonId });
        }

        public async Task<IActionResult> TeamDev(int id)
        {
            ViewBag.seasonId = id;

            return View(await Context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveTeamDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = await Context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefaultAsync();

            var teams = await Context.SeasonTeams
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
            Context.UpdateRange(teams);
            await Context.SaveChangesAsync();

            return RedirectToAction(nameof(TeamDev), new { id = seasonId.SeasonId });
        }

        public async Task<IActionResult> EngineDev(int id)
        {
            ViewBag.seasonId = id;

            var teams = await Context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(st => st.Engine)
                .ToListAsync();

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
            if (dev is null) { return NotFound(); }

            var engines = await Context.Engines.ToListAsync();
            foreach (var enginedev in dev)
            {
                var engine = engines.First(e => e.Id == enginedev.Id);
                engine.Power = enginedev.Newdev;
            }
            Context.UpdateRange(engines);
            await Context.SaveChangesAsync();

            var seasonId = Context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefault();

            return RedirectToAction(nameof(EngineDev), new { id = seasonId.SeasonId });
        }

        public async Task<IActionResult> TeamReliabilityDev(int id)
        {
            ViewBag.seasonId = id;

            return View(await Context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveTeamReliabilityDev([FromBody]IEnumerable<GetDev> dev)
        {
            if (dev is null) { return NotFound(); }

            var seasonId = Context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefault();

            var teams = await Context.SeasonTeams
                .Where(s => s.SeasonId == seasonId.SeasonId)
                .OrderBy(t => t.Team.Abbreviation)
                .ToListAsync();

            foreach (var teamdev in dev)
            {
                var team = teams.First(t => t.SeasonTeamId == teamdev.Id);
                team.Reliability = teamdev.Newdev;
            }
            Context.UpdateRange(teams);
            await Context.SaveChangesAsync();

            return RedirectToAction(nameof(TeamReliabilityDev), new { id = seasonId.SeasonId });
        }

        public async Task<IActionResult> DriverReliabilityDev(int id)
        {
            ViewBag.seasonId = id;
            ViewBag.year = GetCurrentYear(id);

            return View(await Context.SeasonDrivers
                .Where(s => s.SeasonId == id && !s.Dropped)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Abbreviation)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveDriverReliabilityDev([FromBody]IEnumerable<GetDev> dev)
        {
            if (dev is null) { return NotFound(); }

            var seasonId = await Context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefaultAsync();

            var drivers = await Context.SeasonDrivers
                .Where(s => s.SeasonId == seasonId.SeasonId && !s.Dropped)
                .ToListAsync();

            foreach (var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Reliability = driverdev.Newdev;
            }
            Context.UpdateRange(drivers);
            await Context.SaveChangesAsync();

            return RedirectToAction(nameof(DriverReliabilityDev), new { id = seasonId.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("Driver/Drop/{driverId}")]
        public async Task<IActionResult> DropDriverFromTeam(int seasonId, int driverId)
        {
            var driver = await Context.SeasonDrivers.SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverId);
            if (driver is null)
                return NotFound();

            driver.Dropped = true;
            Context.Update(driver);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        public async Task<IActionResult> QualifyingBattle(int seasonId)
        {
            var races = await Context.Races
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId && r.RaceState == RaceState.Finished)
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                .ToListAsync();

            var drivers = await Context.SeasonDrivers
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonId)
                .Select(sd => sd.SeasonDriverId)
                .ToListAsync();

            var teams = await Context.SeasonTeams
                .AsNoTracking()
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
                    var teamDriverIds = team.SeasonDrivers.Select(res => res.SeasonDriverId);
                    var qualyWinner = race.DriverResults
                        .Where(dr => teamDriverIds.Contains(dr.SeasonDriverId))
                        .OrderBy(o => o.Grid)
                        .FirstOrDefault();

                    if (qualyWinner is null)
                        continue;

                    Battles[qualyWinner.SeasonDriverId]++;
                }
            }
            return View(new QualifyingBattleModel { QualyBattles = Battles, Teams = teams, SeasonId = seasonId });
        }
    }

    public class GetDev
    {
        public int Id { get; set; }
        public int Newdev { get; set; }
    }
}
