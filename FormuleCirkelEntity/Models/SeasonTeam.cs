using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class SeasonTeam
    {
        [Key]
        public int SeasonTeamId { get; set; }
        public string Name { get; set; }
        public string Principal { get; set; }
        [StringLength(7)]
        public string Colour { get; set; }
        [StringLength(7)]
        public string Accent { get; set; }
        public int Chassis { get; set; }
        public int Reliability { get; set; }
        public int Points { get; set; }
        public int Topspeed { get; set; }
        public int Acceleration { get; set; }
        public int Handling { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int EngineId { get; set; }
        public Engine Engine { get; set; }

        public IList<SeasonDriver> SeasonDrivers { get; } = new List<SeasonDriver>();
    }
}
