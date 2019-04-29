using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public enum SeasonState
    {
        Draft = 0,
        Progress = 1,
        Finished = 2
    }

    public class Season
    {
        [Key]
        public int SeasonId { get; set; }

        public DateTime? SeasonStart { get; set; }

        [EnumDataType(typeof(SeasonState))]
        public SeasonState State { get; set; }

        public virtual IList<Race> Races { get; set; }
        public virtual IList<SeasonDriver> Drivers { get; set; }
        public virtual IList<SeasonTeam> Teams { get; set; }
    }
}