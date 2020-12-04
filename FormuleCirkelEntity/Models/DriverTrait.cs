using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class DriverTrait
    {
        public int DriverId { get; set; }
        public Driver Driver { get; set; }
        public int TraitId { get; set; }
        public Trait Trait { get; set; }
    }
}
