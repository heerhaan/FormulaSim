using FormuleCirkelEntity.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class SeasonDetailModel
    {
        public Season Season { get; set; }
        public IEnumerable<SeasonDriver> SeasonDrivers { get; set; }
        public IEnumerable<SeasonTeam> SeasonTeams { get; set; }
    }
}
