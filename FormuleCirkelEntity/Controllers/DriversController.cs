using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class DriversController : ViewDataController<Driver>
    {
        private readonly IDriverService _driverService;
        public DriversController(FormulaContext context, 
            UserManager<SimUser> userManager, 
            PagingHelper pagingHelper,
            IDriverService dataService)
            : base(context, userManager, pagingHelper, dataService)
        {
            _driverService = dataService;
        }

        [SortResult(nameof(Driver.Name)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            var drivers = await DataService.GetEntities();
            _driverService.Pee();
            ViewBag.driverIds = drivers.Select(d => d.Id);
            return base.Index().Result;
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int? id)
        {
            if (id is null)
                return NotFound();

            var stats = new DriverStatsModel();

            // Prepares table items for ViewModel
            var driver = await DataService.GetEntityByIdUnfiltered(id.Value);
            var seasons = _context.Seasons
                .Where(s => s.Championship.ActiveChampionship)
                .Include(s => s.Drivers)
                .ToList();

            // Basic information about the driver
            stats.DriverId = driver.Id;
            stats.DriverName = driver.Name;
            stats.DriverNumber = driver.DriverNumber;
            stats.DriverBio = driver.Biography;

            // Count of the types of race finishes the driver had
            var results = _context.DriverResults
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver)
                .ToList();

            stats.StartCount = results.Count;
            stats.WinCount = results.Where(r => r.Position == 1).Count();
            stats.SecondCount = results.Where(r => r.Position == 2).Count();
            stats.ThirdCount = results.Where(r => r.Position == 3).Count();
            stats.PoleCount = results.Where(r => r.Grid == 1).Count();
            stats.DNFCount = results.Where(r => r.Status == Status.DNF).Count();
            stats.DSQCount = results.Where(r => r.Status == Status.DSQ).Count();

            // Calculate point finishes
            int pointCount = 0;
            foreach (var season in seasons)
            {
                var current = results.Where(r => r.SeasonDriver.SeasonId == season.SeasonId);
                var pointsMax = season.PointsPerPosition.Keys.Max();
                pointCount += (current.Where(dr => dr.Position > 3 && dr.Position <= pointsMax).Count());
            }

            // Apply point finishes and subtract others to form outside point finishes
            stats.PointFinishCount = pointCount;
            stats.OutsideCount = (stats.StartCount - stats.WinCount - stats.SecondCount - stats.ThirdCount - pointCount - stats.DNFCount - stats.DSQCount);
            // Count of the sort of non-finishes a driver had
            stats.AccidentCount = results.Where(r => r.DNFCause == DNFCause.Accident || r.DNFCause == DNFCause.Puncture).Count();
            stats.ContactCount = results.Where(r => r.DNFCause == DNFCause.Damage || r.DNFCause == DNFCause.Collision).Count();
            stats.EngineCount = results.Where(r => r.DNFCause == DNFCause.Engine).Count();
            stats.MechanicalCount = results.Where(r => r.DNFCause == DNFCause.Brakes || r.DNFCause == DNFCause.Clutch || r.DNFCause == DNFCause.Electrics ||
                r.DNFCause == DNFCause.Exhaust || r.DNFCause == DNFCause.Hydraulics || r.DNFCause == DNFCause.Wheel).Count();

            var seasondriver = _context
                .SeasonDrivers
                .Where(s => s.Driver.Id == id)
                .Include(s => s.SeasonTeam)
                    .Where(st => st.Season.Championship.ActiveChampionship)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .ToList();

            stats.Teams = seasondriver.Select(s => s.SeasonTeam.Team).Distinct().ToList();

            // Calculates the amount of WDCs a driver might have.
            int championships = 0;
            foreach(var season in seasons)
            {
                var winner = season.Drivers
                    .OrderByDescending(dr => dr.Points)
                    .FirstOrDefault();

                if (winner != null)
                {
                    if (winner.DriverId == id)
                    {
                        championships++;
                    }
                }
            }
            stats.WDCCount = championships;

            return View(stats);
        }

        [Route("Traits/{id}")]
        public async Task<IActionResult> DriverTraits(int id)
        {
            Driver driver = await DataService.GetEntityById(id);

            List<Trait> driverTraits = await _context.DriverTraits
                .Where(drt => drt.DriverId == id)
                .Select(drt => drt.Trait)
                .ToListAsync();

            List<Trait> traits = _context.Traits
                .AsNoTracking()
                .AsEnumerable()
                .Where(tr => tr.TraitGroup == TraitGroup.Driver && !driverTraits.Any(drt => drt.TraitId == tr.TraitId))
                .OrderBy(tr => tr.Name)
                .ToList();

            var viewmodel = new DriverTraitsModel
            {
                Driver = driver,
                DriverTraits = driverTraits,
                Traits = traits
            };
            return View(viewmodel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Traits/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DriverTraits(int id, [Bind("TraitId")] int traitId)
        {
            Driver driver = await DataService.GetEntityById(id);
            Trait trait = await _context.Traits.FirstAsync(tr => tr.TraitId == traitId);

            if (driver is null || trait is null)
                return NotFound();

            DriverTrait newTrait = new DriverTrait { Driver = driver, Trait = trait };
            await _context.AddAsync(newTrait);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(DriverTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{driverId}")]
        public async Task<IActionResult> RemoveDriverTrait(int driverId, int traitId)
        {
            Driver driver = await _context.Drivers
                .Include(dr => dr.DriverTraits)
                .FirstAsync(dr => dr.Id == driverId);
            Trait trait = await _context.Traits
                .FirstAsync(tr => tr.TraitId == traitId);

            if (driver == null || trait == null)
                return NotFound();

            DriverTrait removetrait = driver.DriverTraits
                .First(drt => drt.TraitId == traitId);

            _context.Remove(removetrait);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DriverTraits), new { id = driverId });
        }

        [Route("Leaderlists")]
        public IActionResult Leaderlists()
        {
            DriverLeaderlistsModel leaderlistsModel = new DriverLeaderlistsModel();

            var drivers = _context.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.Race.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver.Driver)
                .AsEnumerable()
                .GroupBy(sd => sd.SeasonDriver.Driver)
                .ToList();

            var seasons = _context.Seasons
                .Where(s => s.Championship.ActiveChampionship && s.State == SeasonState.Finished)
                .Include(s => s.Drivers)
                    .ThenInclude(sd => sd.Driver)
                .ToList();

            Dictionary<Driver, int> driverTitles = new Dictionary<Driver, int>();
            foreach (var season in seasons)
            {
                var winner = season.Drivers
                    .OrderByDescending(sd => sd.Points)
                    .FirstOrDefault()
                    .Driver;

                if (driverTitles.ContainsKey(winner))
                    driverTitles[winner] += 1;
                else
                    driverTitles.Add(winner, 1);
            }
            driverTitles = driverTitles.OrderByDescending(res => res.Value).Take(10).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Counts how many races a driver has entered
            Dictionary<Driver, int> driverStarts = drivers
                .Select(t => new KeyValuePair<Driver, int>(t.Key, t.Count()))
                .OrderByDescending(res => res.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            // Creates local functions to be given as a parameter to get the sum of the targeted value
            static int winSelect(DriverResult a) => a.Position == 1 ? 1 : 0;
            static int podiumSelect(DriverResult a) => a.Position <= 3 ? 1 : 0;
            static int nonFinishSelect(DriverResult a) => a.Status == Status.DNF || a.Status == Status.DSQ ? 1 : 0;
            static int poleSelect(DriverResult a) => a.Grid == 1 ? 1 : 0;

            // All the gathered dictionaries are put in a viewmodel and sent over to the view to be put in leaderlists
            DriverLeaderlistsModel viewmodel = new DriverLeaderlistsModel
            {
                LeaderlistTitles = driverTitles,
                LeaderlistWins = GetDriverLeaderlistDict(drivers, winSelect),
                LeaderlistPodiums = GetDriverLeaderlistDict(drivers, podiumSelect),
                LeaderlistStarts = driverStarts,
                LeaderlistNonFinishes = GetDriverLeaderlistDict(drivers, nonFinishSelect),
                LeaderlistPoles = GetDriverLeaderlistDict(drivers, poleSelect)
            };

            return View(viewmodel);
        }

        // Generic helper method to get the right dictionary with the given selector
        private static Dictionary<Driver, int> GetDriverLeaderlistDict(List<IGrouping<Driver, DriverResult>> drivers, Func<DriverResult, int> selector)
        {
            Dictionary<Driver, int> driverDict = drivers
                .Select(t => new { t.Key, Sum = t.Sum(selector) })
                .AsEnumerable()
                .Select(t => new KeyValuePair<Driver, int>(t.Key, t.Sum))
                .OrderByDescending(res => res.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            return driverDict;
        }

        // Underneath should be added to the service eventually
        [Route("Archived")]
        public IActionResult ArchivedDrivers()
        {
            var drivers = _context.Drivers
                .IgnoreQueryFilters()
                .Where(d => d.Archived)
                .OrderBy(d => d.Name)
                .ToList();

            return View(drivers);
        }

        [HttpPost("SaveBiography")]
        public async Task<IActionResult> SaveBiography(int id, string biography)
        {
            var driver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == id);
            driver.Biography = biography;
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction("Stats", new { id });
        }
    }
}
