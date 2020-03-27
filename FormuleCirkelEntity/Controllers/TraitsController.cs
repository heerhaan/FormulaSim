using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;

namespace FormuleCirkelEntity.Controllers
{
    public class TraitsController : Controller
    {
        private readonly FormulaContext _context;

        public TraitsController(FormulaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var traits = await _context.Traits.ToListAsync();
            var indexmodel = new TraitsIndexModel
            {
                DriverTraits = traits.Where(t => t.TraitGroup == TraitGroup.Driver).OrderBy(t => t.Name),
                TeamTraits = traits.Where(t => t.TraitGroup == TraitGroup.Team).OrderBy(t => t.Name),
                TrackTraits = traits.Where(t => t.TraitGroup == TraitGroup.Track).OrderBy(t => t.Name)
            };
            return View(indexmodel);
        }

        public async Task<IActionResult> Info(int? id)
        {
            if (id == null)
                return NotFound();

            var trait = await _context.Traits.FindAsync(id);

            if (trait == null)
            {
                return NotFound();
            }
            return View(trait);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,EngineRacePace,Archived")] Trait trait)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trait);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trait);
        }

        // GET: Traits/Edit/5
        public async Task<IActionResult> Modify(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trait = await _context.Traits.FindAsync(id);
            if (trait == null)
            {
                return NotFound();
            }
            return View(trait);
        }

        // POST: Traits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(int id, [Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,EngineRacePace")] Trait trait)
        {
            if (id != trait.TraitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trait);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TraitExists(trait.TraitId))
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
            return View(trait);
        }

        private bool TraitExists(int id)
        {
            return _context.Traits.Any(e => e.TraitId == id);
        }
    }
}
