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
        public string DriverBio { get; set; }
        public IEnumerable<Team> Teams { get; set; }
        // Statistics about their races
        public int StartCount { get; set; }
        public int PointFinishCount { get; set; }
        public int OutsideCount { get; set; }
        public int WDCCount { get; set; }
        public int PoleCount { get; set; }
        public int WinCount { get; set; }
        public int PodiumCount { get; set; }
        // Statistics about their DNFs
        public int DNFCount { get; set; }
        public int DSQCount { get; set; }
        public int AccidentCount { get; set; }
        public int ContactCount { get; set; }
        public int EngineCount { get; set; }
        public int MechanicalCount { get; set; }
    }
}