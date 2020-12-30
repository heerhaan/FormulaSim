using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITeamService : IDataService<Team>
    {
        Task<IList<Team>> GetArchivedTeams();
        Task SaveBio(int id, string biography);
    }

    public class TeamService : DataService<Team>, ITeamService
    {
        public TeamService(FormulaContext context) : base(context) { }

        public async Task<IList<Team>> GetArchivedTeams()
        {
            var teams = await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Abbreviation)
                .ToListAsync();

            return teams;
        }

        public async Task SaveBio(int id, string biography)
        {
            var team = await GetEntityById(id);
            team.Biography = biography;
            Update(team);
        }
    }
}
