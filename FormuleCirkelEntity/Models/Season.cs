using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Season
    {
        [Key]
        public int SeasonId { get; set; }
        public bool CurrentSeason { get; set; }

        public virtual IList<Race> Races { get; set; }
        public virtual IList<SeasonDriver> Drivers { get; set; }
        public virtual IList<SeasonTeam> Teams { get; set; }
        public virtual IList<SeasonEngine> Engines { get; set; }
    }
}
