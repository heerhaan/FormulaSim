﻿using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITrackService : IDataService<Track>
    {
        Task<IList<Track>> GetArchivedTracks();
    }

    public class TrackService : DataService<Track>, ITrackService
    {
        public TrackService(FormulaContext context) : base(context) { }

        public async Task<IList<Track>> GetArchivedTracks()
        {
            var tracks = await Data
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(res => res.Archived)
                .OrderBy(res => res.Country)
                .ToListAsync();

            return tracks;
        }
    }
}