using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonDriver
    {
        public SeasonDriver()
        {
            Traits = new Dictionary<int, Trait>();
        }
        [Key]
        public int SeasonDriverId { get; set; }
        public int Skill { get; set; }
        public int Reliability { get; set; }
        [EnumDataType(typeof(Tire))]
        public Tire Tires { get; set; }
        [EnumDataType(typeof(DriverStatus))]
        public DriverStatus DriverStatus { get; set; }

        public int Points { get; set; }

        public int DriverId { get; set; }
        public Driver Driver { get; set; }

        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public IDictionary<int, Trait> Traits { get; set; }
        public virtual ICollection<DriverResult> DriverResults { get; set; }
    }

    public enum Tire { Hards, Softs }

    public enum DriverStatus { First, None, Second }
}
