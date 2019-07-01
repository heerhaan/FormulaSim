using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonDriver
    {
        [Key]
        public int SeasonDriverId { get; set; }
        public int Skill { get; set; }

        [EnumDataType(typeof(Style))]
        public Style Style { get; set; }
        [EnumDataType(typeof(Tires))]
        public Tires Tires { get; set; }
        [EnumDataType(typeof(DriverStatus))]
        public DriverStatus DriverStatus { get; set; }

        public int Points { get; set; }

        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public virtual ICollection<DriverResult> DriverResults { get; set; }
    }

    public enum Style
    {
        Aggressive = 0,
        Neutral = 1,
        Defensive = 2
    }

    public enum Tires { Hards, Softs }

    public enum DriverStatus { First, None, Second }
}
