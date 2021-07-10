using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverResultService : IDataService<DriverResult>
    {
        Task<List<DriverResult>> GetAllResultsFromDriver(int driverId);
    }

    public class DriverResultService : DataService<DriverResult>, IDriverResultService
    {
        public DriverResultService(FormulaContext context) : base(context) { }

        public async Task<List<DriverResult>> GetAllResultsFromDriver(int driverId)
        {
            return await Data.AsNoTracking()
                .Where(res => res.SeasonDriver.DriverId == driverId)
                .ToListAsync();
        }
    }
}
