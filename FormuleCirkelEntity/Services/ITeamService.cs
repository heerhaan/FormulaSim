using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITeamService : IDataService<Team>
    {
        Task<List<Team>> GetTeams(bool noFilter = false);
        Task<Team> GetTeamById(int id, bool noFilter = false);
        Task<IList<Team>> GetArchivedTeams();
        Task SaveBio(int id, string biography);
    }

    public class TeamService : DataService<Team>, ITeamService
    {
        public TeamService(FormulaContext context) : base(context) { }

        public async Task<List<Team>> GetTeams(bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().ToListAsync();
        }

        public async Task<Team> GetTeamById(int id, bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<IList<Team>> GetArchivedTeams()
        {
            return await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Abbreviation)
                .ToListAsync();
        }

        public async Task SaveBio(int id, string biography)
        {
            var team = await GetTeamById(id);
            team.Biography = biography;
            Update(team);
        }
    }
}
