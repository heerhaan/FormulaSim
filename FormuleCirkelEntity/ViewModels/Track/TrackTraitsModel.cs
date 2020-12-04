using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TrackTraitsModel
    {
        public Track Track { get; set; }
        public IEnumerable<Trait> TrackTraits { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
