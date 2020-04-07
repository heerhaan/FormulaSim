using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonDetailModel
    {
        public Season Season { get; set; }
        public IEnumerable<SeasonDriver> SeasonDrivers { get; set; }
        public IEnumerable<SeasonTeam> SeasonTeams { get; set; }
    }
}
