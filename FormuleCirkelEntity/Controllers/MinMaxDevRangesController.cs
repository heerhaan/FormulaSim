using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Authorization;

namespace FormuleCirkelEntity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MinMaxDevRangesController : Controller
    {
        private readonly FormulaContext _context;

        public MinMaxDevRangesController(FormulaContext context)
        {
            _context = context;
        }

        // GET: MinMaxDevRanges
        public async Task<IActionResult> Index()
        {
            return View(await _context.MinMaxDevRange.ToListAsync());
        }

        // GET: MinMaxDevRanges/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MinMaxDevRanges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MinMaxDevId,ValueKey,MinDev,MaxDev")] MinMaxDevRange minMaxDevRange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(minMaxDevRange);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(minMaxDevRange);
        }

        // GET: MinMaxDevRanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var minMaxDevRange = await _context.MinMaxDevRange.FindAsync(id);
            if (minMaxDevRange == null)
            {
                return NotFound();
            }
            return View(minMaxDevRange);
        }

        // POST: MinMaxDevRanges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MinMaxDevId,ValueKey,MinDev,MaxDev")] MinMaxDevRange minMaxDevRange)
        {
            if (minMaxDevRange is null || id != minMaxDevRange.MinMaxDevId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(minMaxDevRange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MinMaxDevRangeExists(minMaxDevRange.MinMaxDevId))
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
            return View(minMaxDevRange);
        }

        // GET: MinMaxDevRanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var minMaxDevRange = await _context.MinMaxDevRange
                .FirstOrDefaultAsync(m => m.MinMaxDevId == id);
            if (minMaxDevRange == null)
            {
                return NotFound();
            }

            return View(minMaxDevRange);
        }

        // POST: MinMaxDevRanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var minMaxDevRange = await _context.MinMaxDevRange.FindAsync(id);
            _context.MinMaxDevRange.Remove(minMaxDevRange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MinMaxDevRangeExists(int id)
        {
            return _context.MinMaxDevRange.Any(e => e.MinMaxDevId == id);
        }
    }
}
