using FormuleCirkelEntity.Builders;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using FormuleCirkelEntity.ViewModels.Season;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : FormulaController
    {
        private readonly ISeasonService _seasons;
        private readonly IChampionshipService _championships;
        private readonly ITrackService _tracks;
        private readonly RaceBuilder _raceBuilder;

        public SeasonController(FormulaContext context,
            UserManager<SimUser> userManager,
            ISeasonService seasons,
            IChampionshipService championships,
            ITrackService tracks,
            RaceBuilder raceBuilder)
            : base(context, userManager)
        {
            _seasons = seasons;
            _championships = championships;
            _tracks = tracks;
            _raceBuilder = raceBuilder;
        }

        public async Task<IActionResult> Index()
        {
            var championship = await _championships.GetActiveChampionship();
            if (championship == null) { return RedirectToAction("Index", "Championship"); }
            else { return await Index(championship.ChampionshipId); }
        }

        [Route("[controller]/[action]/{championshipID}")]
        public async Task<IActionResult> Index(int championshipID)
        {
            var championships = await _championships.GetChampionships();
            var seasonIndex = await _seasons.GetSeasonIndexListOfChampionship(championshipID);
            var viewModel = new SeasonIndexModel()
            {
                ChampionshipID = championshipID,
                ChampionshipName = championships.FirstOrDefault(e => e.ActiveChampionship).ChampionshipName,
                AllChampionships = championships.ToDictionary(e => e.ChampionshipId, e => e.ChampionshipName),
                SeasonIndex = seasonIndex
            };
            return View("Index", viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var season = new Season();
            var championship = await _championships.GetActiveChampionship();
            if (championship is null) { return NotFound(); }

            season.Championship = championship;
            await _seasons.Add(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { seasonID = season.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CopyLast(int championshipID)
        {
            var seasonID = await _seasons.CreateCopyOfLastSeason(championshipID);
            return RedirectToAction(nameof(Detail), new { seasonID });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Start(int seasonID)
        {
            var season = await _seasons.GetSeasonById(seasonID);
            if (season is null) { return NotFound(); }

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
            _seasons.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { seasonID });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Finish(int seasonID)
        {
            var season = await _seasons.GetSeasonById(seasonID);
            if (season is null) { return NotFound(); }

            season.State = SeasonState.Finished;
            _seasons.Update(season);
            await _seasons.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { seasonID });
        }

        public async Task<IActionResult> CurrentDetail()
        {
            var current = await _seasons.FindActiveSeason();
            return RedirectToAction(nameof(Detail), new { seasonID = current.SeasonId });
        }
        public async Task<IActionResult> Detail(int seasonID)
        {
            var season = await Context.Seasons
                .IgnoreQueryFilters()
                .Include(s => s.Races)
                    .ThenInclude(r => r.Track)
                .Include(s => s.Races)
                    .ThenInclude(r => r.Stints)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);

            var drivers = await Context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(sd => sd.Driver)
                .Where(sd => sd.SeasonId == seasonID)
                .ToListAsync();

            var teams = await Context.SeasonTeams
                .IgnoreQueryFilters()
                .Include(t => t.Team)
                .Include(t => t.Engine)
                .Include(t => t.Rubber)
                .Where(st => st.SeasonId == seasonID)
                .ToListAsync();

            if (season is null) { return NotFound(); }

            var seasonmodel = new SeasonDetailModel
            {
                Season = season,
                SeasonDrivers = drivers,
                SeasonTeams = teams
            };
            return View(nameof(Detail), seasonmodel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRace(int raceID, int seasonID)
        {
            var race = await Context.Races.SingleOrDefaultAsync(s => s.RaceId == raceID);
            var season = await Context.Seasons.Include(s => s.Races).SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            if (race is null) { return NotFound(); }

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
            return RedirectToAction(nameof(Detail), new { id = seasonID });
        }

        public async Task<IActionResult> SeasonStats(int seasonID)
        {
            var season = await Context.Seasons
                .AsNoTracking()
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);

            if (season is null) { return NotFound(); }

            ViewBag.seasonId = seasonID;
            ViewBag.number = season.SeasonNumber;

            var seasondrivers = Context.SeasonDrivers
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(sd => sd.SeasonId == seasonID)
                .Include(sd => sd.Driver)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Team)
                .Include(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Engine)
                .OrderByDescending(sd => (sd.Skill + sd.SeasonTeam.Chassis + sd.SeasonTeam.Engine.Power + (((int)sd.DriverStatus) * -2) + 2))
                .ToList();

            return View(seasondrivers);
        }

        [Route("[Controller]/{seasonID}/Settings")]
        public async Task<IActionResult> Settings(int seasonID, string statusmessage = null)
        {
            var season = await _seasons.GetSeasonById(seasonID);

            if (season is null) { return NotFound(); }

            ViewBag.statusmessage = statusmessage;
            return View(new SeasonSettingsViewModel(season));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{seasonID}/Settings")]
        public async Task<IActionResult> Settings(int seasonID, [Bind] SeasonSettingsViewModel settingsModel)
        {
            var season = await Context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);

            if (season is null || settingsModel is null) { return NotFound(); }

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
        public async Task<IActionResult> SetPoints(int seasonID)
        {
            var season = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var model = new SeasonSetPointsModel
            {
                SeasonId = seasonID,
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
            if (model is null) { return NotFound(); }

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
            return RedirectToAction(nameof(Settings), new { seasonID = model.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{seasonID}/Teams/Add")]
        public async Task<IActionResult> AddTeams(int seasonID)
        {
            var seasons = await Context.Seasons
                .Where(s => s.State == SeasonState.Draft || s.State == SeasonState.Progress)
                .Include(s => s.Teams)
                .ToListAsync();

            if (seasons is null) { return NotFound(); }

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

            ViewBag.seasonId = seasonID;
            return View(unregisteredTeams);
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{seasonID}/Teams/Add/{globalTeamID}")]
        public async Task<IActionResult> AddTeam(int seasonID, int globalTeamId)
        {
            var season = await _seasons.GetSeasonById(seasonID);
            var globalTeam = await Context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId);

            if (season is null || globalTeam is null) { return NotFound(); }

            var engines = await Context.Engines
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();
            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            var rubbers = await Context.Rubbers
                .Select(r => new { r.RubberId, r.Name })
                .ToListAsync();
            ViewBag.rubbers = new SelectList(rubbers, nameof(Rubber.RubberId), nameof(Rubber.Name));
            ViewBag.seasonId = seasonID;

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
        [HttpPost("[Controller]/{seasonID}/Teams/Add/{globalTeamID}")]
        public async Task<IActionResult> AddTeam(int seasonID, int globalTeamID, [Bind] SeasonTeam seasonTeam)
        {
            // Get and validate URL parameter objects.
            var season = await Context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);

            var globalTeam = await Context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamID);

            if (season is null || globalTeam is null || seasonTeam is null)
                return NotFound();

            if (season.Teams.Select(d => d.TeamId).Contains(seasonTeam.TeamId))
                return UnprocessableEntity();

            if (ModelState.IsValid)
            {
                // Set the Season and global Driver again as these are not bound in the view.
                seasonTeam.SeasonId = seasonID;
                seasonTeam.TeamId = globalTeamID;

                // Persist the new SeasonDriver and return to AddDrivers page.
                await Context.AddAsync(seasonTeam);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(AddTeams), new { seasonID });
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
        [Route("[Controller]/{seasonID}/Teams/Update/{teamID}")]
        public async Task<IActionResult> UpdateTeam(int seasonID, int teamID)
        {
            var season = await Context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var team = season.Teams.SingleOrDefault(t => t.SeasonTeamId == teamID);

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
            ViewBag.seasonId = seasonID;
            return View("AddOrUpdateTeam", team);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{seasonID}/Teams/Update/{teamID}")]
        public async Task<IActionResult> UpdateTeam(int seasonID, int teamID, [Bind] SeasonTeam updatedTeam)
        {
            var season = await Context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var team = season.Teams.SingleOrDefault(d => d.SeasonTeamId == teamID);

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
                return RedirectToAction(nameof(Detail), new { seasonID });
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
        [Route("[Controller]/{seasonID}/Drivers/Add")]
        public async Task<IActionResult> AddDrivers(int seasonID)
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

            ViewBag.seasonId = seasonID;
            var result = await Context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            ViewBag.year = result.SeasonNumber;
            return View(unregisteredDrivers);
        }

        [Authorize(Roles = "Admin")]
        [Route("[Controller]/{seasonID}/Drivers/Add/{globalDriverID}")]
        public async Task<IActionResult> AddDriver(int seasonID, int globalDriverID)
        {
            var season = await Context.Seasons
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var globalDriver = await Context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverID);

            if (season is null || globalDriver is null)
                return NotFound();

            var teams = season.Teams
                .Select(t => new { t.SeasonTeamId, t.Name })
                .ToList();

            ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
            ViewBag.seasonId = seasonID;

            var seasonDriver = new SeasonDriver
            {
                Driver = globalDriver,
                Season = season
            };

            // Adds last previous used values from driver as default
            var allSeasonDrivers = await Context.SeasonDrivers.ToListAsync();
            var lastDriver = allSeasonDrivers
                .LastOrDefault(ld => ld.DriverId == globalDriverID);

            if (lastDriver != null)
            {
                seasonDriver.Skill = lastDriver.Skill;
                seasonDriver.Reliability = lastDriver.Reliability;
                seasonDriver.DriverStatus = lastDriver.DriverStatus;
            }

            return View("AddOrUpdateDriver", seasonDriver);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{seasonID}/Drivers/Add/{globalDriverID}")]
        public async Task<IActionResult> AddDriver(int seasonID, int globalDriverID, [Bind] SeasonDriver seasonDriver)
        {
            // Get and validate URL parameter objects.
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var globalDriver = await Context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverID);

            if (season is null || globalDriver is null || seasonDriver is null)
                return NotFound();

            if (season.Drivers.Select(d => d.DriverId).Contains(seasonDriver.DriverId))
                return UnprocessableEntity();

            if (ModelState.IsValid)
            {
                // Set the Season and global Driver again as these are not bound in the view.
                seasonDriver.SeasonId = seasonID;
                seasonDriver.DriverId = globalDriverID;

                // Persist the new SeasonDriver and return to AddDrivers page.
                await Context.AddAsync(seasonDriver);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(AddDrivers), new { seasonID });
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
        [Route("[Controller]/{seasonID}/Drivers/Update/{driverID}")]
        public async Task<IActionResult> UpdateDriver(int seasonID, int driverID)
        {
            // Get and validate URL parameter objects.
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(s => s.Driver)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverID);

            if (season is null || driver is null)
                return NotFound();

            var teams = season.Teams
                .Select(t => new { t.SeasonTeamId, t.Name })
                .ToList();

            ViewBag.teams = new SelectList(teams, nameof(SeasonTeam.SeasonTeamId), nameof(SeasonTeam.Name));
            ViewBag.seasonId = seasonID;
            return View("AddOrUpdateDriver", driver);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[Controller]/{seasonID}/Drivers/Update/{driverID}")]
        public async Task<IActionResult> UpdateDriver(int seasonID, int driverID, [Bind] SeasonDriver updatedDriver)
        {
            // Get and validate URL parameter objects.
            var season = await Context.Seasons
                .Include(s => s.Drivers)
                    .ThenInclude(d => d.Driver)
                .Include(s => s.Teams)
                .SingleOrDefaultAsync(s => s.SeasonId == seasonID);

            var driver = season.Drivers
                .SingleOrDefault(d => d.SeasonDriverId == driverID);

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
                return RedirectToAction(nameof(Detail), new { seasonID });
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

        [Route("[Controller]/{seasonID}/Driver/Penaltylist/")]
        public async Task<IActionResult> PenaltyList(int seasonID)
        {
            ViewBag.seasonId = seasonID;
            var drivers = await Context.SeasonDrivers
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)
                .Where(s => s.SeasonId == seasonID)
                .OrderBy(s => s.SeasonTeam.Name)
                .ToListAsync();

            return View(drivers);
        }

        public async Task<IActionResult> DriverDev(int seasonID)
        {
            DriverDevModel viewmodel = new DriverDevModel();
            var season = await Context.Seasons.FirstAsync(s => s.SeasonId == seasonID);
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
                .Where(s => s.SeasonId == seasonID && !s.Dropped)
                .Include(t => t.SeasonTeam)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Abbreviation)
                .ToListAsync();

            return View(viewmodel);
        }

        private int GetCurrentYear(int seasonID)
        {
            var season = Context.Seasons.FirstOrDefault(s => s.SeasonId == seasonID);
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
            return RedirectToAction(nameof(DriverDev), new { seasonID = seasonId.SeasonId });
        }

        public async Task<IActionResult> TeamDev(int seasonID)
        {
            ViewBag.seasonId = seasonID;

            return View(await Context.SeasonTeams
                .Where(s => s.SeasonId == seasonID)
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

            return RedirectToAction(nameof(TeamDev), new { seasonID = seasonId.SeasonId });
        }

        public async Task<IActionResult> EngineDev(int seasonID)
        {
            ViewBag.seasonId = seasonID;

            var teams = await Context.SeasonTeams
                .Where(s => s.SeasonId == seasonID)
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

            return RedirectToAction(nameof(EngineDev), new { seasonID = seasonId.SeasonId });
        }

        public async Task<IActionResult> TeamReliabilityDev(int seasonID)
        {
            ViewBag.seasonId = seasonID;

            return View(await Context.SeasonTeams
                .Where(s => s.SeasonId == seasonID)
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

            return RedirectToAction(nameof(TeamReliabilityDev), new { seasonID = seasonId.SeasonId });
        }

        public async Task<IActionResult> DriverReliabilityDev(int seasonID)
        {
            ViewBag.seasonId = seasonID;
            ViewBag.year = GetCurrentYear(seasonID);

            return View(await Context.SeasonDrivers
                .Where(s => s.SeasonId == seasonID && !s.Dropped)
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

            return RedirectToAction(nameof(DriverReliabilityDev), new { seasonID = seasonId.SeasonId });
        }

        [Authorize(Roles = "Admin")]
        [Route("Driver/Drop/{driverID}")]
        public async Task<IActionResult> DropDriverFromTeam(int seasonID, int driverID)
        {
            var driver = await Context.SeasonDrivers.SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverID);
            if (driver is null)
                return NotFound();

            driver.Dropped = true;
            Context.Update(driver);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { seasonID });
        }

        public async Task<IActionResult> QualifyingBattle(int seasonID)
        {
            var races = await Context.Races
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonID && r.RaceState == RaceState.Finished)
                .Include(r => r.DriverResults)
                    .ThenInclude(dr => dr.SeasonDriver)
                .ToListAsync();

            var drivers = await Context.SeasonDrivers
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonID)
                .Select(sd => sd.SeasonDriverId)
                .ToListAsync();

            var teams = await Context.SeasonTeams
                .AsNoTracking()
                .IgnoreQueryFilters()
                .Where(r => r.SeasonId == seasonID)
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
            return View(new QualifyingBattleModel { QualyBattles = Battles, Teams = teams, SeasonId = seasonID });
        }
    }

    public class GetDev
    {
        public int Id { get; set; }
        public int Newdev { get; set; }
    }
}
