using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
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
        Task<IList<Season>> GetSeasons();
        Task<Season> GetSeasonById(int id);
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

        public async Task<IList<Season>> GetSeasons()
        {
            var seasons = await Data.AsNoTracking().ToListAsync();
            return seasons;
        }

        public async Task<Season> GetSeasonById(int id)
        {
            var item = await Data.AsNoTracking()
                .FirstOrDefaultAsync(res => res.SeasonId == id);
            return item;
        }

        public async Task<Season> FirstOrDefault(Expression<Func<Season, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task Add(Season season)
        {
            await Data.AddAsync(season);
        }

        public void Update(Season season)
        {
            Data.Update(season);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
