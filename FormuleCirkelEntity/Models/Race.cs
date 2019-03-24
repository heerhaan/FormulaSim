using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Race
    {
        [Key]
        public int RaceId { get; set; }
        public int Round { get; set; }
        public string Name { get; set; }

        public int TrackId { get; set; }
        public Track Track { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public virtual ICollection<DriverResult> DriverResults { get; set; }
        public virtual ICollection<TeamResult> TeamResults { get; set; }
    }
}
