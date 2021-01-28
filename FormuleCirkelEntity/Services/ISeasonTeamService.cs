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
    public interface ISeasonTeamService : IDataService<SeasonTeam>
    {
        Task<List<SeasonTeam>> GetSeasonTeams();
        Task<SeasonTeam> GetSeasonTeamById(int id);
        Task<List<SeasonTeam>> GetRankedSeasonTeams(int seasonId, bool withDrivers = false);
    }

    public class SeasonTeamService : DataService<SeasonTeam>, ISeasonTeamService
    {
        public SeasonTeamService(FormulaContext context) : base(context) { }

        public async Task<List<SeasonTeam>> GetSeasonTeams()
        {
            return await Data.AsNoTracking().ToListAsync();
        }

        public async Task<SeasonTeam> GetSeasonTeamById(int id)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(res => res.SeasonTeamId == id);
        }

        public async Task<List<SeasonTeam>> GetRankedSeasonTeams(int seasonId, bool withDrivers = false)
        {
            return await Data.AsNoTracking().IgnoreQueryFilters()
                .Where(res => res.SeasonId == seasonId)
                .Include(st => st.Team)
                .If(withDrivers, res => res.Include(st => st.SeasonDrivers))
                .OrderByDescending(st => st.Points)
                .ToListAsync();
        }
    }
}
