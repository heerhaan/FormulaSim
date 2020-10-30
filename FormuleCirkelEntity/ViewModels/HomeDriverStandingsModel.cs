using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class HomeDriverStandingsModel
    {
        public IEnumerable<SeasonDriver> SeasonDrivers { get; set; }
        public IList<double> Averages { get; set; }
        public IEnumerable<Track> Tracks { get; set; }
        public IEnumerable<int> Rounds { get; set; }
        public int SeasonId { get; set; }
        public int Year { get; set; }
        public int LastPointPos { get; set; }
        public string Points { get; set; }
        public int PolePoints { get; set; }
    }
}
