using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TraitsDriverModel
    {
        public SeasonDriver Driver { get; set; }
        public IEnumerable<Trait> Traits { get; set; }
    }
}
