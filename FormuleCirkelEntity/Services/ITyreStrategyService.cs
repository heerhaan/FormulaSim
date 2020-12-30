using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Extensions;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITyreStrategyService
    {
        IQueryable<Tyre> GetTyresQuery();
        IQueryable<Strategy> GetStrategiesQuery();
        Task<IList<Tyre>> GetTyres(bool ordered = false);
        Task<IList<Strategy>> GetStrategies(bool ordered = false, bool includeTyres = false);
        Task<Tyre> GetTyreById(int id);
        Task<Strategy> GetStrategyById(int id, bool includeTyres = false);
        Task<TyreStrategy> GetTyreStratById(int id);
        Task<Tyre> FirstOrDefaultTyre(Expression<Func<Tyre, bool>> predicate);
        Task AddTyre(Tyre tyre);
        Task AddStrategy(Strategy strategy);
        void UpdateTyre(Tyre tyre);
        void UpdateStrategy(Strategy strategy);
        Task AddTyreToStrategy(Strategy strategy, Tyre tyre, int applyNum);
        void RemoveTyreFromStrategy(TyreStrategy strat);
        Task SaveChangesAsync();
    }

    public class TyreStrategyService : ITyreStrategyService
    {
        private readonly FormulaContext _context;
        private DbSet<Tyre> TyreData { get; }
        private DbSet<Strategy> StratData { get; }

        public TyreStrategyService(FormulaContext context)
        {
            _context = context;
            TyreData = _context.Set<Tyre>();
            StratData = _context.Set<Strategy>();
        }

        public IQueryable<Tyre> GetTyresQuery()
        {
            return TyreData;
        }

        public IQueryable<Strategy> GetStrategiesQuery()
        {
            return StratData;
        }

        public async Task<IList<Tyre>> GetTyres(bool ordered = false)
        {
            var tyres = await TyreData.AsNoTracking()
                .If(ordered, res => res.OrderBy(t => t.StintLen))
                .ToListAsync();
            return tyres;
        }

        public async Task<IList<Strategy>> GetStrategies(bool ordered = false, bool includeTyres = false)
        {
            var strategies = await StratData.AsNoTracking()
                .If(includeTyres, res => res.Include(ty => ty.Tyres)
                    .ThenInclude(ty => ty.Tyre))
                .If(ordered, res => res.OrderBy(s => s.RaceLen))
                .ToListAsync();
            return strategies;
        }

        public async Task<Tyre> GetTyreById(int id)
        {
            var item = await TyreData.AsNoTracking()
                .FirstOrDefaultAsync(res => res.Id == id);
            return item;
        }

        public async Task<Strategy> GetStrategyById(int id, bool includeTyres = false)
        {
            var item = await StratData.AsNoTracking()
                .If(includeTyres, res => res.Include(ty => ty.Tyres)
                    .ThenInclude(ty => ty.Tyre))
                .FirstOrDefaultAsync(res => res.StrategyId == id);
            return item;
        }

        public async Task<TyreStrategy> GetTyreStratById(int id)
        {
            var item = await _context.TyreStrategies
                .FirstOrDefaultAsync(res => res.TyreStrategyId == id);
            return item;
        }

        public async Task<Tyre> FirstOrDefaultTyre(Expression<Func<Tyre, bool>> predicate)
        {
            return await TyreData.AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task AddTyre(Tyre tyre)
        {
            await TyreData.AddAsync(tyre);
        }

        public async Task AddStrategy(Strategy strategy)
        {
            await StratData.AddAsync(strategy);
        }

        public void UpdateTyre(Tyre tyre)
        {
            TyreData.Update(tyre);
        }

        public void UpdateStrategy(Strategy strategy)
        {
            StratData.Update(strategy);
        }

        public async Task AddTyreToStrategy(Strategy strategy, Tyre tyre, int applyNum)
        {
            TyreStrategy newStrat = new TyreStrategy { Strategy = strategy, Tyre = tyre, StintNumberApplied = applyNum };
            await _context.AddAsync(newStrat);
        }

        public void RemoveTyreFromStrategy(TyreStrategy strat)
        {
            _context.Remove(strat);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
