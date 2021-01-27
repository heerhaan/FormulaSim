using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ISeasonService
    {
        IQueryable<Season> GetSeasonsQuery();
        Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false);
        Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false);
        Task<Season> FirstOrDefault(Expression<Func<Season, bool>> predicate);
        Task Add(Season season);
        void Update(Season season);
        Task SaveChangesAsync();
    }

    public class SeasonService : ISeasonService
    {
        private readonly FormulaContext _context;
        private DbSet<Season> Data { get; }

        public SeasonService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<Season>();
        }

        public IQueryable<Season> GetSeasonsQuery()
        {
            return Data;
        }

        public async Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(activeChamp, res => res.Where(s => s.Championship.ActiveChampionship))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(withRace, res => res.Include(s => s.Races))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .FirstOrDefaultAsync(res => res.SeasonId == id)
                .ConfigureAwait(false);
        }

        public async Task<Season> FirstOrDefault(Expression<Func<Season, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate).ConfigureAwait(false);
        }

        public async Task Add(Season season)
        {
            await Data.AddAsync(season).ConfigureAwait(false);
        }

        public void Update(Season season)
        {
            Data.Update(season);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
