using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverTraitsModel
    {
        public Driver Driver { get; set; }
        public IEnumerable<Trait> DriverTraits { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
