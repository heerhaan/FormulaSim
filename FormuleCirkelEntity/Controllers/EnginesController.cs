using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace FormuleCirkelEntity.Controllers
{
    public class EnginesController : Controller
    {
        private readonly FormulaContext _context;

        public EnginesController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            var engines = _context.Engines;
            var pageNumber = page ?? 1;
            var onePageOfEngines = engines.ToPagedList(pageNumber, 10);
            ViewBag.OnePage = onePageOfEngines;
            return View();
        }

        public IActionResult Create()
        {
            var engine = new Engine();
            engine.Power = 0;
            engine.Available = true;
            return View("Modify", engine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Engine engine)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(engine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Modify", engine);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var engine = await _context.Engines.FindAsync(id);
            if (engine == null)
                return NotFound();
            return View("Modify", engine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind] Engine updatedEngine)
        {
            var engine = await _context.Engines.FindAsync(id);
            if (engine == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                engine.Available = updatedEngine.Available;
                engine.Power = updatedEngine.Power;
                engine.Name = updatedEngine.Name;
                _context.Update(engine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Modify", updatedEngine);
        }

        // GET: Engines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var engine = await _context.Engines
                .FirstOrDefaultAsync(m => m.EngineId == id);

            if (engine == null)
            {
                return NotFound();
            }

            return View(engine);
        }

        // POST: Engines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var engine = await _context.Engines.FindAsync(id);
            if (engine.Archived == false)
            {
                engine.Archived = true;
                _context.Engines.Update(engine);
                await _context.SaveChangesAsync();
            }
            else
            {
                engine.Archived = false;
                _context.Engines.Update(engine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ArchivedEngines()
        {
            var engines = _context.Engines.Where(e => e.Archived).ToList();
            return View(engines);
        }
    }
}
