using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RacesRaceModel
    {
        public Race Race { get; set; }
        public IList<DriverResult> DriverResults { get; set; }
        public IList<int> Power { get; set; }
        public int SeasonId { get; set; }
        public SeasonState SeasonState { get; set; }
        public IDictionary<int, int?> PointsPerPosition { get; set; }
        public int MaxPos { get; set; }
        public int CountDrivers { get; set; }
    }
}
