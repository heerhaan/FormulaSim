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
    public interface ITraitService : IDataService<Trait>
    {
        Task<List<Trait>> GetTraits();
        Task<Trait> GetTraitById(int id);
        Task<List<Trait>> GetUnusedTraitsFromEntity(TraitGroup group, List<int> usedTraits);
        Task<List<Trait>> GetTraitsFromDriver(int driverId);
        Task AddTraitToDriver(Driver driver, Trait trait);
        Task RemoveTraitFromDriver(Driver driver, Trait trait);
        Task AddTraitToTeam(Team team, Trait trait);
        Task AddTraitToTrack(Track track, Trait trait);
    }

    public class TraitService : DataService<Trait>, ITraitService
    {
        public TraitService(FormulaContext context) : base(context) { }

        public async Task<List<Trait>> GetTraits()
        {
            return await Data.AsNoTracking()
                .ToListAsync();
        }

        public async Task<Trait> GetTraitById(int id)
        {
            return await Data.AsNoTracking()
                .FirstOrDefaultAsync(res => res.TraitId == id);
        }

        // To return the unused traits from an entity the corresponding group needs to be given as a parameter and
        // also a list with id's of used traits
        public async Task<List<Trait>> GetUnusedTraitsFromEntity(TraitGroup group, List<int> usedTraits)
        {
            return await Data.AsNoTracking()
                .Where(res => res.TraitGroup == group && !usedTraits.Any(used => used == res.TraitId))
                .OrderBy(res => res.Name)
                .ToListAsync();
        }

        public async Task<List<Trait>> GetTraitsFromDriver(int driverId)
        {
            return await Context.DriverTraits
                .AsNoTracking()
                .Where(res => res.DriverId == driverId)
                .Select(res => res.Trait)
                .ToListAsync();
        }

        public async Task AddTraitToDriver(Driver driver, Trait trait)
        {
            DriverTrait newTrait = new DriverTrait { Driver = driver, Trait = trait };
            await Context.AddAsync(newTrait);
        }

        public async Task RemoveTraitFromDriver(Driver driver, Trait trait)
        {
            var removingTrait = await Context.DriverTraits.FirstOrDefaultAsync(res => res.Driver == driver && res.Trait == trait);
            Context.Remove(removingTrait);
        }

        public async Task AddTraitToTeam(Team team, Trait trait)
        {
            TeamTrait newTrait = new TeamTrait { Team = team, Trait = trait };
            await Context.AddAsync(newTrait);
        }

        public async Task AddTraitToTrack(Track track, Trait trait)
        {
            TrackTrait newTrait = new TrackTrait { Track = track, Trait = trait };
            await Context.AddAsync(newTrait);
        }
    }
}
