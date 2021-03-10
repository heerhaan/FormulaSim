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
    public interface IRubberService : IDataService<Rubber>
    {
        Task<List<Rubber>> GetRubbers(bool noFilter = false);
        Task<Rubber> GetRubberById(int id, bool noFilter = false);
        Task<List<Rubber>> GetArchivedRubbers();
    }

    public class RubberService : DataService<Rubber>, IRubberService
    {
        public RubberService(FormulaContext context) : base(context) { }

        public async Task<List<Rubber>> GetRubbers(bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().ToListAsync();
        }

        public async Task<Rubber> GetRubberById(int id, bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().FirstOrDefaultAsync(res => res.RubberId == id);
        }

        public async Task<List<Rubber>> GetArchivedRubbers()
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
