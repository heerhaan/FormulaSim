using FormuleCirkelEntity.DAL;
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

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : FormulaController
    {
        public SeasonController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager)
            : base(context, identityContext, userManager)
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
                .Include(s => s.Championship)
                .ToList()
                .LastOrDefault(s => s.State == SeasonState.Finished && s.ChampionshipId == season.ChampionshipId);

            if (lastSeason != null)
            {
                season.PointsPerPosition = lastSeason.PointsPerPosition;
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
                            Stints = race.Stints,
                            TrackId = race.TrackId,
                            Weather = Helpers.RandomWeather(),
                            RaceState = RaceState.Concept
                        };
                        season.Races.Add(newRace);
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Detail), new { id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Start(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id);
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
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
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

            season.Races.Remove(race);
            int round = 0;
            foreach (var seasonRace in season.Races.OrderBy(r => r.Round))
            {
                round++;
                seasonRace.Round = round;
            }

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

            seasondrivers = AddTraitReliabilityEffects(seasondrivers);

            return View(seasondrivers);
        }

        // Adds the effects of a trait to the total driver and chassis reliablity to all the drivers.
        private static List<SeasonDriver> AddTraitReliabilityEffects(List<SeasonDriver> drivers)
        {
            foreach (var driver in drivers)
            {
                // This loop looks over all the traits a driver has.
                foreach (var trait in driver.Traits.Values)
                {
                    if (trait.DriverReliability.HasValue)
                        driver.Reliability += trait.DriverReliability.Value;
                }
                // This loop looks over all the traits a team has.
                foreach (var trait in driver.SeasonTeam.Traits.Values)
                {
                    if (trait.DriverReliability.HasValue)
                        driver.Reliability += trait.DriverReliability.Value;
                }
            }

            return drivers;
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

            season.PointsPerPosition = pairs;
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

        // Page underneath is slow
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
                .Where(e => e.Archived == false)
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
            var lastTeam = _context.SeasonTeams
                .Include(st => st.Team)
                .ToList()
                .OrderBy(st => st.SeasonTeamId)
                .LastOrDefault(s => s.Team.Id == globalTeamId);

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

        // Page underneath is slow
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

                // Adds last previous used traits from team as default
                var lastTeam = _context.SeasonTeams
                    .Include(st => st.Team)
                    .ToList()
                    .OrderBy(st => st.SeasonTeamId)
                    .LastOrDefault(s => s.Team.Id == globalTeamId);

                if (lastTeam != null)
                {
                    seasonTeam.Traits = lastTeam.Traits;
                }

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
        public async Task<IActionResult> AddDriver(int? id, int? globalDriverId)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
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
            var lastDriver = _context.SeasonDrivers
                .AsEnumerable()
                .LastOrDefault(s => s.DriverId == globalDriverId);

            if (lastDriver != null)
            {
                seasonDriver.Skill = lastDriver.Skill;
                seasonDriver.Reliability = lastDriver.Reliability;
                seasonDriver.Tires = lastDriver.Tires;
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
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
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

                // Adds last previous used traits from driver as default
                var lastDriver = _context.SeasonDrivers
                    .AsEnumerable()
                    .LastOrDefault(s => s.DriverId == globalDriverId);

                if (lastDriver != null)
                    seasonDriver.Traits = lastDriver.Traits;

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
                    .ThenInclude(d => d.Driver)
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
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == id);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season is null || driver is null || updatedDriver is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                driver.SeasonTeamId = updatedDriver.SeasonTeamId;
                driver.Reliability = updatedDriver.Reliability;
                driver.Skill = updatedDriver.Skill;
                driver.Tires = updatedDriver.Tires;
                driver.DriverStatus = updatedDriver.DriverStatus;
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
            ViewBag.seasonId = id;
            ViewBag.year = GetCurrentYear(id);

            return View(await _context.SeasonDrivers
                .Where(s => s.SeasonId == id && s.Dropped == false)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Name)
                .ToListAsync());
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
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefaultAsync();

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

        [Route("Driver/Traits/{id}")]
        public async Task<IActionResult> DriverTraits(int id)
        {
            var seasondriver = await _context.SeasonDrivers
                .Include(sd => sd.Driver)
                .SingleOrDefaultAsync(t => t.SeasonDriverId == id);

            var traits = _context.Traits
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Driver && !seasondriver.Traits.Any(res => res.Value.TraitId == tr.TraitId))
                .OrderBy(t => t.Name)
                .ToList();

            if (seasondriver == null)
                return NotFound();

            var model = new SeasonTraitsDriverModel
            {
                Driver = seasondriver,
                Traits = traits
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Driver/Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverTraits(int id, [Bind("TraitId")] int traitId)
        {
            var seasondriver = await _context.SeasonDrivers
                .SingleOrDefaultAsync(t => t.SeasonDriverId == id);

            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (seasondriver == null || trait == null)
                return NotFound();

            seasondriver.Traits.Add(seasondriver.Traits.Count, trait);
            _context.Update(seasondriver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DriverTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Driver/Traits/Remove/{driverId}")]
        public async Task<IActionResult> RemoveDriverTrait(int driverId, int traitId)
        {
            var driver = await _context.SeasonDrivers.SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverId);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (driver == null || trait == null)
                return NotFound();

            var removetrait = driver.Traits.First(item => item.Value.TraitId == trait.TraitId);
            driver.Traits.Remove(removetrait);
            _context.Update(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DriverTraits), new { id = driverId });
        }

        [Route("Team/Traits/{id}")]
        public async Task<IActionResult> TeamTraits(int id)
        {
            var seasonteam = await _context.SeasonTeams
                .Include(st => st.Team)
                .SingleOrDefaultAsync(st => st.SeasonTeamId == id);

            var traits = _context.Traits
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Team && !seasonteam.Traits.Any(res => res.Value.TraitId == tr.TraitId))
                .OrderBy(t => t.Name)
                .ToList();

            if (seasonteam == null)
                return NotFound();

            var model = new SeasonTraitsTeamModel
            {
                Team = seasonteam,
                Traits = traits
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Team/Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeamTraits(int id, [Bind("TraitId")] int traitId)
        {
            var seasonteam = await _context.SeasonTeams.SingleOrDefaultAsync(st => st.SeasonTeamId == id);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (seasonteam == null || trait == null)
                return NotFound();

            seasonteam.Traits.Add(seasonteam.Traits.Count, trait);
            _context.Update(seasonteam);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeamTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Team/Traits/Remove/{teamId}")]
        public async Task<IActionResult> RemoveTeamTrait(int teamId, int traitId)
        {
            var team = await _context.SeasonTeams.SingleOrDefaultAsync(st => st.SeasonTeamId == teamId);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId);

            if (team == null || trait == null)
                return NotFound();

            var removetrait = team.Traits.First(item => item.Value.TraitId == trait.TraitId);
            team.Traits.Remove(removetrait);
            _context.Update(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeamTraits), new { id = teamId });
        }

        [Authorize(Roles = "Admin")]
        [Route("Driver/Drop/{driverId}")]
        public async Task<IActionResult> DropDriverFromTeam(int seasonId, int driverId)
        {
            var driver = await _context.SeasonDrivers.SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverId);
            if (driver is null)
                return NotFound();

            driver.Dropped = true;
            _context.Update(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }
    }

    public class GetDev
    {
        public int Id { get; set; }
        public int Newdev { get; set; }
    }
}