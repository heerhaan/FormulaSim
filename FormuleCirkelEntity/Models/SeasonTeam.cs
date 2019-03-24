﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonTeam
    {
        public int SeasonTeamId { get; set; }
        public int Chassis { get; set; }
        public int Relability { get; set; }
        [EnumDataType(typeof(Specification))]
        public Specification Specification { get; set; }
        public int Points { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int SeasonEngineId { get; set; }
        public SeasonEngine SeasonEngine { get; set; }

        public virtual ICollection<SeasonDriver> SeasonDrivers { get; set; }
        public virtual ICollection<TeamResult> TeamResults { get; set; }
    }
}
