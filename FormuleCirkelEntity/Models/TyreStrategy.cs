using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class TyreStrategy
    {
        public int TyreStrategyId { get; set; }

        public int StrategyId { get; set; }
        public Strategy Strategy { get; set; }
        public int TyreId { get; set; }
        public Tyre Tyre { get; set; }

        public int StintNumberApplied { get; set; }
    }
}
