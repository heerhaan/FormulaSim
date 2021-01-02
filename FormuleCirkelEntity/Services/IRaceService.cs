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
    public interface IRaceService
    {
        IQueryable<Race> GetRaceQuery();
        Task<IList<Race>> GetRacesAsync();
        Task<Race> GetRaceByIdAsync(int id, bool withStints = false);
        Task<Race> FirstOrDefaultAsync(Expression<Func<Race, bool>> predicate);
        Task AddAsync(Race race);
        void Update(Race race);
        Task<Race> GetLastRace(int championshipId, int trackId);
        IQueryable<DriverResult> GetDriverResultQuery();
        Task SaveChangesAsync();
    }

    public class RaceService : IRaceService
    {
        private readonly FormulaContext _context;
        private DbSet<Race> Data { get; }

        public RaceService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<Race>();
        }

        public IQueryable<Race> GetRaceQuery()
        {
            return Data;
        }

        public async Task<IList<Race>> GetRacesAsync()
        {
            var races = await Data.AsNoTracking().ToListAsync();
            return races;
        }

        public async Task<Race> GetRaceByIdAsync(int id, bool withStints = false)
        {
            var race = await Data.AsNoTracking()
                .If(withStints, res => res.Include(r => r.Stints))
                .FirstOrDefaultAsync(res => res.RaceId == id);
            return race;
        }

        public async Task<Race> FirstOrDefaultAsync(Expression<Func<Race, bool>> predicate)
        {
            return await Data.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(Race race)
        {
            await Data.AddAsync(race);
        }

        public void Update(Race race)
        {
            Data.Update(race);
        }

        public async Task<Race> GetLastRace(int championshipId, int trackId)
        {
            var lastRace = await Data
                .AsNoTracking()
                .Where(r => r.Season.ChampionshipId == championshipId && r.TrackId == trackId)
                .Include(r => r.Stints)
                .Include(r => r.Track)
                .OrderByDescending(r => r.RaceId)
                .FirstOrDefaultAsync();
            return lastRace;
        }

        public IQueryable<DriverResult> GetDriverResultQuery()
        {
            return _context.DriverResults;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public static void SetDriverTraitMods(DriverResult driver, IEnumerable<DriverTrait> driverTraits, Weather weather)
        {
            // Null-check, since I don't like warnings
            if (driver is null || driverTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits a driver may have and adds them to the driverresult modifiers
            foreach (var trait in driverTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait, weather);
            }
        }

        public static void SetTeamTraitMods(DriverResult driver, IEnumerable<TeamTrait> teamTraits, Weather weather)
        {
            // Null-check, since I don't like warnings
            if (driver is null || teamTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits the team of a driver may have and adds them to the driverresult modifiers
            foreach (var trait in teamTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait, weather);
            }
        }

        public static void SetTrackTraitMods(DriverResult driver, List<TrackTrait> trackTraits, Weather weather)
        {
            // Null-check, since I don't like warnings
            if (driver is null || trackTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits a track may have and adds them to the driverresult modifiers
            foreach (var trait in trackTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait, weather);
            }
        }

        private static void SetIndividualTraitMod(DriverResult driver, Trait trait, Weather weather)
        {
            // Checks if we currenly are dealing with wet weather
            bool isWet = (weather == Weather.Rain || weather == Weather.Storm);
            if (trait.WetWeatherPace.HasValue)
            {
                // WetWeatherPace being filled indicates that this is a wet weather trait, so if it isn't wet the rest of the method isn't ran
                if (isWet)
                    driver.DriverRacePace += trait.WetWeatherPace.Value;
                else
                    return;
            }

            if (trait.QualyPace.HasValue)
                driver.QualyMod += trait.QualyPace.Value;

            if (trait.DriverRacePace.HasValue)
                driver.DriverRacePace += trait.DriverRacePace.Value;

            if (trait.ChassisRacePace.HasValue)
                driver.ChassisRacePace += trait.ChassisRacePace.Value;

            if (trait.EngineRacePace.HasValue)
                driver.EngineRacePace += trait.EngineRacePace.Value;

            if (trait.ChassisReliability.HasValue)
                driver.ChassisRelMod += trait.ChassisReliability.Value;

            if (trait.DriverReliability.HasValue)
                driver.DriverRelMod += trait.DriverReliability.Value;

            if (trait.MaximumRNG.HasValue)
                driver.MaxRNG += trait.MaximumRNG.Value;

            if (trait.MinimumRNG.HasValue)
                driver.MinRNG += trait.MinimumRNG.Value;

            if (trait.MaxTyreWear.HasValue)
                driver.MaxTyreWear += trait.MaxTyreWear.Value;

            if (trait.MinTyreWear.HasValue)
                driver.MinTyreWear += trait.MinTyreWear.Value;
        }

        public static void SetRandomStrategy(DriverResult driverRes, Strategy strategy)
        {
            if (driverRes is null || strategy is null) { throw new NullReferenceException(); }

            var currentTyre = strategy.Tyres.Single(t => t.StintNumberApplied == 1).Tyre;
            driverRes.Strategy = strategy;
            driverRes.CurrTyre = currentTyre;
            driverRes.TyreLife = currentTyre.Pace;
        }
    }
}
