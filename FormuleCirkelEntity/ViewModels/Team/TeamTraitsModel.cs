using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TeamTraitsModel
    {
        public Team Team { get; set; }
        public IEnumerable<Trait> TeamTraits { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
