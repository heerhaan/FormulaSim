using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TrackTraitsTrackModel
    {
        public Track Track { get; set; }
        public IEnumerable<TrackTrait> TrackTraits { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
