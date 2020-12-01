using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.Models
{
    public class DriverResult
    {
        [Key]
        public int DriverResultId { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
        public int Grid { get; set; }
        public string PenaltyReason { get; set; }

        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }
        [EnumDataType(typeof(DNFCause))]
        public DNFCause DNFCause { get; set; }
        [EnumDataType(typeof(DSQCause))]
        public DSQCause DSQCause { get; set; }

        public IDictionary<int, int?> StintResults { get; } = new Dictionary<int, int?>();

        // Modifiers for this race that apply every stint || maybe rethink, change or remove this since this is a bit weird
        public int QualyMod { get; set; }
        public int DriverRacePace { get; set; }
        public int ChassisRacePace { get; set; }
        public int EngineRacePace { get; set; }
        public int MinRNG { get; set; }
        public int MaxRNG { get; set; }
        public int DriverRelMod { get; set; }
        public int ChassisRelMod { get; set; }

        public int SeasonDriverId { get; set; }
        public SeasonDriver SeasonDriver { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }
    }

    public enum Status { Finished, DNF, DSQ }

    public enum DNFCause
    {
        None = 0,
        Damage = 1,
        Collision = 2,
        Accident = 3,
        Puncture = 4,
        Engine = 5,
        Electrics = 6,
        Exhaust = 7,
        Clutch = 8,
        Hydraulics = 9,
        Wheel = 10,
        Brakes = 11
    }

    public enum DSQCause
    {
        None = 0,
        Illegal = 1,
        Fuel = 2,
        Dangerous = 3
    }
}
