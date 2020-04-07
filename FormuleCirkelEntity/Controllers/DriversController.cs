using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    [Route("[controller]")]
    public class DriversController : ViewDataController<Driver>
    {
        public DriversController(FormulaContext context, PagingHelper pagingHelper)
            : base(context, pagingHelper)
        { }

        [SortResult(nameof(Driver.Name)), PagedResult]
        public override Task<IActionResult> Index()
        {
            return base.Index();
        }

        [Route("Stats/{id}")]
        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            var stats = new DriverStatsModel();

            // Prepares table items for ViewModel
            var driver = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);
            var seasons = DataContext.Seasons
                .Where(s => s.Championship.ActiveChampionship);

            // Basic information about the driver
            stats.DriverId = driver.Id;
            stats.DriverName = driver.Name;
            stats.DriverBio = driver.Biography;

            // Count of the types of race finishes the driver had
            var results = DataContext.DriverResults
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship);

            stats.StartCount = results.Count();
            stats.WinCount = results.Where(r => r.Position == 1).Count();
            stats.PodiumCount = results.Where(r => r.Position == 2 || r.Position == 3).Count();
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
            stats.OutsideCount = (stats.StartCount - stats.WinCount - stats.PodiumCount - pointCount - stats.DNFCount - stats.DSQCount);
            // Count of the sort of non-finishes a driver had
            stats.AccidentCount = results.Where(r => r.DNFCause == DNFCause.Accident || r.DNFCause == DNFCause.Puncture).Count();
            stats.ContactCount = results.Where(r => r.DNFCause == DNFCause.Damage || r.DNFCause == DNFCause.Collision).Count();
            stats.EngineCount = results.Where(r => r.DNFCause == DNFCause.Engine).Count();
            stats.MechanicalCount = results.Where(r => r.DNFCause == DNFCause.Brakes || r.DNFCause == DNFCause.Clutch || r.DNFCause == DNFCause.Electrics ||
                r.DNFCause == DNFCause.Exhaust || r.DNFCause == DNFCause.Hydraulics || r.DNFCause == DNFCause.Wheel).Count();

            var seasondriver = DataContext
                .SeasonDrivers
                .Where(s => s.Driver.Id == id)
                .Include(s => s.SeasonTeam)
                    .Where(st => st.Season.Championship.ActiveChampionship)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team);

            stats.Teams = seasondriver.Select(s => s.SeasonTeam.Team).Distinct().ToList();

            // Calculates the amount of WDCs a driver might have.
            int championships = 0;
            foreach(var season in DataContext.Seasons)
            {
                var winner = DataContext.SeasonDrivers
                    .Where(s => s.SeasonId == season.SeasonId && s.Season.State == SeasonState.Finished)
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

        [Route("Archived")]
        public IActionResult ArchivedDrivers()
        {
            var drivers = Data.IgnoreQueryFilters().Where(d => d.Archived).OrderBy(d => d.Name).ToList();
            return View(drivers);
        }

        [HttpPost("SaveBiography")]
        public IActionResult SaveBiography(int id, string biography)
        {
            var driver = DataContext.Drivers.SingleOrDefault(d => d.Id == id);
            driver.Biography = biography;
            DataContext.Drivers.Update(driver);
            DataContext.SaveChanges();
            return RedirectToAction("Stats", new { id });
        }
    }
}
