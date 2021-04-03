using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class TeamStatsModel
    {
        // Information about the team
        public int TeamId { get; set; }
        public string TeamShort { get; set; }
        public string TeamBio { get; set; }
        public string TeamLong { get; set; }
        public string TeamColour { get; set; }
        public string TeamAccent { get; set; }
        public IEnumerable<string> Drivers { get; set; }

        // Statistics about the races of the team
        public decimal ConstructorTitles { get; set; }
        public decimal RaceEntries { get; set; }
        public decimal TotalCarEntries { get; set; }
        public decimal Poles { get; set; }
        public decimal RaceWins { get; set; }
        public decimal SecondFinishes { get; set; }
        public decimal ThirdFinishes { get; set; }
        public decimal PointFinishes { get; set; }
        public decimal NoPointFinishes { get; set; }
        public decimal DidNotFinish { get; set; }
        public double AveragePos { get; set; }
        public List<int> PositionList { get; } = new List<int>();
        public List<decimal> ResultList { get; } = new List<decimal>();
    }
}
