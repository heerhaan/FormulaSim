using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class TeamStandings
    {
        [Key]
        public int TeamStandingsId { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }

        public ICollection<Races> Races { get; set; }
    }
}
