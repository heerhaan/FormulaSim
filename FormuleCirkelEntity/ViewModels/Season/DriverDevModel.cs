using System;
using System.Collections.Generic;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.ViewModels
{
    public class DriverDevModel
    {
        public IEnumerable<SeasonDriver> SeasonDrivers { get; set; }
        public int SeasonId { get; set; }
        public int Year { get; set; }
        public IList<MinMaxDevRange> SkillDevRanges { get; } = new List<MinMaxDevRange>();
        public IList<MinMaxDevRange> AgeDevRanges { get; } = new List<MinMaxDevRange>();
    }
}
