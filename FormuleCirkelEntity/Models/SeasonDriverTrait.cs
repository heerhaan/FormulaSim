using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class SeasonDriverTrait
    {
        public int SeasonDriverId { get; set; }
        public SeasonDriver SeasonDriver { get; set; }
        public int TraitId { get; set; }
        public Trait Trait { get; set; }
    }
}
