using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace FormuleCirkelEntity.Controllers
{
    public class DriversController : ViewDataController<Driver>
    {
        public DriversController(FormulaContext context, PagingHelper pagingHelper)
            : base(context, pagingHelper)
        { }

        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            // Prepares table items for ViewModel
            var driver = await Data.FindAsync(id);
            var seasondriver = DataContext.SeasonDrivers
                .Where(s => s.Driver.Id == id)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team);
            var driverresult = DataContext.DriverResults
                .Where(dr => dr.SeasonDriver.DriverId == id && dr.SeasonDriver.Season.Championship.ActiveChampionship)
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

                if(winner != null)
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

        public IActionResult ArchivedDrivers()
        {
            var drivers = DataContext.Drivers.Where(d => d.Archived).OrderBy(d => d.Name).ToList();
            return View(drivers);
        }

        [HttpPost]
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
