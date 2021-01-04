using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class QualifyingBattleModel
    {
        public Dictionary<int, int> QualyBattles { get; set; }
        public IEnumerable<SeasonTeam> Teams { get; set; }
        public int SeasonId { get; set; }
    }
}
