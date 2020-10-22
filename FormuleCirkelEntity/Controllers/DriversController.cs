using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Filters;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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
                .Where(s => s.Championship.ActiveChampionship)
                .ToList();

            // Basic information about the driver
            stats.DriverId = driver.Id;
            stats.DriverName = driver.Name;
            stats.DriverNumber = driver.DriverNumber;
            stats.DriverBio = driver.Biography;

            // Count of the types of race finishes the driver had
            var results = DataContext.DriverResults
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

            var seasondriver = DataContext
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

        [Route("Leaderlists")]
        public async Task<IActionResult> Leaderlists()
        {
            LeaderlistsModel leaderlistsModel = new LeaderlistsModel();

            var drivers = DataContext.DriverResults
                .IgnoreQueryFilters()
                .Where(dr => dr.Race.Season.Championship.ActiveChampionship)
                .Include(dr => dr.SeasonDriver.Driver)
                .AsEnumerable()
                .GroupBy(sd => sd.SeasonDriver.Driver)
                .ToList();

            leaderlistsModel.LeaderlistWins = drivers
                .Select(dr => new LeaderlistWin
                {
                    Driver = dr.Key,
                    WinCount = dr.Sum(s => s.Position == 1 ? 1 : 0),
                })
                .OrderByDescending(dr => dr.WinCount)
                .Take(10)
                .ToList();

            leaderlistsModel.LeaderlistPodiums = drivers
                .Select(dr => new LeaderlistPodium
                {
                    Driver = dr.Key,
                    PodiumCount = dr.Sum(s => s.Position <= 3 ? 1 : 0),
                })
                .OrderByDescending(dr => dr.PodiumCount)
                .Take(10)
                .ToList();

            leaderlistsModel.LeaderlistStarts = drivers
                .Select(dr => new LeaderlistStart
                {
                    Driver = dr.Key,
                    StartCount = dr.Count(),
                })
                .OrderByDescending(dr => dr.StartCount)
                .Take(10)
                .ToList();

            leaderlistsModel.LeaderlistNonFinishes = drivers
                .Select(dr => new LeaderlistNonFinish
                {
                    Driver = dr.Key,
                    NonFinishCount = dr.Sum(s => s.Status == Status.DNF || s.Status == Status.DSQ ? 1 : 0),
                })
                .OrderByDescending(dr => dr.NonFinishCount)
                .Take(10)
                .ToList();

            leaderlistsModel.LeaderlistPoles = drivers
                .Select(dr => new LeaderlistPole
                {
                    Driver = dr.Key,
                    PoleCount = dr.Sum(s => s.Grid == 1 ? 1 : 0),
                })
                .OrderByDescending(dr => dr.PoleCount)
                .Take(10)
                .ToList();

            return View(leaderlistsModel);
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
