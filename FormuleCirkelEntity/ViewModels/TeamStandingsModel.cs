using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class TeamStandingsModel
    {
        public IEnumerable<SeasonTeam> SeasonTeams { get; set; }
        public List<string> Locations { get; set; }
        public IEnumerable<int> Rounds { get; set; }
        public IEnumerable<DriverResult> DriverResults { get; set; }
        public int LastPointPos { get; set; }
    }
}
