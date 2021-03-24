using FormuleCirkelEntity.Models;
using System.Collections;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverStatsModel
    {
        // Information about the driver
        public int DriverId { get; set; }
        public string DriverName { get; set; }
        public int DriverNumber { get; set; }
        public string DriverCountry { get; set; }
        public string DriverBio { get; set; }
        public IEnumerable<Team> Teams { get; set; }

        // Statistics about their races
        public decimal StartCount { get; set; }
        public decimal WDCCount { get; set; }
        public decimal PoleCount { get; set; }
        public decimal WinCount { get; set; }
        public decimal SecondCount { get; set; }
        public decimal ThirdCount { get; set; }
        public double AveragePos { get; set; }
        public List<int> PositionList { get; } = new List<int>();
        public List<decimal> ResultList { get; } = new List<decimal>();

        // Statistics about their DNFs
        public decimal DNFCount { get; set; }
        public decimal DSQCount { get; set; }
        public decimal AccidentCount { get; set; }
        public decimal ContactCount { get; set; }
        public decimal EngineCount { get; set; }
        public decimal MechanicalCount { get; set; }
    }
}