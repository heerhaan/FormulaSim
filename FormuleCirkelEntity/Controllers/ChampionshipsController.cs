using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Validation;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FormuleCirkelEntity.Controllers
{
    public class ChampionshipsController : FormulaController
    {
        public ChampionshipsController(FormulaContext context, 
            UserManager<SimUser> userManager)
            : base(context, userManager)
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

        [Authorize(Roles = "Admin")]
        public IActionResult SetDevRanges(int id, string statusmessage = null)
        {
            var championship = _context.Championships
                .Include(c => c.AgeDevRanges)
                .Include(c => c.SkillDevRanges)
                .Single(ch => ch.ChampionshipId == id);
            if (championship is null)
                return NotFound();

            ViewBag.statusmessage = statusmessage;
            return View(new SetDevModel(championship));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetDevRanges(SetDevModel setDevModel)
        {
            if (setDevModel is null)
                return NotFound();

            var championship = await _context.Championships
                .Include(c => c.AgeDevRanges)
                .Include(c => c.SkillDevRanges)
                .SingleAsync(ch => ch.ChampionshipId == setDevModel.ChampionshipId);
            
            // This if-structure makes it so that we are going to apply all those darn age dev ranges to the code!
            if (setDevModel.AgeValueKey.Count > 0)
            {
                // Empties the dev ranges for age so we can put in some new ages (owo)
                _context.MinMaxDevRange.RemoveRange(championship.AgeDevRanges);
                // If somehow the order of the keys is messed up then it will return the page with an error
                if (!CheckIfListIsInOrder(setDevModel.AgeValueKey))
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Error: Age list isn't in order!" });

                // Loops through all the key values which assumes that the age value, min dev and max dev lists are of the same length
                for (int i = 0; i < setDevModel.AgeValueKey.Count; i++)
                {
                    // Creates an MinMax object from the given lists, assuming they are made equally
                    MinMaxDevRange newAgeRange = new MinMaxDevRange
                    {
                        ValueKey = setDevModel.AgeValueKey[i],
                        MinDev = setDevModel.AgeMinDev[i],
                        MaxDev = setDevModel.AgeMaxDev[i]
                    };

                    // Validates the current entry
                    var validate = MinMaxDevValidator.ValidateMinMax(newAgeRange);
                    if (!validate.IsValid)
                    {
                        string errString = "";
                        foreach (var failure in validate.Errors)
                            errString += $"{failure.ErrorMessage}\n";
                        return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = $"Error: {errString}" });
                    }
                    // Validator didn't trigger so this line can be added to the ranges
                    championship.AgeDevRanges.Add(newAgeRange);
                }
            }
            // Applies the same process for the skill values
            if (setDevModel.SkillValueKey.Count > 0)
            {
                // Clear the existing list of skill ranges so they can be filled the right way again (uwu)
                _context.MinMaxDevRange.RemoveRange(championship.SkillDevRanges);
                // Ensures the keys are in order
                if (!CheckIfListIsInOrder(setDevModel.SkillValueKey))
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Error: Skill list wasn't in order!" });

                // Internally cries a bit because the code is so ugly
                for (int i = 0; i < setDevModel.SkillValueKey.Count; i++)
                {
                    // Creates an MinMax object from the given lists, assuming they are made equally
                    MinMaxDevRange newSkillRange = new MinMaxDevRange
                    {
                        ValueKey = setDevModel.SkillValueKey[i],
                        MinDev = setDevModel.SkillMinDev[i],
                        MaxDev = setDevModel.SkillMaxDev[i]
                    };

                    // Validates the current entry
                    var validate = MinMaxDevValidator.ValidateMinMax(newSkillRange);
                    if (!validate.IsValid)
                    {
                        string errString = "";
                        foreach (var failure in validate.Errors)
                            errString += $"{failure.ErrorMessage}\n";
                        return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = $"Error: {errString}" });
                    }
                    // Validator didn't trigger so this line can be added to the ranges
                    championship.SkillDevRanges.Add(newSkillRange);
                }
            }
            _context.Update(championship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Dev ranges succesfully set!" });
        }

        private static bool CheckIfListIsInOrder(IList<int> Range)
        {
            var orderedList = Range.OrderBy(a => a);
            if (Range.SequenceEqual(orderedList))
                return true;
            else
                return false;
        }
    }
}
