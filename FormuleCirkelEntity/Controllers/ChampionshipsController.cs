using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

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

        [Authorize(Roles = "Admin")]
        public IActionResult SetDevRanges(int id, string statusmessage = null)
        {
            var championship = _context.Championships.Find(id);
            if (championship is null)
                return NotFound();

            SetDevModel viewModel = new SetDevModel();
            viewModel.ChampionshipId = id;
            foreach (var skilldev in championship.SkillDevRanges)
            {
                viewModel.SkillKey.Add(skilldev.Key);
                viewModel.SkillLower.Add(skilldev.Value.MinDev);
                viewModel.SkillHigher.Add(skilldev.Value.MaxDev);
            }
            foreach (var agedev in championship.AgeDevRanges)
            {
                viewModel.AgeKey.Add(agedev.Key);
                viewModel.AgeLower.Add(agedev.Value.MinDev);
                viewModel.AgeHigher.Add(agedev.Value.MaxDev);
            }
            ViewBag.statusmessage = statusmessage;

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetDevRanges(SetDevModel setDevModel)
        {
            var championship = await _context.Championships.FindAsync(setDevModel.ChampionshipId);
            if (championship is null)
                return NotFound();

            if (setDevModel.AgeKey.Count > 0 && 
                setDevModel.AgeKey.Count == setDevModel.AgeHigher.Count && 
                setDevModel.AgeKey.Count == setDevModel.AgeLower.Count)
            {
                if (!CheckIfListIsInOrder(setDevModel.AgeKey))
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Age list wasn't in order!" });
                championship.AgeDevRanges.Clear();
                for (int i = 0; i < setDevModel.AgeKey.Count; i++)
                {
                    championship.AgeDevRanges.Add(setDevModel.AgeKey[i], 
                        new MinMaxDevRange { MinDev = setDevModel.AgeLower[i], MaxDev = setDevModel.AgeHigher[i] });
                }
            }
            // If all the values match, then sets the 
            if (setDevModel.SkillKey.Count > 0 &&
                setDevModel.SkillKey.Count == setDevModel.SkillHigher.Count &&
                setDevModel.SkillKey.Count == setDevModel.SkillLower.Count)
            {
                if (!CheckIfListIsInOrder(setDevModel.SkillKey))
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Error: Skill list wasn't in order!" });
                if (!CheckIfMinIsLessThanMax(setDevModel.SkillLower, setDevModel.SkillHigher))
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Error: Negative skill dev should be less than positive skill dev!" });
                championship.SkillDevRanges.Clear();
                for (int i = 0; i < setDevModel.SkillKey.Count; i++)
                {
                    championship.SkillDevRanges.Add(setDevModel.SkillKey[i],
                        new MinMaxDevRange { MinDev = setDevModel.SkillLower[i], MaxDev = setDevModel.SkillHigher[i] });
                }
            }
            _context.Update(championship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Error: Dev ranges succesfully set" });
        }

        private static bool CheckIfListIsInOrder(IList<int> Range)
        {
            var orderedList = Range.OrderBy(a => a);
            if (Range.SequenceEqual(orderedList))
                return true;
            else
                return false;
        }

        private static bool CheckIfMinIsLessThanMax(IList<int> Min, IList<int> Max)
        {
            if (Min.Count != Max.Count)
                return false;
            for (int i = 0; i < Max.Count; i++)
            {
                if (Max[i] < Min[i])
                    return false;
            }
            return true;
        }
    }
}
