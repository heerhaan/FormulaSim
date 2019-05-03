using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class TeamResult
    {
        public int TeamResultId { get; set; }
        public int EarnedPoints { get; set; }

        public int RaceId { get; set; }
        public Race Race { get; set; }
        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }
    }
}
