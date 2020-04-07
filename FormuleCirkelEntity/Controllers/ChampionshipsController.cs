using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Controllers
{
    public class ChampionshipsController : Controller
    {
        private readonly FormulaContext _context;

        public ChampionshipsController(FormulaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Championships
                .ToListAsync());
        }

        [HttpPost]
        public IActionResult Index([Bind("ChampionshipId")] Championship champ)
        {
            if (champ == null)
            {
                return NotFound();
            }

            var championships = _context.Championships.ToList();
            foreach (var championship in championships)
            {
                if (championship.ChampionshipId == champ.ChampionshipId)
                {
                    championship.ActiveChampionship = true;
                }
                else
                {
                    championship.ActiveChampionship = false;
                }
            }
            _context.UpdateRange(championships);
            _context.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        // Underlying Task doesn't even have a View yet, but it's supposed to show the highscores for that certain championship.
        //
        //public async Task<IActionResult> Stats(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var championship = await _context.Championships
        //        .FirstOrDefaultAsync(m => m.ChampionshipId == id)
        //        ;
        //    if (championship == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(championship);
        //}

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("ChampionshipId,ChampionshipName,ActiveChampionship")] Championship championship)
        {
            if (ModelState.IsValid)
            {
                var championships = _context.Championships.ToList();
                foreach (var item in championships)
                {
                    item.ActiveChampionship = false;
                }
                _context.UpdateRange(championships);

                championship.ActiveChampionship = true;
                _context.Add(championship);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(championship);
        }

        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var championship = await _context.Championships
                .FirstOrDefaultAsync(m => m.ChampionshipId == id);

            if (championship == null)
            {
                return NotFound();
            }

            return View(championship);
        }

        // POST: Championships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var championship = await _context.Championships.FindAsync(id);
            _context.Championships.Remove(championship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChampionshipExists(int id)
        {
            return _context.Championships.Any(e => e.ChampionshipId == id);
        }
    }
}
