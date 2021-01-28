using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using FormuleCirkelEntity.Services;

namespace FormuleCirkelEntity.Controllers
{
    public class ChampionshipsController : FormulaController
    {
        private readonly IChampionshipService _champService;
        public ChampionshipsController(FormulaContext context,
            UserManager<SimUser> userManager,
            IChampionshipService service)
            : base(context, userManager)
        {
            _champService = service;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _champService.GetChampionships());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Index(int championshipId)
        {
            // Activates the current championship and ensures the rest is deactivated
            var championship = await _champService.GetChampionshipById(championshipId);
            await _champService.ActivateChampionship(championship);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Championship championship)
        {
            if (championship is null)
                throw new NullReferenceException();

            if (ModelState.IsValid)
            {
                await _champService.ActivateChampionship(championship);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(championship);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetDevRanges(int id, string statusmessage = null)
        {
            var championship = await _champService.GetChampionshipById(id, true);
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

            var championship = await _champService.GetChampionshipById(setDevModel.ChampionshipId, true);

            // This if-structure makes it so that we are going to apply all those darn age dev ranges to the code!
            if (setDevModel.AgeValueKey.Count > 0)
            {
                // Calls the service method that validates and adds the ranges to the championship
                var errString = _champService.SetRangeToChampionship(championship, true,
                    setDevModel.AgeValueKey,
                    setDevModel.AgeMinDev,
                    setDevModel.AgeMaxDev);
                // Uh oh, the returned string wasn't empty so this means something went wrong!
                // Time to return it to the view
                if (!string.IsNullOrEmpty(errString))
                {
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = $"Error at age: {errString}" });
                }
            }
            // Applies the same process for the skill values
            if (setDevModel.SkillValueKey.Count > 0)
            {
                // Calls the service method that validates and adds the ranges to the championship
                var errString = _champService.SetRangeToChampionship(championship, false,
                    setDevModel.SkillValueKey,
                    setDevModel.SkillMinDev,
                    setDevModel.SkillMaxDev);
                // Ruh roh, the returned string wasn't empty so this means something went wrong!
                // Time to return it to the view
                if (!string.IsNullOrEmpty(errString))
                {
                    return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = $"Error at skill: {errString}" });
                }
            }
            _champService.Update(championship);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(SetDevRanges), new { id = setDevModel.ChampionshipId, statusmessage = "Dev ranges succesfully set!" });
        }
    }
}
