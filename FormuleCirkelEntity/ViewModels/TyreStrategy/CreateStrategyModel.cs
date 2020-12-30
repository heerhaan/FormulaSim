using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class CreateStrategyModel
    {
        public CreateStrategyModel() { }
        public CreateStrategyModel(int strategyId, int raceLen, IList<TyreStrategy> strats, IList<Tyre> tyres)
        {
            StrategyId = strategyId;
            RaceLen = raceLen;
            if (strats != null)
            {
                foreach (var strat in strats)
                    StrategyTyres.Add(strat);
            }
            if (tyres != null)
            {
                foreach (var tyre in tyres)
                    Tyres.Add(tyre);
            }
        }
        public int StrategyId { get; set; }
        public int RaceLen { get; set; }
        public List<TyreStrategy> StrategyTyres { get; } = new List<TyreStrategy>();
        public List<Tyre> Tyres { get; } = new List<Tyre>();
    }
}
