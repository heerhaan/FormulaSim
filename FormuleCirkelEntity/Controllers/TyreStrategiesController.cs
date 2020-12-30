using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.Services;

namespace FormuleCirkelEntity.Controllers
{
    public class TyreStrategiesController : FormulaController
    {
        private readonly ITyreStrategyService _tyreStrats;
        public TyreStrategiesController(FormulaContext context,
            UserManager<SimUser> userManager,
            ITyreStrategyService tyreStrategyService)
            : base(context, userManager)
        {
            _tyreStrats = tyreStrategyService;
        }

        public async Task<IActionResult> TyreIndex()
        {
            var tyres = await _tyreStrats.GetTyres();
            return View(tyres);
        }

        public IActionResult TyreCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TyreCreate(Tyre tyre)
        {
            if (ModelState.IsValid)
            {
                await _tyreStrats.AddTyre(tyre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TyreIndex));
            }
            return View(tyre);
        }

        public async Task<IActionResult> TyreEdit(int id)
        {
            var tyre = await _tyreStrats.GetTyreById(id);
            return View(tyre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TyreEdit(int id, Tyre tyre)
        {
            if (tyre is null || id != tyre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _tyreStrats.UpdateTyre(tyre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TyreIndex));
            }
            return View(tyre);
        }

        public async Task<IActionResult> StrategyIndex()
        {
            var strategies = await _tyreStrats.GetStrategies(true, true);
            return View(strategies);
        }

        public async Task<IActionResult> StrategyCreate(int id = 0)
        {
            var tyres = await _tyreStrats.GetTyres(true);
            var strategy = await _tyreStrats.GetStrategyById(id, true);
            if (strategy is null)
            {
                var viewmodel = new CreateStrategyModel(0, 0, null, tyres);
                return View(viewmodel);
            }
            else
            {
                var viewmodel = new CreateStrategyModel(strategy.StrategyId, strategy.RaceLen, strategy.Tyres, tyres);
                return View(viewmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StrategyCreate(int strategyId, int tyreId, int raceLen, int applyNum)
        {
            var _strategy = await _tyreStrats.GetStrategyById(strategyId);
            var tyre = await _tyreStrats.GetTyreById(tyreId);
            if (tyre is null)
                return NotFound();

            Strategy strategy;
            if (_strategy is null)
            {
                strategy = new Strategy
                {
                    RaceLen = raceLen
                };
                await _tyreStrats.AddStrategy(strategy);
            }
            else
            {
                strategy = _strategy;
                strategy.RaceLen = raceLen;
                _tyreStrats.UpdateStrategy(strategy);
            }

             _tyreStrats.UpdateTyre(tyre);
            await _tyreStrats.AddTyreToStrategy(strategy, tyre, applyNum);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StrategyCreate), new { id = strategy.StrategyId });
        }

        public async Task<IActionResult> RemoveTyreStrategy(int tyreStratId, int strategyId)
        {
            var tyreStrat = await _tyreStrats.GetTyreStratById(tyreStratId);
            _tyreStrats.RemoveTyreFromStrategy(tyreStrat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StrategyCreate), new { id = strategyId });
        }
    }
}
