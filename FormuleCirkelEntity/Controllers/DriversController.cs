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
        private readonly IDriverService _drivers;
        private readonly ITraitService _traits;
        private readonly ISeasonService _seasons;

        public DriversController(FormulaContext context,
            UserManager<SimUser> userManager,
            PagingHelper pagingHelper,
            IDriverService dataService,
            ISeasonService seasonService,
            ITraitService traitService)
            : base(context, userManager, pagingHelper, dataService)
        {
            _drivers = dataService;
            _seasons = seasonService;
            _traits = traitService;
        }

        [SortResult(nameof(Driver.Name)), PagedResult]
        public override async Task<IActionResult> Index()
        {
            var drivers = await _drivers.GetEntities().ConfigureAwait(false);
            ViewBag.driverIds = drivers.Select(d => d.Id);
            return base.Index().Result;
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int id)
        {
            var stats = new DriverStatsModel();

            // Prepares table items for ViewModel
            var driver = await _drivers.GetEntityByIdUnfiltered(id).ConfigureAwait(false);
            var seasons = await _seasons.GetSeasons(true, true).ConfigureAwait(false);

            // Basic information about the driver
            stats.DriverId = driver.Id;
            stats.DriverName = driver.Name;
            stats.DriverNumber = driver.DriverNumber;
            stats.DriverCountry = driver.Country;
            stats.DriverBio = driver.Biography;

            // Count of the types of race finishes the driver had
            var results = await _context.DriverResults
                .AsNoTracking()
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver)
                .ToListAsync();

            stats.StartCount = results.Count;
            stats.WinCount = results.Count(r => r.Position == 1);
            stats.SecondCount = results.Count(r => r.Position == 2);
            stats.ThirdCount = results.Count(r => r.Position == 3);
            stats.PoleCount = results.Count(r => r.Grid == 1);
            stats.DNFCount = results.Count(r => r.Status == Status.DNF);
            stats.DSQCount = results.Count(r => r.Status == Status.DSQ);

            // Calculate point finishes
            int pointCount = 0;
            foreach (var season in seasons)
            {
                var current = results.Where(r => r.SeasonDriver.SeasonId == season.SeasonId);
                var pointsMax = season.PointsPerPosition.Keys.Max();
                pointCount += (current.Count(dr => dr.Position > 3 && dr.Position <= pointsMax));
            }

            // Apply point finishes and subtract others to form outside point finishes
            stats.PointFinishCount = pointCount;
            stats.OutsideCount = (stats.StartCount - stats.WinCount - stats.SecondCount - stats.ThirdCount - pointCount - stats.DNFCount - stats.DSQCount);
            // Count of the sort of non-finishes a driver had
            stats.AccidentCount = results.Count(r => r.DNFCause == DNFCause.Accident || r.DNFCause == DNFCause.Puncture);
            stats.ContactCount = results.Count(r => r.DNFCause == DNFCause.Damage || r.DNFCause == DNFCause.Collision);
            stats.EngineCount = results.Count(r => r.DNFCause == DNFCause.Engine);
            stats.MechanicalCount = results.Count(r => r.DNFCause == DNFCause.Brakes || r.DNFCause == DNFCause.Clutch || r.DNFCause == DNFCause.Electrics ||
                r.DNFCause == DNFCause.Exhaust || r.DNFCause == DNFCause.Hydraulics || r.DNFCause == DNFCause.Wheel);

            var seasondriver = await _context.SeasonDrivers
                .Where(s => s.Driver.Id == id)
                .Include(s => s.SeasonTeam)
                    .Where(st => st.Season.Championship.ActiveChampionship)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .ToListAsync();

            stats.Teams = seasondriver.Select(s => s.SeasonTeam.Team).Distinct().ToList();
            // Calculates the amount of WDCs a driver might have
            var driverChampions = _drivers.GetDriverChampionsIds(seasons);
            stats.WDCCount = driverChampions.Count(s => s == id);

            return View(stats);
        }

        [Route("Traits/{id}")]
        public async Task<IActionResult> DriverTraits(int id)
        {
            var driver = await _drivers.GetEntityById(id).ConfigureAwait(false);
            var driverTraits = await _traits.GetTraitsFromDriver(id).ConfigureAwait(false);
            var usedTraitIds = driverTraits.Select(drt => drt.TraitId).ToList();
            var traits = await _traits.GetUnusedTraitsFromEntity(TraitGroup.Driver, usedTraitIds).ConfigureAwait(false);

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
            Driver driver = await _drivers.GetEntityById(id).ConfigureAwait(false);
            Trait trait = await _traits.GetTraitById(traitId).ConfigureAwait(false);

            if (driver is null || trait is null)
                return NotFound();

            await _traits.AddTraitToDriver(driver, trait).ConfigureAwait(false);
            _drivers.Update(driver);
            _traits.Update(trait);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(DriverTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{driverId}")]
        public async Task<IActionResult> RemoveDriverTrait(int driverId, int traitId)
        {
            Driver driver = await _drivers.GetEntityById(driverId).ConfigureAwait(false);
            Trait trait = await _traits.GetTraitById(traitId).ConfigureAwait(false);

            if (driver == null || trait == null)
                return NotFound();

            await _traits.RemoveTraitFromDriver(driver, trait).ConfigureAwait(false);
            _drivers.Update(driver);
            _traits.Update(trait);
            await _context.SaveChangesAsync().ConfigureAwait(false);
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
                    .FirstOrDefault()?
                    .Driver;

                if (driverTitles.ContainsKey(winner))
                    driverTitles[winner]++;
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
            return drivers
                .Select(t => new { t.Key, Sum = t.Sum(selector) })
                .AsEnumerable()
                .Select(t => new KeyValuePair<Driver, int>(t.Key, t.Sum))
                .OrderByDescending(res => res.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        [Route("Archived")]
        public async Task<IActionResult> ArchivedDrivers()
        {
            var drivers = await _drivers.GetArchivedDrivers().ConfigureAwait(false);
            return View(drivers);
        }

        [HttpPost("SaveBiography")]
        public async Task<IActionResult> SaveBiography(int id, string biography)
        {
            await _drivers.SaveBio(id, biography).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction("Stats", new { id });
        }
    }
}
