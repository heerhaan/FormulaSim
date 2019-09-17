using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public Season()
        {
            PointsPerPosition = new Dictionary<int, int?>();
        }

        [Key]
        public int SeasonId { get; set; }
        public DateTime? SeasonStart { get; set; }

        [EnumDataType(typeof(SeasonState))]
        public SeasonState State { get; set; }

        public int SeasonNumber { get; set; }
        public int QualificationRemainingDriversQ2 { get; set; }
        public int QualificationRemainingDriversQ3 { get; set; }
        public int QualificationRNG { get; set; }
        public IDictionary<int, int?> PointsPerPosition { get; set; }
        public int PolePoints { get; set; }

        public int ChampionshipId { get; set; }
        public Championship Championship { get; set; }

        public virtual IList<Race> Races { get; set; }
        public virtual IList<SeasonDriver> Drivers { get; set; }
        public virtual IList<SeasonTeam> Teams { get; set; }
    }
}
