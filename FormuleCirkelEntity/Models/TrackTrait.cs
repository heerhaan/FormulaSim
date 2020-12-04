using System;

namespace FormuleCirkelEntity.Models
{
    public class TrackTrait
    {
        public int TrackId { get; set; }
        public Track Track { get; set; }
        public int TraitId { get; set; }
        public Trait Trait { get; set; }
    }
}
