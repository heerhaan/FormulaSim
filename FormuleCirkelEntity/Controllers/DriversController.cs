using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class DriversController : Controller
    {
        private readonly FormulaContext _context;

        public DriversController(FormulaContext context)
        {
            _context = context;
        }

        // GET: Drivers
        public IActionResult Index()
        {
            var drivers = _context.Drivers.Where(d => !d.Archived).ToList();
            return View(drivers);
        }

        [HttpPost]
        public ActionResult Index(string searchString)
        {
            //Search functionality for driver index
            ViewData["SearchString"] = searchString;
            IQueryable<Driver> drivers = from d in _context.Drivers where d.Archived == false select d;

            if (!string.IsNullOrEmpty(searchString))
            {
                drivers = drivers.Where(d => d.Name.Contains(searchString));
            }

            return View(drivers.ToList());
        }

        public async Task<IActionResult> Stats(int? id)
        {
            if (id == null)
                return NotFound();

            // Prepares table items for ViewModel
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.DriverId == id);
            var seasondriver = _context.SeasonDrivers
                .Where(s => s.Driver.DriverId == id)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team);
            var driverresult = _context.DriverResults
                .Where(dr => dr.SeasonDriver.DriverId == id);

            // Calculates the amount of WDCs a driver might have.
            int championships = 0;
            foreach(var season in _context.Seasons)
            {
                var winner = _context.SeasonDrivers
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
                DriverResults = driverresult
            };

            return View(stats);
        }

        // GET: Drivers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("DriverId,DriverNumber,Name,Abbreviation")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        // POST: Drivers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,DriverNumber,Name,Abbreviation")] Driver driver)
        {
            if (id != driver.DriverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var edit = _context.Drivers.First(d => d.DriverId == driver.DriverId);
                    _context.Entry(edit).CurrentValues.SetValues(driver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.DriverId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // GET: Drivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            driver.Archived = true;
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);
        }

        [HttpPost]
        public IActionResult SaveBiography(int id, string biography)
        {
            var driver = _context.Drivers.SingleOrDefault(d => d.DriverId == id);
            driver.Biography = biography;
            _context.Drivers.Update(driver);
            _context.SaveChanges();
            return RedirectToAction("Stats", new { id });
        }
    }
}
