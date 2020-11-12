using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FormuleCirkelEntity.Controllers
{
    public class ChampionshipsController : FormulaController
    {
        public ChampionshipsController(FormulaContext context, IdentityContext identityContext, IAuthorizationService authorizationService, UserManager<SimUser> userManager)
            : base(context, identityContext, authorizationService, userManager)
        { }

        public async Task<IActionResult> Index()
        {
            ViewBag.userid = await IsUserOwner();
            return View(await _context.Championships.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index([Bind("ChampionshipId")] Championship champ)
        {
            // Checks if the current logged-in user is the sim owner
            var isOwner = await IsUserOwner();
            if (!isOwner)
                return Forbid();

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
            // Checks if the current logged-in user is the sim owner
            var isOwner = await IsUserOwner();
            if (!isOwner)
                return Forbid();

            if (championship is null)
                throw new NullReferenceException();

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
    }
}
