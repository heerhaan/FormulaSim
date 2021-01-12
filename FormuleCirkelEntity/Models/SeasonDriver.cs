using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class SeasonDriver
    {
        [Key]
        public int SeasonDriverId { get; set; }
        public int Skill { get; set; }
        public int Reliability { get; set; }
        // Defines if a driver has #1, #2 or neutral status in a team
        [EnumDataType(typeof(DriverStatus))]
        public DriverStatus DriverStatus { get; set; }
        // Defines if a driver is dropped from his team at this moment
        public bool Dropped { get; set; }
        // How many championship points the driver has scored over the season
        public int Points { get; set; }
        // Used to determine the placed position of a driver after sorting to points
        public double HiddenPoints { get; set; }

        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public IList<DriverResult> DriverResults { get; } = new List<DriverResult>();
    }

    public enum DriverStatus { First, None, Second }
}
