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

            // Prepares table items for ViewModel
            var driver = await Data.IgnoreQueryFilters().FindAsync(id ?? 0);
            var seasondriver = DataContext
                .SeasonDrivers
                .Where(s => s.Driver.Id == id)
                .Include(s => s.SeasonTeam)
                    .Where(st => st.Season.Championship.ActiveChampionship)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team);
            var driverresult = DataContext.DriverResults
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver)
                    .ThenInclude(sd => sd.SeasonTeam)
                    .ThenInclude(st => st.Team)
                 .Include(dr => dr.SeasonDriver)
                    .ThenInclude(sd => sd.Season);
            var seasons = DataContext.Seasons
                .Where(s => s.Championship.ActiveChampionship);

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

            ViewBag.championships = championships;

            var stats = new DriverStatsModel()
            {
                Driver = driver,
                SeasonDriver = seasondriver,
                DriverResults = driverresult,
                Seasons = seasons
            };

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
