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
        public TraitsController(FormulaContext context, IdentityContext identityContext, IAuthorizationService authorizationService, UserManager<SimUser> userManager)
            : base(context, identityContext, authorizationService, userManager)
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
            ViewBag.userid = await IsUserOwner();
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

        public async Task<IActionResult> Create()
        {
            // Checks if the current logged-in user is the sim owner
            var isOwner = await IsUserOwner();
            if (!isOwner)
                return Forbid();

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

        public async Task<IActionResult> Modify(int? id)
        {
            // Checks if the current logged-in user is the sim owner
            var isOwner = await IsUserOwner();
            if (!isOwner)
                return Forbid();

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
