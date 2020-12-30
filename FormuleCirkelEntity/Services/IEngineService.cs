using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface IEngineService : IDataService<Engine>
    {
        Task<IList<Engine>> GetArchivedEngines();
    }

    public class EngineService : DataService<Engine>, IEngineService
    {
        public EngineService(FormulaContext context) : base(context) { }

        public async Task<IList<Engine>> GetArchivedEngines()
        {
            var engines = await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Name)
                .ToListAsync();

            return engines;
        }
    }
}
