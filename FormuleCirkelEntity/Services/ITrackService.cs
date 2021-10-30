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
    public interface ITrackService : IDataService<Track>
    {
        Task<List<Track>> GetTracks(bool noFilter = false);
        Task<Track> GetTrackById(int id, bool noFilter = false);
        Task<IList<Track>> GetArchivedTracks();
        Task<IList<Track>> GetUnusedTracks(List<int> usedTrackIds);
        Task<Specification> GetTrackSpecification(int trackId);
    }

    public class TrackService : DataService<Track>, ITrackService
    {
        public TrackService(FormulaContext context) : base(context) { }

        public async Task<List<Track>> GetTracks(bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters()).AsNoTracking().ToListAsync();
        }

        public async Task<Track> GetTrackById(int id, bool noFilter = false)
        {
            return await Data.If(noFilter, res => res.IgnoreQueryFilters())
                .AsNoTracking().FirstOrDefaultAsync(res => res.Id == id);
        }

        public async Task<IList<Track>> GetArchivedTracks()
        {
            return await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Country)
                .ToListAsync();
        }

        public async Task<IList<Track>> GetUnusedTracks(List<int> usedTrackIds)
        {
            return await Data
                .AsNoTracking()
                .Where(t => !usedTrackIds.Contains(t.Id))
                .OrderBy(t => t.Location)
                .ToListAsync();
        }

        public async Task<Specification> GetTrackSpecification(int trackId)
        {
            var track = await Data.AsNoTracking().FirstOrDefaultAsync(res => res.Id == trackId);
            return track.Specification;
        }
    }
}
