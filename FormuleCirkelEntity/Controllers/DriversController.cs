using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
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
        public IActionResult Index(string sortOrder)
        {
            //Adds sorting functionality to index list
            ViewData["SortName"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SortNumber"] = String.IsNullOrEmpty(sortOrder) ? "number_asc" : "";

            var drivers = SortDrivers(sortOrder);

            return View(drivers.ToList());
        }

        [HttpPost]
        public ActionResult Index(string searchString, string sortOrder)
        {
            //Search functionality for driver index
            ViewData["SearchString"] = searchString;
            IQueryable<Driver> drivers;

            if (!String.IsNullOrEmpty(sortOrder))
            {
                drivers = SortDrivers(sortOrder);
            }
            else
            {
                drivers = from d in _context.Drivers select d;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                drivers = drivers.Where(d => d.Name.Contains(searchString));
            }

            return View(drivers.ToList());
        }

        //Sorts drivers based on given value
        public IQueryable<Driver> SortDrivers(string sortOrder)
        {
            IQueryable<Driver> drivers = from d in _context.Drivers select d;

            switch (sortOrder)
            {
                case "name_desc":
                    drivers = drivers.OrderByDescending(d => d.Name);
                    break;

                case "number_asc":
                    drivers = drivers.OrderBy(d => d.DriverNumber);
                    break;

                default:
                    drivers = drivers.OrderBy(d => d.Name);
                    break;
            }

            return drivers;
        }

        public async Task<IActionResult> Stats(int? id)
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
                    _context.Update(driver);
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
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DriverExists(int id)
        {
            return _context.Drivers.Any(e => e.DriverId == id);
        }
    }
}
