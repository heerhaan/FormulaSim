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
            ViewBag.driverId = await _drivers.GetRandomDriverId();
            return base.Index().Result;
        }

        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id)
        {
            var updatingObject = await _drivers.GetDriverById(id);
            if (updatingObject == null)
                return NotFound();

            return View("Modify", updatingObject);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public virtual async Task<IActionResult> Edit(int id, Driver updatedObject)
        {
            if (updatedObject is null) { return NotFound(); }
            updatedObject.Id = id;

            if (!ModelState.IsValid)
                return View("Modify", updatedObject);

            if (await _drivers.FirstOrDefault(res => res.Id == id) is null)
                return NotFound();

            _drivers.Update(updatedObject);
            await _drivers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [Route("Delete/{id}")]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _drivers.GetDriverById(id, true);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [HttpErrorsToPagesRedirect]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objectToDelete = await _drivers.GetDriverById(id, true);
            if (objectToDelete == null)
                return NotFound();

            _drivers.Archive(objectToDelete);
            await _drivers.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int id)
        {
            // Prepares table items for ViewModel
            var stats = new DriverStatsModel();
            var driver = await _drivers.GetDriverById(id, true);
            var seasons = await _seasons.GetSeasons(true, true);
            // [TODO]: Statement underneath should be converted to a service call to driverresultservice
            var results = await Context.DriverResults
                .AsNoTracking()
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver)
                .ToListAsync();
            // Fills the viewmodel class with the required data
            _drivers.PrepareDriverStatsModel(stats, driver, seasons, results);
            // Gets all the teams that this driver has driven for
            stats.Teams = await _drivers.GetDistinctTeamsHistoryByDriver(id);
            // Calculates the amount of WDCs a driver might have
            var driverChampions = _drivers.GetDriverChampionsIds(seasons);
            stats.WDCCount = driverChampions.Count(s => s == id);

            return View(stats);
        }

        [Route("Traits/{id}")]
        public async Task<IActionResult> DriverTraits(int id)
        {
            var driver = await _drivers.GetDriverById(id);
            var driverTraits = await _traits.GetTraitsFromDriver(id);
            var usedTraitIds = driverTraits.ConvertAll(drt => drt.TraitId);
            var traits = await _traits.GetUnusedTraitsFromEntity(TraitGroup.Driver, usedTraitIds);

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
            Driver driver = await _drivers.GetDriverById(id);
            Trait trait = await _traits.GetTraitById(traitId);

            if (driver is null || trait is null)
                return NotFound();

            await _traits.AddTraitToDriver(driver, trait);
            _drivers.Update(driver);
            _traits.Update(trait);
            await _drivers.SaveChangesAsync();
            return RedirectToAction(nameof(DriverTraits), new { id });
        }

        [Authorize(Roles = "Admin")]
        [Route("Traits/Remove/{driverId}")]
        public async Task<IActionResult> RemoveDriverTrait(int driverId, int traitId)
        {
            Driver driver = await _drivers.GetDriverById(driverId);
            Trait trait = await _traits.GetTraitById(traitId);

            if (driver == null || trait == null)
                return NotFound();

            await _traits.RemoveTraitFromDriver(driver, trait);
            _drivers.Update(driver);
            _traits.Update(trait);
            await _drivers.SaveChangesAsync();
            return RedirectToAction(nameof(DriverTraits), new { id = driverId });
        }

        [Route("Leaderlists")]
        public IActionResult Leaderlists()
        {
            DriverLeaderlistsModel leaderlistsModel = new DriverLeaderlistsModel();

            var drivers = Context.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.Race.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver.Driver)
                .AsEnumerable()
                .GroupBy(sd => sd.SeasonDriver.Driver)
                .ToList();

            var seasons = Context.Seasons
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
            var drivers = await _drivers.GetArchivedDrivers();
            return View(drivers);
        }

        [HttpPost("SaveBiography")]
        public async Task<IActionResult> SaveBiography(int id, string biography)
        {
            await _drivers.SaveBio(id, biography);
            await _drivers.SaveChangesAsync();
            return RedirectToAction("Stats", new { id });
        }
    }
}
