using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class RaceWeekendModel
    {
        public int Year { get; set; }
        public Race Race { get; set; }
        public IEnumerable<DriverResult> DriverResults { get; set; }
    }
}
