using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public enum Style { Aggressive, Neutral, Defensive}
    public enum Tires { Hard, Soft}
    public class SeasonDriver
    {
        [Key]
        public int SeasonDriverId { get; set; }
        public int Skill { get; set; }
        [EnumDataType(typeof(Style))]
        public Style Style { get; set; }
        [EnumDataType(typeof(Tires))]
        public Tires Tires { get; set; }
        public int Points { get; set; }

        public int DriverId { get; set; }
        public Driver Drivers { get; set; }
        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }

        public virtual ICollection<DriverResult> DriverResults { get; set; }
    }
}
