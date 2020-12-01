using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Controllers
{
    public class TraitsController : FormulaController
    {
        public TraitsController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager)
            : base(context, identityContext, userManager)
        { }

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
                return NotFound();

            return View(trait);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,EngineRacePace,Archived")] Trait trait)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(trait);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(trait);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Modify(int? id)
        {
            if (id == null)
                return NotFound();

            var trait = await _context.Traits.FindAsync(id);
            if (trait == null)
                return NotFound();

            return View(trait);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(int id, [Bind("TraitId,Name,TraitGroup,TraitDescription,QualyPace,DriverRacePace,ChassisRacePace,ChassisReliability,DriverReliability,MaximumRNG,MinimumRNG,EngineRacePace")] Trait trait)
        {
            if (trait is null || id != trait.TraitId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trait);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trait);
        }
    }
}
