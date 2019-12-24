using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TraitsTeamModel
    {
        public SeasonTeam Team { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
