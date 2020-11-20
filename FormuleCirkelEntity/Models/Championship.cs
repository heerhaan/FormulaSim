using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Championship
    {
        [Key]
        public int ChampionshipId { get; set; }
        public string ChampionshipName { get; set; }
        public bool ActiveChampionship { get; set; }

        public IDictionary<int, MinMaxDevRange> SkillDevRanges { get; } = new Dictionary<int, MinMaxDevRange>();
        public IDictionary<int, MinMaxDevRange> AgeDevRanges { get; } = new Dictionary<int, MinMaxDevRange>();

        public virtual IList<Season> Seasons { get; set; }
    }
}
