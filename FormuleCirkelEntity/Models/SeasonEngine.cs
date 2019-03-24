using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonEngine
    {
        [Key]
        public int SeasonEngineId { get; set; }
        public int Power { get; set; }

        public int EngineId { get; set; }
        public Engine Engine { get; set; }

        public virtual ICollection<SeasonTeam> SeasonTeams { get; set; }
    }
}
