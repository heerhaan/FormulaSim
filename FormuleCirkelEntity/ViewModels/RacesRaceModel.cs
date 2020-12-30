using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RacesRaceModel
    {
        public int RaceId { get; set; }
        public int SeasonId { get; set; }
        public string Weather { get; set; }
        public IDictionary<int, int?> PointsPerPosition { get; set; }
        public int MaxPos { get; set; }
        public int CountDrivers { get; set; }
        public string FullRaceTitle { get; set; }
        public bool ShowRaceButtons { get; set; }
        public bool IsAdmin { get; set; }
        public IList<Stint> RaceStints { get; } = new List<Stint>();
        public IList<DriverResult> DriverResults { get; } = new List<DriverResult>();
        public IList<int> Power { get; } = new List<int>();
    }
}
