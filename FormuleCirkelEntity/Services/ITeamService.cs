using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITeamService : IDataService<Team>
    {
    }

    public class TeamService : DataService<Team>, ITeamService
    {
        public TeamService(FormulaContext context) : base(context) { }
    }
}
