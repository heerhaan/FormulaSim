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
        // Represents the current life the season is in, which ranges from in settings to finished
        [EnumDataType(typeof(SeasonState))]
        public SeasonState State { get; set; }
        // Number stands for the year the season is in, usually
        public int SeasonNumber { get; set; }
        // How many drivers remain in qualifying after the first knock-out round
        public int QualificationRemainingDriversQ2 { get; set; }
        // How many drivers remain in qualifying after the second and last knock-out round
        public int QualificationRemainingDriversQ3 { get; set; }
        // The randomly generated value to be added to the drivers base values in qualifying
        public int QualificationRNG { get; set; }
        // The bonus every qualifying position above last gives a driver during a race
        public int QualyBonus { get; set; }
        // The lowest value a pitstop can randomly get
        public int PitMin { get; set; }
        // The highest value a pitstop can get
        public int PitMax { get; set; }
        // How many points scoring a pole (P1) in qualifying gives a driver
        public int PolePoints { get; set; }
        // How many points each given position gets when finishing a race
        public IDictionary<int, int?> PointsPerPosition { get; } = new Dictionary<int, int?>();

        public int ChampionshipId { get; set; }
        public Championship Championship { get; set; }

        public IList<Race> Races { get; } = new List<Race>();
        public IList<SeasonDriver> Drivers { get; } = new List<SeasonDriver>();
        public IList<SeasonTeam> Teams { get; } = new List<SeasonTeam>();
    }
}
