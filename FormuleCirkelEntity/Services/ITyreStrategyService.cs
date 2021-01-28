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
    public interface ITyreStrategyService : IDataService<TyreStrategy>
    {
        IQueryable<Tyre> GetTyresQuery();
        IQueryable<Strategy> GetStrategiesQuery();
        Task<List<Tyre>> GetTyres(bool ordered = false);
        Task<List<Strategy>> GetStrategies(bool ordered = false, bool includeTyres = false);
        Task<Tyre> GetTyreById(int id);
        Task<Strategy> GetStrategyById(int id, bool includeTyres = false);
        Task<TyreStrategy> GetTyreStratById(int id, bool includeTyresAndStrat = false);
        Task<Tyre> FirstOrDefaultTyre(Expression<Func<Tyre, bool>> predicate);
        Task AddTyre(Tyre tyre);
        Task AddStrategy(Strategy strategy);
        void UpdateTyre(Tyre tyre);
        void UpdateStrategy(Strategy strategy);
        Task AddTyreToStrategy(Strategy strategy, Tyre tyre, int applyNum);
        void RemoveTyreFromStrategy(TyreStrategy strat, Strategy strategy);
    }

    public class TyreStrategyService : DataService<TyreStrategy>, ITyreStrategyService
    {
        private DbSet<Tyre> TyreData { get; }
        private DbSet<Strategy> StratData { get; }

        public TyreStrategyService(FormulaContext context) : base(context)
        {
            TyreData = Context.Set<Tyre>();
            StratData = Context.Set<Strategy>();
        }

        public IQueryable<Tyre> GetTyresQuery()
        {
            return TyreData;
        }

        public IQueryable<Strategy> GetStrategiesQuery()
        {
            return StratData;
        }

        public async Task<List<Tyre>> GetTyres(bool ordered = false)
        {
            return await TyreData.AsNoTracking()
                .If(ordered, res => res.OrderBy(t => t.StintLen))
                .ToListAsync();
        }

        public async Task<List<Strategy>> GetStrategies(bool ordered = false, bool includeTyres = false)
        {
            return await StratData.AsNoTracking()
                .If(includeTyres, res => res.Include(ty => ty.Tyres)
                    .ThenInclude(ty => ty.Tyre))
                .If(ordered, res => res.OrderBy(s => s.RaceLen))
                .ToListAsync();
        }

        public async Task<Tyre> GetTyreById(int id)
        {
            return await TyreData.AsNoTracking()
                .FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<Strategy> GetStrategyById(int id, bool includeTyres = false)
        {
            return await StratData
                .AsNoTracking()
                .If(includeTyres, res => res.Include(ty => ty.Tyres)
                    .ThenInclude(ty => ty.Tyre))
                .FirstOrDefaultAsync(res => res.StrategyId == id);
        }

        public async Task<TyreStrategy> GetTyreStratById(int id, bool includeTyresAndStrat = false)
        {
            // Returns the tyrestrategy object, if a true bool is given then it includes the corresponding tyre and strategy
            return await Context.TyreStrategies
                .AsNoTracking()
                .If(includeTyresAndStrat, res => res.Include(tr => tr.Strategy))
                .If(includeTyresAndStrat, res => res.Include(tr => tr.Tyre))
                .FirstOrDefaultAsync(res => res.TyreStrategyId == id);
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

        // TyreStrategy adds a new strategy option to an existing possible strategy for a race
        // <param name="applyNum">Int value that represents in which stint the new strategy option is applied</param>
        public async Task AddTyreToStrategy(Strategy strategy, Tyre tyre, int applyNum)
        {
            TyreStrategy newStrat = new TyreStrategy { Strategy = strategy, Tyre = tyre, StintNumberApplied = applyNum };
            await Add(newStrat);
        }

        public void RemoveTyreFromStrategy(TyreStrategy strat, Strategy strategy)
        {
            if (strat is null || strategy is null) { throw new NullReferenceException(); }
            // Subtract the usable length of the tyre from the total length of the strategy
            strategy.RaceLen -= strat.Tyre.StintLen;
            // Then the tyrestrategy object can be fully deleted
            Context.Remove(strat);
        }
    }
}
