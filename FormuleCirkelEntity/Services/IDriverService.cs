using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverService : IDataService<Driver>
    {
        Task<IList<Driver>> GetArchivedDrivers();
        Task SaveBio(int id, string biography);
    }

    public class DriverService : DataService<Driver>, IDriverService
    {
        public DriverService(FormulaContext context) : base(context) { }

        public async Task<IList<Driver>> GetArchivedDrivers()
        {
            var drivers = await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Name)
                .ToListAsync();

            return drivers;
        }

        public async Task SaveBio(int id, string biography)
        {
            var driver = await GetEntityById(id);
            driver.Biography = biography;
            Update(driver);
        }
    }
}
