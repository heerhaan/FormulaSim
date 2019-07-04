﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : Controller
    {
        private readonly FormulaContext _context;
        private static readonly Random rng = new Random();

        public SeasonController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var seasons = _context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .ToList();
            return View(seasons);
        }

        public async Task<IActionResult> Create()
        {
            var season = new Season();
            await _context.AddAsync(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
        }

        public async Task<IActionResult> Start(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            if (season == null)
                return NotFound();

            season.SeasonStart = DateTime.Now;
            season.State = SeasonState.Progress;
            if (season.PointsPerPosition == null)
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
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Finish(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
            if (season == null)
                return NotFound();

            season.State = SeasonState.Finished;
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await _context.Seasons
                .Include(s => s.Races)
                    .ThenInclude(r => r.Track)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Engine)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            return View(nameof(Detail), season);
        }

        // Page that displays certain statistics related to the selected season
        public async Task<IActionResult> SeasonStats(int? id)
        {
            var season = await _context.Seasons
                   .Include(s => s.Races)
                   .SingleOrDefaultAsync(s => s.SeasonId == id);

            ViewBag.seasonId = id;

            if (season == null)
                return NotFound();

            ViewBag.points = season.PointsPerPosition.Values.ToList();

            var seasondrivers = _context.SeasonDrivers
                .Where(sd => sd.SeasonId == id)
                .Include(sd => sd.Driver)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Team)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Engine)
                .OrderByDescending(sd => (sd.Skill + sd.SeasonTeam.Chassis + sd.SeasonTeam.Engine.Power + (2 - (2 * (int)sd.Style)) + (((int)sd.DriverStatus) * -2) + 2))
                .ToList();

            return View(seasondrivers);
        }


        [Route("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            return View(new SeasonSettingsViewModel(season));
        }

        [HttpPost("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id, [Bind] SeasonSettingsViewModel settingsModel)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                season.QualificationRemainingDriversQ2 = settingsModel.QualificationRemainingDriversQ2;
                season.QualificationRemainingDriversQ3 = settingsModel.QualificationRemainingDriversQ3;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
            }

            return View(settingsModel);
        }

        public IActionResult SetPoints(int id)
        {
            var model = new SetPointsModel
            {
                SeasonId = id
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SetPoints(SetPointsModel model)
        {
            if (model == null)
                return NotFound();

            var season = _context.Seasons.Single(s => s.SeasonId == model.SeasonId);
            Dictionary<int, int?> pairs = new Dictionary<int, int?>();
            int position = 1;
            foreach (var points in model.Points)
            {
                pairs.Add(position, points);
                position++;
            }

            season.PointsPerPosition = pairs;
            _context.SaveChanges();
            return RedirectToAction(nameof(Settings), new { id = model.SeasonId });
        }

        [Route("[Controller]/{id}/Teams/Add")]
        public async Task<IActionResult> AddTeams(int? id)
        {
            var season = await _context.Seasons
                    .Include(s => s.Teams)
                    .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTeamIds = season.Teams.Select(t => t.TeamId);
            var unregisteredTeams = _context.Teams
                .Where(t => t.Archived == false)
                .Where(t => !existingTeamIds.Contains(t.TeamId)).ToList();

            ViewBag.seasonId = id;
            return View(unregisteredTeams);
        }

        [Route("[Controller]/{id}/Teams/Add/{globalTeamId}")]
        public async Task<IActionResult> AddTeam(int? id, int? globalTeamId)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.TeamId == globalTeamId);

            if (season == null || globalTeam == null)
                return NotFound();

            var engines = _context.Engines.Where(e => e.Available).Select(t => new { t.EngineId, t.Name });
            ViewBag.engines = new SelectList(engines, nameof(Engine.EngineId), nameof(Engine.Name));
            ViewBag.seasonId = id;

            var seasonTeam = new SeasonTeam
            {
                Team = globalTeam,
                Season = season
            };

            // Adds last previous used values from team as default
            var lastTeam = _context.SeasonTeams.LastOrDefault(s => s.Team.TeamId == globalTeamId);
            if (lastTeam != null)
            {
                seasonTeam.Principal = lastTeam.Principal;
                seasonTeam.Chassis = lastTeam.Chassis;
                seasonTeam.Reliability = lastTeam.Reliability;
                seasonTeam.EngineId = lastTeam.EngineId;
                seasonTeam.Topspeed = lastTeam.Topspeed;
                seasonTeam.Acceleration = lastTeam.Acceleration;
                seasonTeam.Stability = lastTeam.Stability;
                seasonTeam.Handling = lastTeam.Handling;
            }

            return View("AddOrUpdateTeam", seasonTeam);
        }

        [HttpPost("[Controller]/{id}/Teams/Add/{globalTeamId}")]
        public async Task<IActionResult> AddTeam(int id, int? globalTeamId, [Bind] SeasonTeam seasonTeam)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.TeamId == globalTeamId);

            if (season == null || globalTeam == null)
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
                var engines = _context.Engines.Where(e => e.Available).Select(t => new { t.EngineId, t.Name });
                ViewBag.engines = new SelectList(engines, nameof(Engine.EngineId), nameof(Engine.Name));
                return View("AddOrUpdateTeam", seasonTeam);
            }
        }

        [Route("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var team = season.Teams.SingleOrDefault(t => t.SeasonTeamId == teamId);

            if (season == null || team == null)
                return NotFound();

            var engines = _context.Engines.Where(e => e.Available).Select(t => new { t.EngineId, t.Name });
            ViewBag.engines = new SelectList(engines, nameof(Engine.EngineId), nameof(Engine.Name));
            ViewBag.seasonId = id;
            return View("AddOrUpdateTeam", team);
        }

        [HttpPost("[Controller]/{id}/Teams/Update/{teamId}")]
        public async Task<IActionResult> UpdateTeam(int id, int? teamId, [Bind] SeasonTeam updatedTeam)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var team = season.Teams.SingleOrDefault(d => d.SeasonTeamId == teamId);

            if (season == null || team == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                team.Principal = updatedTeam.Principal;
                team.Chassis = updatedTeam.Chassis;
                team.Topspeed = updatedTeam.Topspeed;
                team.Acceleration = updatedTeam.Acceleration;
                team.Stability = updatedTeam.Stability;
                team.Handling = updatedTeam.Handling;
                team.Reliability = updatedTeam.Reliability;
                team.EngineId = updatedTeam.EngineId;
                _context.Update(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var engines = _context.Engines.Where(e => e.Available).Select(t => new { t.EngineId, t.Name });
                ViewBag.engines = new SelectList(engines, nameof(Engine.EngineId), nameof(Engine.Name));
                return View("AddOrUpdateDriver", team);
            }
        }

        [Route("[Controller]/{id}/Drivers/Add")]
        public async Task<IActionResult> AddDrivers(int? id)
        {
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTrackIds = season.Drivers.Select(d => d.DriverId);
            var unregisteredDrivers = _context.Drivers
                .Where(d => d.Archived == false)
                .Where(d => !existingTrackIds.Contains(d.DriverId)).ToList();

            ViewBag.seasonId = id;
            return View(unregisteredDrivers);
        }

        [Route("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int? id, int? globalDriverId)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.DriverId == globalDriverId);

            if (season == null || globalDriver == null)
                return NotFound();

            var teams = season.Teams
                .Select(t => new { t.SeasonTeamId, t.Team.Name });
            ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Team.Name));
            ViewBag.seasonId = id;

            var seasonDriver = new SeasonDriver
            {
                Driver = globalDriver,
                Season = season
            };

            // Adds last previous used values from driver as default
            var lastDriver = _context.SeasonDrivers
                .LastOrDefault(s => s.Driver.DriverId == globalDriverId);
            if (lastDriver != null)
            {
                seasonDriver.Skill = lastDriver.Skill;
                seasonDriver.Style = lastDriver.Style;
                seasonDriver.Tires = lastDriver.Tires;
            }

            return View("AddOrUpdateDriver", seasonDriver);
        }

        [HttpPost("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int id, int? globalDriverId, [Bind] SeasonDriver seasonDriver)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.DriverId == globalDriverId);

            if (season == null || globalDriver == null)
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
                var teams = season.Teams.Select(t => new { t.SeasonTeamId, t.Team.Name });
                ViewBag.teams = new SelectList(teams, "SeasonTeamId", "Name");
                return View("AddOrUpdateDriver", seasonDriver);
            }
        }

        [Route("[Controller]/{id}/Drivers/Update/{driverId}")]
        public async Task<IActionResult> UpdateDriver(int id, int? driverId)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season == null || driver == null)
                return NotFound();

            var teams = season.Teams.Select(t => new { t.SeasonTeamId, t.Team.Name });
            ViewBag.teams = new SelectList(teams, "SeasonTeamId", "Name");
            ViewBag.seasonId = id;
            return View("AddOrUpdateDriver", driver);
        }

        [HttpPost("[Controller]/{id}/Drivers/Update/{driverId}")]
        public async Task<IActionResult> UpdateDriver(int id, int? driverId, [Bind] SeasonDriver updatedDriver)
        {
            // Get and validate URL parameter objects.
            var season = await _context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season == null || driver == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                driver.SeasonTeamId = updatedDriver.SeasonTeamId;
                driver.Skill = updatedDriver.Skill;
                driver.Tires = updatedDriver.Tires;
                driver.Style = updatedDriver.Style;
                driver.DriverStatus = updatedDriver.DriverStatus;
                _context.Update(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var teams = season.Teams.Select(t => new { t.SeasonTeamId, t.Team.Name });
                ViewBag.teams = new SelectList(teams, "SeasonTeamId", "Name");
                return View("AddOrUpdateDriver", driver);
            }
        }

        [Route("[Controller]/{id}/Driver/Penalty/{driverId}")]
        public IActionResult PenaltyList(int id, int driverId)
        {
            var driver = _context.SeasonDrivers
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)
                .ThenInclude(t => t.Team)
                .SingleOrDefault(s => s.SeasonId == id && s.SeasonDriverId == driverId);
            return View(driver);
        }

        public IActionResult DriverDev(int id)
        {
            ViewBag.seasonId = id;

            return View(_context.SeasonDrivers
                .Where(s => s.SeasonId == id)
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Name).ToList());
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveDriverDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress)
                .FirstOrDefault();

            var drivers = _context.SeasonDrivers
                .Where(s => s.SeasonId == seasonId.SeasonId);

            foreach (var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Skill = driverdev.Newdev;
            }
            _context.UpdateRange(drivers);
            _context.SaveChanges();

            return RedirectToAction("DriverDev", new { id = seasonId.SeasonId });
        }

        public IActionResult TeamDev(int id)
        {
            ViewBag.seasonId = id;

            return View(_context.SeasonTeams
                .Where(s => s.SeasonId == id)
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Name).ToList());
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveTeamDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress)
                .FirstOrDefault();

            var teams = _context.SeasonTeams
                .Where(s => s.SeasonId == seasonId.SeasonId)
                .OrderBy(t => t.Team.Name);

            foreach (var teamdev in dev)
            {
                var team = teams.First(t => t.SeasonTeamId == teamdev.Id);
                team.Chassis = teamdev.Newdev;
            }
            _context.UpdateRange(teams);
            _context.SaveChanges();

            return RedirectToAction("TeamDev", new { id = seasonId.SeasonId });
        }

        public IActionResult EngineDev(int id)
        {
            ViewBag.seasonId = id;

            return View(_context.Engines.Where(e => e.Available == true).ToList());
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveEngineDev([FromBody]IEnumerable<GetDev> dev)
        {
            var engines = _context.Engines.Where(e => e.Available == true);
            foreach (var enginedev in dev)
            {
                var engine = engines.First(e => e.EngineId == enginedev.Id);
                engine.Power = enginedev.Newdev;
            }
            _context.UpdateRange(engines);
            _context.SaveChanges();

            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress)
                .FirstOrDefault();

            return RedirectToAction("EngineDev", new { id = seasonId.SeasonId });
        }
        
        public class GetDev
        {
            public int Id { get; set; }
            public int Newdev { get; set; }
        }
    }
}