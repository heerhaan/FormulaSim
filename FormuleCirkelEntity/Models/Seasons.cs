using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Seasons
    {
        [Key]
        public int SeasonId { get; set; }

        public int RaceId { get; set; }
        public Races Races { get; set; }

        public ICollection<TeamDetails> TeamDetails { get; set; }
        public ICollection<EngineDetails> EngineDetails { get; set; }
        public ICollection<DriverDetails> DriverDetails { get; set; }
    }
}
