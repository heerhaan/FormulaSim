using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class TeamLeaderlistsModel
    {
        public IDictionary<Team, int> LeaderlistTitles { get; set; }
        public IDictionary<Team, int> LeaderlistWins { get; set; }
        public IDictionary<Team, int> LeaderlistPodiums { get; set; }
        public IDictionary<Team, int> LeaderlistStarts { get; set; }
        public IDictionary<Team, int> LeaderlistNonFinishes { get; set; }
        public IDictionary<Team, int> LeaderlistPoles { get; set; }
    }
}
