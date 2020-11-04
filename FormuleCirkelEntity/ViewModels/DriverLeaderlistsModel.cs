using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverLeaderlistsModel
    {
#pragma warning disable CA2227 // Collection properties should be read only
        public IDictionary<Driver, int> LeaderlistTitles { get; set; }
        public IDictionary<Driver, int> LeaderlistWins { get; set; }
        public IDictionary<Driver, int> LeaderlistPodiums { get; set; }
        public IDictionary<Driver, int> LeaderlistStarts { get; set; }
        public IDictionary<Driver, int> LeaderlistNonFinishes { get; set; }
        public IDictionary<Driver, int> LeaderlistPoles { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
