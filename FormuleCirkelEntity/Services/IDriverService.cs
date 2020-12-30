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
        IList<int> GetDriverChampionsIds(List<Season> seasons);
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

        public IList<int> GetDriverChampionsIds(List<Season> seasons)
        {
            // Null check since we really dont want to do stuff with something as empty as a null
            if (seasons is null) { return null; }
            List<int> driverIds = new List<int>();
            foreach (var season in seasons)
            {
                // Get the driver with the highest amount of points
                var winner = season.Drivers
                    .OrderByDescending(res => res.Points)
                    .FirstOrDefault();
                // If the result works out correctly then the winner can be added to the list of driverIds
                if (winner != null)
                    driverIds.Add(winner.DriverId);
            }
            return driverIds;
        }

        public async Task SaveBio(int id, string biography)
        {
            var driver = await GetEntityById(id);
            driver.Biography = biography;
            Update(driver);
        }
    }
}
