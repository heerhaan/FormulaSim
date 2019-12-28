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
            var traits = await _context.Traits.ToListAsync().ConfigureAwait(false);
            var indexmodel = new TraitsIndexModel
            {
                DriverTraits = traits.Where(t => t.TraitGroup == TraitGroup.Driver),
                TeamTraits = traits.Where(t => t.TraitGroup == TraitGroup.Team),
                TrackTraits = traits.Where(t => t.TraitGroup == TraitGroup.Track)
            };
            return View(indexmodel);
        }

        public async Task<IActionResult> Info(int? id)
        {
            if (id == null)
                return NotFound();

            var trait = await _context.Traits.FindAsync(id).ConfigureAwait(false);

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
        public async Task<IActionResult> Create([Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,ChassisMultiplier,DriverMultiplier,EngineMultiplier,Archived")] Trait trait)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trait);
                await _context.SaveChangesAsync().ConfigureAwait(false);
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

            var trait = await _context.Traits.FindAsync(id).ConfigureAwait(false);
            if (trait == null)
            {
                return NotFound();
            }
            return View(trait);
        }

        // POST: Traits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(int id, [Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,ChassisMultiplier,DriverMultiplier,EngineMultiplier,Archived")] Trait trait)
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
                    await _context.SaveChangesAsync().ConfigureAwait(false);
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
