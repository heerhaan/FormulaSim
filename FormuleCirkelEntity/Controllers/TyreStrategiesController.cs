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

namespace FormuleCirkelEntity.Controllers
{
    public class TyreStrategiesController : FormulaController
    {
        public TyreStrategiesController(FormulaContext context,
            UserManager<SimUser> userManager)
            : base(context, userManager)
        { }

        public async Task<IActionResult> TyreIndex()
        {
            var tyres = await _context.Tyres.ToListAsync();
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
                _context.Add(tyre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TyreIndex));
            }
            return View(tyre);
        }

        public async Task<IActionResult> TyreEdit(int id)
        {
            var tyre = await _context.Tyres.FindAsync(id);
            if (tyre == null)
                return NotFound();

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
                _context.Update(tyre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TyreIndex));
            }
            return View(tyre);
        }

        public async Task<IActionResult> StrategyIndex()
        {
            var strategies = await _context.Strategies
                .Include(s => s.Tyres)
                    .ThenInclude(st => st.Tyre)
                .OrderBy(s => s.RaceLen)
                .ToListAsync();
            return View(strategies);
        }

        public async Task<IActionResult> StrategyCreate(int id = 0)
        {
            var tyres = await _context.Tyres
                .OrderBy(t => t.StintLen)
                .ToListAsync();
            var strategy = await _context.Strategies
                .Include(s => s.Tyres)
                    .ThenInclude(st => st.Tyre)
                .SingleOrDefaultAsync(s => s.StrategyId == id);
            if (strategy is null)
            {
                var viewmodel = new CreateStrategyModel(0, 0, null, tyres);
                return View(viewmodel);
            }
            else
            {
                var viewmodel = new CreateStrategyModel(strategy.StrategyId ,strategy.RaceLen, strategy.Tyres, tyres);
                return View(viewmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StrategyCreate(int strategyId, int tyreId, int raceLen, int applyNum)
        {
            var _strategy = await _context.Strategies.FirstOrDefaultAsync(s => s.StrategyId == strategyId);
            var tyre = await _context.Tyres.FindAsync(tyreId);
            if (tyre is null)
                return NotFound();

            Strategy strategy;
            if (_strategy is null)
            {
                strategy = new Strategy
                {
                    RaceLen = raceLen
                };
                await _context.AddAsync(strategy);
            }
            else
            {
                strategy = _strategy;
                strategy.RaceLen = raceLen;
                _context.Update(strategy);
            }

            TyreStrategy tyreStrategy = new TyreStrategy { Strategy = strategy, Tyre = tyre, StintNumberApplied = applyNum };
            await _context.AddAsync(tyreStrategy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StrategyCreate), new { id = strategy.StrategyId });
        }

        public async Task<IActionResult> RemoveTyreStrategy(int tyreStratId, int strategyId)
        {
            TyreStrategy tyreToRemove = await _context.TyreStrategies.FindAsync(tyreStratId);

            if (tyreToRemove is null)
                return NotFound();

            _context.Remove(tyreToRemove);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StrategyCreate), new { id = strategyId });
        }
    }
}
