using System;

namespace FormuleCirkelEntity.Models
{
    public class TeamTrait
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int TraitId { get; set; }
        public Trait Trait { get; set; }
    }
}
