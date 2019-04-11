﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Controllers
{
    public class EnginesController : Controller
    {
        private readonly FormulaContext _context;

        public EnginesController(FormulaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Engines.ToListAsync());
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
            _context.Engines.Remove(engine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
