using System.Collections.Generic;

namespace FormuleCirkelEntity.Models
{
    public class Strategy
    {
        public int StrategyId { get; set; }
        public int RaceLen { get; set; }

        public IList<TyreStrategy> Tyres { get; } = new List<TyreStrategy>();
    }
}
