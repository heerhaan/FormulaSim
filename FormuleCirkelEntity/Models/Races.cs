using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Races
    {
        [Key]
        public int RaceId { get; set; }
        public int Round { get; set; }
        public string Name { get; set; }

        public int QualyId { get; set; }
        public Qualification Qualification { get; set; }

        public int ResultId { get; set; }
        public Results Results { get; set; }

        public int DriverStandingsId { get; set; }
        public DriverStandings DriverStandings { get; set; }

        public int TeamStandingsId { get; set; }
        public TeamStandings TeamStandings { get; set; }

        public ICollection<Seasons> Seasons { get; set; }
        public ICollection<Tracks> Tracks { get; set; }
    }
}
