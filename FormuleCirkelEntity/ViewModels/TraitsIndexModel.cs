using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TraitsIndexModel
    {
        public IEnumerable<Trait> DriverTraits { get; set; }
        public IEnumerable<Trait> TeamTraits { get; set; }
        public IEnumerable<Trait> TrackTraits { get; set; }
    }
}
