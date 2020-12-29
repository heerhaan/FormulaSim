using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITraitService
    {
        IQueryable<Trait> GetTraitsQuery();
        Task<IList<Trait>> GetTraits();
        Task<Trait> GetTraitById(int id);
        Task<Trait> FirstOrDefault(Expression<Func<Trait, bool>> predicate);
        Task Add(Trait trait);
        void Update(Trait trait);
        Task<IList<Trait>> GetUnusedTraitsFromEntity(TraitGroup group, List<int> usedTraits);
        Task<IList<Trait>> GetTraitsFromDriver(int driverId);
        Task AddTraitToDriver(Driver driver, Trait trait);
        Task RemoveTraitFromDriver(Driver driver, Trait trait);
        Task AddTraitToTeam(Team team, Trait trait);
        Task AddTraitToTrack(Track track, Trait trait);
        Task SaveChangesAsync();
    }

    public class TraitService : ITraitService
    {
        private readonly FormulaContext _context;
        private DbSet<Trait> Data { get; }

        public TraitService(FormulaContext context)
        {
            _context = context;
            Data = _context.Set<Trait>();
        }

        public IQueryable<Trait> GetTraitsQuery()
        {
            return Data;
        }

        public async Task<IList<Trait>> GetTraits()
        {
            var traits = await Data.AsNoTracking()
                .ToListAsync();
            return traits;
        }

        public async Task<Trait> GetTraitById(int id)
        {
            var item = await Data.AsNoTracking()
                .FirstOrDefaultAsync(res => res.TraitId == id);
            return item;
        }

        public async Task<Trait> FirstOrDefault(Expression<Func<Trait, bool>> predicate)
        {
            return await Data.AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task Add(Trait trait)
        {
            await Data.AddAsync(trait);
        }

        public void Update(Trait trait)
        {
            Data.Update(trait);
        }

        // To return the unused traits from an entity the corresponding group needs to be given as a parameter and
        // also a list with id's of used traits
        public async Task<IList<Trait>> GetUnusedTraitsFromEntity(TraitGroup group, List<int> usedTraits)
        {
            var traits = await Data.AsNoTracking()
                .Where(res => res.TraitGroup == group && !usedTraits.Any(used => used == res.TraitId))
                .OrderBy(res => res.Name)
                .ToListAsync();
            return traits;
        }

        public async Task<IList<Trait>> GetTraitsFromDriver(int driverId)
        {
            var traits = await _context.DriverTraits
                .AsNoTracking()
                .Where(res => res.DriverId == driverId)
                .Select(res => res.Trait)
                .ToListAsync();
            return traits;
        }

        public async Task AddTraitToDriver(Driver driver, Trait trait)
        {
            DriverTrait newTrait = new DriverTrait { Driver = driver, Trait = trait };
            await _context.AddAsync(newTrait);
        }

        public async Task RemoveTraitFromDriver(Driver driver, Trait trait)
        {
            var removingTrait = await _context.DriverTraits.FirstOrDefaultAsync(res => res.Driver == driver && res.Trait == trait);
            _context.Remove(removingTrait);
        }

        public async Task AddTraitToTeam(Team team, Trait trait)
        {
            TeamTrait newTrait = new TeamTrait { Team = team, Trait = trait };
            await _context.AddAsync(newTrait);
        }

        public async Task AddTraitToTrack(Track track, Trait trait)
        {
            TrackTrait newTrait = new TrackTrait { Track = track, Trait = trait };
            await _context.AddAsync(newTrait);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
