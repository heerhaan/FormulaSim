using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonTraitsDriverModel
    {
        public SeasonDriver Driver { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
