using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
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
            return View(_context.Seasons.ToList());
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
            _context.Update(season);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Detail), new { id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            var season = await _context.Seasons
                .Include(s => s.Races)
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

        public async Task<IActionResult> AddTracks(int? id)
        {
            var season = await _context.Seasons
                   .Include(s => s.Races)
                   .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (season == null)
                return NotFound();

            var existingTrackIds = season.Races.Select(r => r.TrackId);
            var unusedTracks = _context.Tracks.Where(t => !existingTrackIds.Contains(t.TrackId)).ToList();

            ViewBag.seasonId = id;
            return View(unusedTracks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTracks(int? id, [Bind("TrackId")] Track track)
        {
            track = await _context.Tracks.SingleOrDefaultAsync(m => m.TrackId == track.TrackId);

            var season = await _context.Seasons
                .Include(s => s.Races)
                .SingleOrDefaultAsync(s => s.SeasonId == id);

            if (track == null || season == null)
                return NotFound();

            var race = new Race();
            race.Track = track;
            race.Name = track.Name;
            race.Round = (_context.Races.Where(r => r.SeasonId == id).Count()+1);
            season.Races.Add(race);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AddTracks), new { id });
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

            var seasonTeam = new SeasonTeam();
            seasonTeam.Team = globalTeam;
            seasonTeam.Season = season;
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
                team.Chassis = updatedTeam.Chassis;
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

            var teams = season.Teams.Select(t => new { t.SeasonTeamId, t.Team.Name });
            ViewBag.teams = new SelectList(teams, "SeasonTeamId", "Name");

            var seasonDriver = new SeasonDriver();
            seasonDriver.Driver = globalDriver;
            seasonDriver.Season = season;
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

        public IActionResult DriverDev()
        {
            return View(_context.SeasonDrivers
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Name).ToList());
        }
        
        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveDriverDev([FromBody]IEnumerable<GetDev> dev)
        {
            var drivers = _context.SeasonDrivers;
            foreach(var driverdev in dev)
            {
                var driver = drivers.First(d => d.SeasonDriverId == driverdev.Id);
                driver.Skill = driverdev.Newdev;
            }
            _context.UpdateRange(drivers);
            _context.SaveChanges();

            return RedirectToAction(nameof(DriverDev));
        }

        public IActionResult TeamDev()
        {
            return View(_context.SeasonTeams
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Name).ToList());
        }

        //Receives development values and saves them in the DB
        [HttpPost]
        public IActionResult SaveTeamDev([FromBody]IEnumerable<GetDev> dev)
        {
            var teams = _context.SeasonTeams;
            foreach (var teamdev in dev)
            {
                var team = teams.First(t => t.SeasonTeamId == teamdev.Id);
                team.Chassis = teamdev.Newdev;
            }
            _context.UpdateRange(teams);
            _context.SaveChanges();

            return RedirectToAction(nameof(TeamDev));
        }

        public IActionResult EngineDev()
        {
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

            return RedirectToAction(nameof(EngineDev));
        }

        [HttpGet]
        public IActionResult Development(int min, int max, string source)
        {
            if(source == null)
                return BadRequest();

            try
            {
                List<DevelopingValues> devlist = new List<DevelopingValues>();
                switch (source)
                {
                    case "driver":
                        devlist = DriverDevList(min, max);
                        break;
                    case "engine":
                        devlist = EngineDevList(min, max);
                        break;
                    case "team":
                        devlist = TeamDevList(min, max);
                        break;
                }
                return new JsonResult(devlist);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        public List<DevelopingValues> DriverDevList(int min, int max)
        {
            var devlist = new List<DevelopingValues>();
            var drivers = _context.SeasonDrivers
                .Include(t => t.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Include(d => d.Driver)
                .OrderBy(s => s.SeasonTeam.Team.Name).ToList();

            //Adds each driver in Season to list and adds development
            foreach (var driver in drivers)
            {
                int dev = rng.Next(min, max+1);

                devlist.Add(new DevelopingValues
                {
                    Id = driver.SeasonDriverId,
                    Name = driver.Driver.Name,
                    Number = driver.Driver.DriverNumber,
                    Abbreviation = driver.SeasonTeam.Team.Abbreviation,
                    Colour = driver.SeasonTeam.Team.Colour,
                    Accent = driver.SeasonTeam.Team.Accent,
                    Old = driver.Skill,
                    Dev = dev,
                    New = driver.Skill + dev
                });
            }
            return devlist;
        }

        public List<DevelopingValues> EngineDevList(int min, int max)
        {
            var devlist = new List<DevelopingValues>();
            var engines = _context.Engines.Where(e => e.Available == true).ToList();

            //Adds each driver in Season to list and adds development
            foreach (var engine in engines)
            {
                int dev = rng.Next(min, max+1);

                devlist.Add(new DevelopingValues
                {
                    Id = engine.EngineId,
                    Name = engine.Name,
                    Old = engine.Power,
                    Dev = dev,
                    New = engine.Power + dev
                });
            }
            return devlist;
        }

        public List<DevelopingValues> TeamDevList(int min, int max)
        {
            var devlist = new List<DevelopingValues>();

            var teams = _context.SeasonTeams
                .Include(t => t.Team)
                .OrderBy(t => t.Team.Name).ToList();

            //Adds each driver in Season to list and adds development
            foreach (var team in teams)
            {
                int dev = rng.Next(min, max+1);

                devlist.Add(new DevelopingValues
                {
                    Id = team.TeamId,
                    Name = team.Team.Name,
                    Abbreviation = team.Team.Abbreviation,
                    Colour = team.Team.Colour,
                    Accent = team.Team.Accent,
                    Old = team.Chassis,
                    Dev = dev,
                    New = team.Chassis + dev
                });
            }
            return devlist;
        }
    }

    public class DevelopingValues
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Abbreviation { get; set; }
        public string Colour { get; set; }
        public string Accent { get; set; }
        public int Old { get; set; }       
        public int Dev { get; set; }
        public int New { get; set; }
    }

    public class GetDev
    {
        public int Id { get; set; }
        public int Newdev { get; set; }
    }
}