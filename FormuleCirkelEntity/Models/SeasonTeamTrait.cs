using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonTeamTrait
    {
        public int SeasonTeamId { get; set; }
        public SeasonTeam SeasonTeam { get; set; }
        public int TraitId { get; set; }
        public Trait Trait { get; set; }
    }
}
