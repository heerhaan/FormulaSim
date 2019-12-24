using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TraitsTrackModel
    {
        public Track Track { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
