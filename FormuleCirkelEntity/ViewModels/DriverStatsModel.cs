using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverStatsModel
    {
        public Driver Driver { get; set;}
        public IEnumerable<SeasonDriver> SeasonDriver { get; set; }
        public IEnumerable<DriverResult> DriverResults { get; set; }
    }
}
