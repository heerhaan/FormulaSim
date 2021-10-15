using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels.Season
{
    public class SeasonIndexModel
    {
        public string ChampionshipName { get; set; }
        public Dictionary<int, string> AllChampionships { get; set; }
        public SeasonIndexList[] SeasonIndex { get; set; }
    }

    public class SeasonIndexList
    {
        public int SeasonID { get; set; }
        public int SeasonNumber { get; set; }
        public string TopDriverName { get; set; }
        public string TopDriverCountry { get; set; }
        public string TopDriverTeamColour { get; set; }
        public string TopDriverTeamAccent { get; set; }
        public string TopTeamName { get; set; }
        public string TopTeamCountry { get; set; }
        public string TopTeamColour { get; set; }
        public string TopTeamAccent { get; set; }

        public bool HasTop { get { return TopDriverName != null && TopTeamName != null; } }
    }
}
