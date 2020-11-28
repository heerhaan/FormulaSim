using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
