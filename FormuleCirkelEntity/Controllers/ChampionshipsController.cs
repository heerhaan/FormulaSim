using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FormuleCirkelEntity.Controllers
{
    public class ChampionshipsController : FormulaController
    {
        public ChampionshipsController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager)
            : base(context, identityContext, userManager)
        { }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Championships.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Index([Bind("ChampionshipId")] Championship champ)
        {
            if (champ == null)
                return NotFound();

            // Activates the current championship and ensures the rest is deactivated
            var championships = await _context.Championships.ToListAsync();
            foreach (var championship in championships)
            {
                if (championship.ChampionshipId == champ.ChampionshipId)
                    championship.ActiveChampionship = true;
                else
                    championship.ActiveChampionship = false;
            }
            _context.UpdateRange(championships);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ChampionshipId,ChampionshipName,ActiveChampionship")] Championship championship)
        {
            if (championship is null)
                throw new NullReferenceException();

            if (ModelState.IsValid)
            {
                var championships = await _context.Championships.ToListAsync();
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
    }
}
