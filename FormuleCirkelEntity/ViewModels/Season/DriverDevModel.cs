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
        public IDictionary<int, MinMaxDevRange> SkillDevRanges { get; } = new Dictionary<int, MinMaxDevRange>();
        public IDictionary<int, MinMaxDevRange> AgeDevRanges { get; } = new Dictionary<int, MinMaxDevRange>();
    }
}
