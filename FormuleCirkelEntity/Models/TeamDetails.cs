using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class TeamDetails
    {
        [Key]
        public int TeamDetailId { get; set; }
        public int Chassis { get; set; }
        public int Reliability { get; set; }
        [EnumDataType(typeof(Specification))]
        public Specification Specification { get; set; }

        public int SeasonId { get; set; }
        public Seasons Seasons { get; set; }

        public ICollection<Teams> Teams { get; set; }
        public ICollection<Engines> Engines { get; set; }
    }
}
