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
    public interface ISeasonDriverService : IDataService<SeasonDriver>
    {
        Task<List<SeasonDriver>> GetSeasonDrivers();
        Task<SeasonDriver> GetSeasonDriverById(int id);
        Task<List<SeasonDriver>> GetRankedSeasonDrivers(int seasonId, bool withTeam = false, bool withResults = false);
        List<double> CalculateDriverAverages(List<SeasonDriver> drivers);
    }

    public class SeasonDriverService : DataService<SeasonDriver>, ISeasonDriverService
    {
        public SeasonDriverService(FormulaContext context) : base(context) { }

        public async Task<List<SeasonDriver>> GetSeasonDrivers()
        {
            return await Data.AsNoTracking().ToListAsync();
        }

        public async Task<SeasonDriver> GetSeasonDriverById(int id)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(res => res.SeasonDriverId == id);
        }

        public async Task<List<SeasonDriver>> GetRankedSeasonDrivers(int seasonId, bool withTeam = false, bool withResults = false)
        {
            return await Data.AsNoTracking().IgnoreQueryFilters()
                .Where(res => res.SeasonId == seasonId)
                .Include(res => res.Driver)
                .If(withTeam, res => res.Include(s => s.SeasonTeam).ThenInclude(st => st.Team))
                .If(withResults, res => res.Include(s => s.DriverResults))
                .OrderByDescending(res => res.Points)
                    .ThenByDescending(res => res.HiddenPoints)
                .ToListAsync();
        }

        // Calculates the average finishing position for the given season per driver
        public List<double> CalculateDriverAverages(List<SeasonDriver> drivers)
        {
            if (drivers is null) { throw new NullReferenceException(); }
            var averages = new List<double>();
            foreach (var driver in drivers)
            {
                var positions = new List<double>();
                double average = 0;
                if (driver.DriverResults.Count > 0)
                {
                    foreach (var result in driver.DriverResults)
                    {
                        if (result.Status == Status.Finished)
                        {
                            positions.Add(result.Position);
                        }
                    }
                    if (positions.Count > 0)
                    {
                        average = Math.Round(positions.Average(), 2);
                    }
                }
                averages.Add(average);
            }
            return averages;
        }
    }
}
