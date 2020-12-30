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
        public int TyreLife { get; set; }
        public Tyre CurrTyre { get; set; }
        public string PenaltyReason { get; set; }
        [EnumDataType(typeof(Status))]
        public Status Status { get; set; }
        [EnumDataType(typeof(DNFCause))]
        public DNFCause DNFCause { get; set; }
        [EnumDataType(typeof(DSQCause))]
        public DSQCause DSQCause { get; set; }
        // Underneath is TBD
        public int QualyMod { get; set; }           // Result of all trait modifiers
        public int DriverRacePace { get; set; }     // Result of all trait modifiers
        public int ChassisRacePace { get; set; }    // Result of all trait modifiers
        public int EngineRacePace { get; set; }     // Result of all trait modifiers
        public int MinRNG { get; set; }             // Result of all trait modifiers
        public int MaxRNG { get; set; }             // Result of all trait modifiers
        public int DriverRelMod { get; set; }       // Result of all trait modifiers
        public int ChassisRelMod { get; set; }      // Result of all trait modifiers
        public int MaxTyreWear { get; set; }        // Result of all trait modifiers
        public int MinTyreWear { get; set; }        // Result of all trait modifiers

        public int SeasonDriverId { get; set; }
        public SeasonDriver SeasonDriver { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }
        public int StrategyId { get; set; }
        public Strategy Strategy { get; set; }

        public IList<StintResult> StintResults { get; } = new List<StintResult>();
    }

    public enum Status 
    { 
        Finished = 0,
        DNF = 1,
        DSQ = 2
    }

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
