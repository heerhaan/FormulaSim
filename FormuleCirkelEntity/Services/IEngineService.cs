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
    public interface IEngineService : IDataService<Engine>
    {
        Task<List<Engine>> GetEngines(bool noFilter = false);
        Task<Engine> GetEngineById(int id, bool noFilter = false);
        Task<List<Engine>> GetArchivedEngines();
    }

    public class EngineService : DataService<Engine>, IEngineService
    {
        public EngineService(FormulaContext context) : base(context) { }

        public async Task<List<Engine>> GetEngines(bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().ToListAsync();
        }

        public async Task<Engine> GetEngineById(int id, bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<List<Engine>> GetArchivedEngines()
        {
            return await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Name)
                .ToListAsync();
        }
    }
}
