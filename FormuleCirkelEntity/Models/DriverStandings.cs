using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class DriverStandings
    {
        [Key]
        public int DriverStandingsId { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }

        public ICollection<Races> Races { get; set; }
    }
}
