using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Services
{
    public interface ITrackService : IDataService<Track>
    {
    }

    public class TrackService : DataService<Track>, ITrackService
    {
        public TrackService(FormulaContext context) : base(context) { }
    }
}
