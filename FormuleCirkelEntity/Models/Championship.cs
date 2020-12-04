using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class Championship
    {
        [Key]
        public int ChampionshipId { get; set; }
        public string ChampionshipName { get; set; }
        public bool ActiveChampionship { get; set; }

        public IList<Season> Seasons { get; } = new List<Season>();
    }
}
