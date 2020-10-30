using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class HomeTeamStandingsModel
    {
        public IEnumerable<SeasonTeam> SeasonTeams { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public IEnumerable<int> Rounds { get; set; }
        public IEnumerable<DriverResult> DriverResults { get; set; }
        public int LastPointPos { get; set; }
        public int SeasonId { get; set; }
        public int Year { get; set; }
        public string Points { get; set; }
        public int PolePoints { get; set; }
    }
}
