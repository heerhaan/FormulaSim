using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RacesModifyRaceModel
    {
        public RacesModifyRaceModel()
        {
            RaceStints = new List<Stint>();
        }
        public int SeasonId { get; set; }
        public int TrackId { get; set; }
        public List<Stint> RaceStints { get; set; }
    }
}
