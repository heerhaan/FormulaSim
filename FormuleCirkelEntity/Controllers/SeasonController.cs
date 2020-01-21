using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using FormuleCirkelEntity.Helpers;
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

        public SeasonController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var seasons = _context.Seasons
                .IgnoreQueryFilters()
                .Where(s => s.Championship.ActiveChampionship)
                .Include(s => s.Drivers)
                    .ThenInclude(dr => dr.Driver)
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .ToList();

            var championship = _context.Championships.FirstOrDefault(s => s.ActiveChampionship);
            if (championship != null)
                ViewBag.championship = championship.ChampionshipName;
            else
                ViewBag.championship = "NaN";

            return View(seasons);
        }

        public async Task<IActionResult> Create()
        {
            // make an option to insert the default races and setup from previous season.
            var season = new Season();
            var championship = _context.Championships.FirstOrDefault(c => c.ActiveChampionship);
            season.Championship = championship;
            await _context.AddAsync(season).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
        }

        public IActionResult AddDefault(int? id)
        {
            // gets the current and previous season in this championship
            var season = _context.Seasons.Include(s => s.Races).SingleOrDefault(s => s.SeasonId == id);
            var lastSeason = _context.Seasons.Include(s => s.Races).LastOrDefault(s => s.State == SeasonState.Finished && s.ChampionshipId == season.ChampionshipId);

            if (lastSeason != null)
            {
                season.PointsPerPosition = lastSeason.PointsPerPosition;
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
                            Weather = Utility.RandomWeather(),
                            RaceState = RaceState.Concept
                        };
                        season.Races.Add(newRace);
                    }
                }
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Start(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            if (season == null)
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
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Finish(int? id)
        {
            var season = await _context.Seasons.SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            if (season == null)
                return NotFound();

            season.State = SeasonState.Finished;
            _context.Update(season);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await _context.Seasons
                .IgnoreQueryFilters()
                .Include(s => s.Races)
                    .ThenInclude(r => r.Track)
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);

            var drivers = _context.SeasonDrivers
                .IgnoreQueryFilters()
                .Include(sd => sd.Driver)
                .Where(sd => sd.SeasonId == id);

            var teams = _context.SeasonTeams
                .IgnoreQueryFilters()
                .Include(t => t.Team)
                .Include(t => t.Engine)
                .Where(st => st.SeasonId == id);

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

        public async Task<IActionResult> RemoveRace(int? id, int seasonId)
        {
            var race = await _context.Races.SingleOrDefaultAsync(s => s.RaceId == id).ConfigureAwait(false);
            var season = await _context.Seasons.Include(s => s.Races).SingleOrDefaultAsync(s => s.SeasonId == seasonId).ConfigureAwait(false);
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
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Detail), new { id = seasonId });
        }

        // Page that displays the power rankings and a few settings, like the points distribution, of a season.
        public async Task<IActionResult> SeasonStats(int? id)
        {
            var season = await _context.Seasons
                .AsNoTracking()
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id)
                .ConfigureAwait(false);

            ViewBag.seasonId = id;
            ViewBag.number = season.SeasonNumber;

            if (season == null)
                return NotFound();

            ViewBag.points = season.PointsPerPosition.Values.ToList();

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
                //.OrderBy(sd => sd.SeasonTeam.Team.Name)
                .ToList();

            seasondrivers = AddTraitReliabilityEffects(seasondrivers);

            return View(seasondrivers);
        }

        // Adds the effects of a trait to the total driver and chassis reliablity to all the drivers.
        private List<SeasonDriver> AddTraitReliabilityEffects(List<SeasonDriver> drivers)
        {
            foreach (var driver in drivers)
            {
                // This loop looks over all the traits a driver has.
                foreach (var trait in driver.Traits.Values)
                {
                    if (trait.DriverReliability.HasValue)
                        driver.Reliability += trait.DriverReliability.Value;
                    if (trait.ChassisReliability.HasValue)
                        driver.SeasonTeam.Reliability += trait.ChassisReliability.Value;
                }
                // This loop looks over all the traits a team has.
                foreach (var trait in driver.SeasonTeam.Traits.Values)
                {
                    if (trait.DriverReliability.HasValue)
                        driver.Reliability += trait.DriverReliability.Value;
                    if (trait.ChassisReliability.HasValue)
                        driver.SeasonTeam.Reliability += trait.ChassisReliability.Value;
                }
            }

            return drivers;
        }

        [Route("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);

            if (season == null)
                return NotFound();

            return View(new SeasonSettingsViewModel(season));
        }

        [HttpPost("[Controller]/{id}/Settings")]
        public async Task<IActionResult> Settings(int id, [Bind] SeasonSettingsViewModel settingsModel)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);

            if (season == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                season.SeasonNumber = settingsModel.SeasonNumber;
                season.QualificationRNG = settingsModel.QualificationRNG;
                season.QualificationRemainingDriversQ2 = settingsModel.QualificationRemainingDriversQ2;
                season.QualificationRemainingDriversQ3 = settingsModel.QualificationRemainingDriversQ3;
                season.QualyBonus = settingsModel.QualyBonus;
                season.PolePoints = settingsModel.PolePoints;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Detail), new { id = season.SeasonId });
            }

            return View(settingsModel);
        }

        public IActionResult SetPoints(int id)
        {
            var season = _context.Seasons.SingleOrDefault(s => s.SeasonId == id);
            var model = new SetPointsModel
            {
                SeasonId = id,
                SeasonNumber = season.SeasonNumber
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
        public IActionResult AddTeams(int? id)
        {
            var seasons = _context.Seasons
                .Where(s => s.State == SeasonState.Draft || s.State == SeasonState.Progress)
                .Include(s => s.Teams);

            if (seasons == null)
                return NotFound();

            List<int> existingTeamIds = new List<int>();
            foreach (var season in seasons)
            {
                existingTeamIds.AddRange(season.Teams.Select(t => t.TeamId));
            }
            var unregisteredTeams = _context.Teams
                .Where(t => t.Archived == false)
                .Where(t => !existingTeamIds.Contains(t.Id)).ToList();

            ViewBag.seasonId = id;
            return View(unregisteredTeams);
        }

        [Route("[Controller]/{id}/Teams/Add/{globalTeamId}")]
        public async Task<IActionResult> AddTeam(int? id, int? globalTeamId)
        {
            var season = await _context.Seasons
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId).ConfigureAwait(false);

            if (season == null || globalTeam == null)
                return NotFound();

            var engines = _context.Engines.Where(e => e.Archived == false).Select(t => new { t.Id, t.Name });
            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
            ViewBag.seasonId = id;

            var seasonTeam = new SeasonTeam
            {
                Team = globalTeam,
                Season = season
            };

            // Adds last previous used values from team as default
            var lastTeam = _context.SeasonTeams.LastOrDefault(s => s.Team.Id == globalTeamId);
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
                seasonTeam.Traits = lastTeam.Traits;
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
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var globalTeam = await _context.Teams.SingleOrDefaultAsync(t => t.Id == globalTeamId).ConfigureAwait(false);

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
                await _context.AddAsync(seasonTeam).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(AddTeams), new { id });
            }
            else
            {
                var engines = _context.Engines.Where(e => e.Archived == false).Select(t => new { t.Id, t.Name });
                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
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
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var team = season.Teams.SingleOrDefault(t => t.SeasonTeamId == teamId);

            if (season == null || team == null)
                return NotFound();

            var engines = _context.Engines.Where(e => e.Archived == false).Select(t => new { t.Id, t.Name });
            ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
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
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
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
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Detail), new { id });
            }
            else
            {
                var engines = _context.Engines.Where(e => e.Archived == false).Select(t => new { t.Id, t.Name });
                ViewBag.engines = new SelectList(engines, nameof(Engine.Id), nameof(Engine.Name));
                return View("AddOrUpdateDriver", team);
            }
        }

        [Route("[Controller]/{id}/Drivers/Add")]
        public IActionResult AddDrivers(int? id)
        {
            var seasons = _context.Seasons
                .Where(s => s.State == SeasonState.Draft || s.State == SeasonState.Progress)
                .Include(s => s.Drivers);

            if (seasons == null)
                return NotFound();

            List<int> existingDriverIds = new List<int>();
            foreach (var season in seasons)
            {
                existingDriverIds.AddRange(season.Drivers.Select(d => d.DriverId));
            }
            var unregisteredDrivers = _context.Drivers
                .Where(d => d.Archived == false)
                .Where(d => !existingDriverIds.Contains(d.Id))
                .OrderByDescending(d => d.Name).ToList();

            ViewBag.seasonId = id;
            ViewBag.year = _context.Seasons.SingleOrDefault(s => s.SeasonId == id).SeasonNumber;
            return View(unregisteredDrivers);
        }

        [Route("[Controller]/{id}/Drivers/Add/{globalDriverId}")]
        public async Task<IActionResult> AddDriver(int? id, int? globalDriverId)
        {
            var season = await _context.Seasons
                .Include(s => s.Teams)
                    .ThenInclude(t => t.Team)
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId).ConfigureAwait(false);

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
                .LastOrDefault(s => s.Driver.Id == globalDriverId);
            if (lastDriver != null)
            {
                seasonDriver.Skill = lastDriver.Skill;
                seasonDriver.Reliability = lastDriver.Reliability;
                seasonDriver.Tires = lastDriver.Tires;
                seasonDriver.DriverStatus = lastDriver.DriverStatus;
                seasonDriver.Traits = lastDriver.Traits;
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
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var globalDriver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == globalDriverId).ConfigureAwait(false);

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
                .SingleOrDefaultAsync(s => s.SeasonId == id).ConfigureAwait(false);
            var driver = season.Drivers.SingleOrDefault(d => d.SeasonDriverId == driverId);

            if (season == null || driver == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                driver.SeasonTeamId = updatedDriver.SeasonTeamId;
                driver.Reliability = updatedDriver.Reliability;
                driver.Skill = updatedDriver.Skill;
                driver.Tires = updatedDriver.Tires;
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

        [Route("[Controller]/{id}/Driver/Penaltylist/")]
        public IActionResult PenaltyList(int id)
        {
            var drivers = _context.SeasonDrivers
                .Include(s => s.DriverResults)
                .Include(s => s.Driver)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Where(s => s.SeasonId == id)
                .OrderBy(s => s.SeasonTeam.Team.Name)
                .ToList();
            return View(drivers);
        }

        public IActionResult DriverDev(int id)
        {
            ViewBag.seasonId = id;
            ViewBag.year = GetCurrentYear(id);

            return View(_context.SeasonDrivers
                .Where(s => s.SeasonId == id)
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Name).ToList());
        }

        public int GetCurrentYear(int seasonId)
        {
            var season = _context.Seasons.FirstOrDefault(s => s.SeasonId == seasonId);
            return season.SeasonNumber;
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveDriverDev([FromBody]IEnumerable<GetDev> dev)
        {
            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
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
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
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
            var teams = _context.SeasonTeams.Where(s => s.SeasonId == id);
            var engines = teams
                .GroupBy(e => e.Engine)
                .Select(e => e.First())
                .Select(e => e.Engine)
                .ToList();

            return View(engines);
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveEngineDev([FromBody]IEnumerable<GetDev> dev)
        {
            var engines = _context.Engines;
            foreach (var enginedev in dev)
            {
                var engine = engines.First(e => e.Id == enginedev.Id);
                engine.Power = enginedev.Newdev;
            }
            _context.UpdateRange(engines);
            _context.SaveChanges();

            var seasonId = _context.Seasons
                .Where(s => s.State == SeasonState.Progress && s.Championship.ActiveChampionship)
                .FirstOrDefault();

            return RedirectToAction("EngineDev", new { id = seasonId.SeasonId });
        }
        
        public class GetDev
        {
            public int Id { get; set; }
            public int Newdev { get; set; }
        }

        [Route("Driver/Traits/{id}")]
        public async Task<IActionResult> DriverTraits(int id)
        {
            var seasondriver = await _context.SeasonDrivers
                .Include(sd => sd.Driver)
                .SingleOrDefaultAsync(t => t.SeasonDriverId == id)
                .ConfigureAwait(false);
            var traits = _context.Traits
                .Where(tr => tr.TraitGroup == TraitGroup.Driver && !seasondriver.Traits.Values.Contains(tr))
                .OrderBy(t => t.Name);

            if (seasondriver == null)
                return NotFound();

            var model = new TraitsDriverModel
            {
                Driver = seasondriver,
                Traits = traits
            };

            return View(model);
        }

        [HttpPost("Driver/Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverTraits(int id, [Bind("TraitId")] int traitId)
        {
            var seasondriver = await _context.SeasonDrivers
                .SingleOrDefaultAsync(t => t.SeasonDriverId == id)
                .ConfigureAwait(false);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId).ConfigureAwait(false);

            if (seasondriver == null || trait == null)
                return NotFound();

            seasondriver.Traits.Add(seasondriver.Traits.Count, trait);
            _context.Update(seasondriver);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction(nameof(DriverTraits), new { id });
        }

        [Route("Driver/Traits/Remove/{driverId}")]
        public async Task<IActionResult> RemoveDriverTrait(int driverId, int traitId)
        {
            var driver = await _context.SeasonDrivers.SingleOrDefaultAsync(sd => sd.SeasonDriverId == driverId).ConfigureAwait(false);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId).ConfigureAwait(false);

            if (driver == null || trait == null)
                return NotFound();

            var removetrait = driver.Traits.First(item => item.Value.TraitId == trait.TraitId);
            driver.Traits.Remove(removetrait);
            _context.Update(driver);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction(nameof(DriverTraits), new { id = driverId });
        }

        [Route("Team/Traits/{id}")]
        public async Task<IActionResult> TeamTraits(int id)
        {
            var seasonteam = await _context.SeasonTeams
                .Include(st => st.Team)
                .SingleOrDefaultAsync(st => st.SeasonTeamId == id)
                .ConfigureAwait(false);
            var traits = _context.Traits
                .Where(tr => tr.TraitGroup == TraitGroup.Team && !seasonteam.Traits.Values.Contains(tr))
                .OrderBy(t => t.Name);

            if (seasonteam == null)
                return NotFound();

            var model = new TraitsTeamModel
            {
                Team = seasonteam,
                Traits = traits
            };

            return View(model);
        }

        [HttpPost("Team/Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeamTraits(int id, [Bind("TraitId")] int traitId)
        {
            var seasonteam = await _context.SeasonTeams.SingleOrDefaultAsync(st => st.SeasonTeamId == id).ConfigureAwait(false);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId).ConfigureAwait(false);

            if (seasonteam == null || trait == null)
                return NotFound();

            seasonteam.Traits.Add(seasonteam.Traits.Count, trait);
            _context.Update(seasonteam);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction(nameof(TeamTraits), new { id });
        }

        [Route("Team/Traits/Remove/{teamId}")]
        public async Task<IActionResult> RemoveTeamTrait(int teamId, int traitId)
        {
            var team = await _context.SeasonTeams.SingleOrDefaultAsync(st => st.SeasonTeamId == teamId).ConfigureAwait(false);
            var trait = await _context.Traits.SingleOrDefaultAsync(tr => tr.TraitId == traitId).ConfigureAwait(false);

            if (team == null || trait == null)
                return NotFound();

            var removetrait = team.Traits.First(item => item.Value.TraitId == trait.TraitId);
            team.Traits.Remove(removetrait);
            _context.Update(team);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            return RedirectToAction(nameof(TeamTraits), new { id = teamId });
        }
    }
}