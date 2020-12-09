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

        public int SeasonNumber { get; set; }
        public int QualificationRemainingDriversQ2 { get; set; }
        public int QualificationRemainingDriversQ3 { get; set; }
        public int QualificationRNG { get; set; }
        public int QualyBonus { get; set; }
        public int PitMin { get; set; }
        public int PitMax { get; set; }
        public IDictionary<int, int?> PointsPerPosition { get; } = new Dictionary<int, int?>();
        public int PolePoints { get; set; }

        public int ChampionshipId { get; set; }
        public Championship Championship { get; set; }

        public IList<Race> Races { get; } = new List<Race>();
        public IList<SeasonDriver> Drivers { get; } = new List<SeasonDriver>();
        public IList<SeasonTeam> Teams { get; } = new List<SeasonTeam>();
    }
}
