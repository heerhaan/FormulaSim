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
    public interface ISeasonService : IDataService<Season>
    {
        Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false);
        Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false);
        Task<Season> FindActiveSeason(bool withRaces = false);
    }

    public class SeasonService : DataService<Season>, ISeasonService
    {
        public SeasonService(FormulaContext context) : base(context) { }

        public async Task<List<Season>> GetSeasons(bool activeChamp = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(activeChamp, res => res.Where(s => s.Championship.ActiveChampionship))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .ToListAsync();
        }

        public async Task<Season> GetSeasonById(int id, bool withRace = false, bool withDriver = false)
        {
            return await Data.AsNoTracking()
                .If(withRace, res => res.Include(s => s.Races))
                .If(withDriver, res => res.Include(s => s.Drivers))
                .FirstOrDefaultAsync(res => res.SeasonId == id);
        }

        public async Task<Season> FindActiveSeason(bool withRaces = false)
        {
            return await Data.AsNoTracking()
                .If(withRaces, res => res.Include(s => s.Races))
                .FirstOrDefaultAsync(res => res.Championship.ActiveChampionship && res.State == SeasonState.Progress);
        }
    }
}
