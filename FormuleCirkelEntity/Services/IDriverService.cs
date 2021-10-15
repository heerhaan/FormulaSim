using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Extensions;
using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.ViewModels;

namespace FormuleCirkelEntity.Services
{
    public interface IDriverService : IDataService<Driver>
    {
        Task<List<Driver>> GetDrivers(bool noFilter = false);
        Task<Driver> GetDriverById(int id, bool noFilter = false);
        Task<int> GetRandomDriverId();
        Task<List<Driver>> GetArchivedDrivers();
        List<int> GetDriverChampionsIds(List<Season> seasons);
        void PrepareDriverStatsModel(DriverStatsModel stats, Driver driver, List<Season> seasons, List<DriverResult> results);
        Task<List<Team>> GetDistinctTeamsHistoryByDriver(int driverId);
        Task SaveBio(int id, string biography);
    }

    public class DriverService : DataService<Driver>, IDriverService
    {
        private readonly Random _rng = new Random();

        public DriverService(FormulaContext context) : base(context) { }

        public async Task<List<Driver>> GetDrivers(bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().ToListAsync();
        }

        public async Task<Driver> GetDriverById(int id, bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<int> GetRandomDriverId()
        {
            var drivers = await GetDrivers();
            if (drivers.Any())
            {
                return drivers[_rng.Next(drivers.Count)].Id;
            }
            else
            {
                return 0;
            }
        }

        public async Task<List<Driver>> GetArchivedDrivers()
        {
            return await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Name)
                .ToListAsync();
        }

        public List<int> GetDriverChampionsIds(List<Season> seasons)
        {
            // Null check since we really dont want to do stuff with something as empty as a null
            if (seasons is null) { return null; }
            List<int> driverIds = new List<int>();
            foreach (var season in seasons.Where(res => res.State == SeasonState.Finished))
            {
                // Get the driver with the highest amount of points
                var winner = season.Drivers
                    .OrderByDescending(res => res.Points)
                    .FirstOrDefault();
                // If the result works out correctly then the winner can be added to the list of driverIds
                if (winner != null) { driverIds.Add(winner.DriverId); }
            }
            return driverIds;
        }

        public void PrepareDriverStatsModel(DriverStatsModel stats, Driver driver, List<Season> seasons, List<DriverResult> results)
        {
            // [TODO]: How to handle possible empty parameters instead of just throwing an exception
            if (stats is null || driver is null || seasons is null || results is null)
                throw new NullReferenceException();

            // Basic information about the driver
            stats.DriverId = driver.Id;
            stats.DriverName = driver.Name;
            stats.DriverNumber = driver.DriverNumber;
            stats.DriverCountry = driver.Country;
            stats.DriverBio = driver.Biography;

            stats.StartCount = results.Count;
            for (int i = 1; i <= 20; i++)
            {
                int positionCount = results.Count(res => res.Position == i && res.Status == Status.Finished);
                stats.PositionList.Add(i);
                stats.ResultList.Add(positionCount);
            }
            stats.WinCount = results.Count(r => r.Position == 1);
            stats.SecondCount = results.Count(r => r.Position == 2);
            stats.ThirdCount = results.Count(r => r.Position == 3);
            stats.AveragePos = Math.Round(results.Where(res => res.Status == Status.Finished).Average(res => res.Position), 2);
            stats.PoleCount = results.Count(r => r.Grid == 1);
            stats.DNFCount = results.Count(r => r.Status == Status.DNF);
            stats.DSQCount = results.Count(r => r.Status == Status.DSQ);

            // Calculate point finishes
            int pointCount = 0;
            foreach (var season in seasons)
            {
                var current = results.Where(r => r.SeasonDriver.SeasonId == season.SeasonId);
                var pointsMax = season.PointsPerPosition.Keys.Max();
                pointCount += (current.Count(dr => dr.Position > 3 && dr.Position <= pointsMax));
            }

            // Count of the sort of non-finishes a driver had
            stats.AccidentCount = results.Count(r => r.DNFCause == DNFCause.Accident || r.DNFCause == DNFCause.Puncture);
            stats.ContactCount = results.Count(r => r.DNFCause == DNFCause.Damage || r.DNFCause == DNFCause.Collision);
            stats.EngineCount = results.Count(r => r.DNFCause == DNFCause.Engine);
            stats.MechanicalCount = results.Count(r => r.DNFCause == DNFCause.Brakes || r.DNFCause == DNFCause.Clutch || r.DNFCause == DNFCause.Electrics ||
                r.DNFCause == DNFCause.Exhaust || r.DNFCause == DNFCause.Hydraulics || r.DNFCause == DNFCause.Wheel);
        }

        public async Task<List<Team>> GetDistinctTeamsHistoryByDriver(int driverId)
        {
            return await Context.SeasonDrivers
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .Where(s => s.Driver.Id == driverId)
                .Include(s => s.SeasonTeam)
                    .Where(st => st.Season.Championship.ActiveChampionship)
                .Include(s => s.SeasonTeam)
                    .ThenInclude(t => t.Team)
                .Select(s => s.SeasonTeam.Team)
                .Distinct()
                .ToListAsync();
        }

        public async Task SaveBio(int id, string biography)
        {
            var driver = await GetDriverById(id);
            driver.Biography = biography;
            Update(driver);
        }
    }
}
