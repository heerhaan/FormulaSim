using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Mvc;

namespace FormuleCirkelEntity.Controllers
{
    public class SeasonController : Controller
    {
        private readonly FormulaContext _context;

        public SeasonController(FormulaContext context)
        {
            _context = context;
        }

        public IActionResult Main()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Main([Bind("SeasonId,CurrentSeason")] Season season)
        {
            if (ModelState.IsValid)
            {
                _context.Add(season);
                await _context.SaveChangesAsync();
                return RedirectToAction("AddTracks", "Races", new { seasonid = season.SeasonId });
            }
            return View(season);
        }
    }
}