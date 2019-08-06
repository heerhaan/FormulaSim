using FormuleCirkelEntity.Models;
using System.Collections.Generic;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverStatsModel
    {
        public Driver Driver { get; set; }
        public IEnumerable<SeasonDriver> SeasonDriver { get; set; }
        public IEnumerable<DriverResult> DriverResults { get; set; }
        public IEnumerable<Season> Seasons { get; set; }
    }
}