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
        [EnumDataType(typeof(Tire))]
        public Tire Tires { get; set; }
        [EnumDataType(typeof(DriverStatus))]
        public DriverStatus DriverStatus { get; set; }
        public bool Dropped { get; set; }

        public int Points { get; set; }

        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public IList<DriverResult> DriverResults { get; } = new List<DriverResult>();
    }

    public enum Tire { Hards, Softs }

    public enum DriverStatus { First, None, Second }
}
