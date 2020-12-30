using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RacesModifyRaceModel
    {
        public int SeasonId { get; set; }
        public int TrackId { get; set; }
        public string TrackName { get; set; }
        public IList<Stint> RaceStints { get; } = new List<Stint>();
    }
}
