using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Championship
    {
        [Key]
        public int ChampionshipId { get; set; }
        public string ChampionshipName { get; set; }
        public bool ActiveChampionship { get; set; }

        public IList<MinMaxDevRange> SkillDevRanges { get; } = new List<MinMaxDevRange>();
        public IList<MinMaxDevRange> AgeDevRanges { get; } = new List<MinMaxDevRange>();

        public IList<Season> Seasons { get; } = new List<Season>();
    }
}
