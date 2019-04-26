using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public enum Style { Agressief, Neutraal, Defensief}
    public enum Tires { Hard, Zacht}
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
        public Driver Driver { get; set; }

        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public virtual ICollection<DriverResult> DriverResults { get; set; }
    }
}
