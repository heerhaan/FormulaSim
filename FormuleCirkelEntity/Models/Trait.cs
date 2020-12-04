using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Models
{
    public class Trait
    {
        // Identifier for the traits.
        public int TraitId { get; set; }
        // The name of the trait.
        public string Name { get; set; }
        //The group which the trait belongs to, this is either driver, team or track as defined in the enum.
        [EnumDataType(typeof(TraitGroup))]
        public TraitGroup TraitGroup { get; set; }
        // A description of the trait.
        public string TraitDescription { get; set; }
        // The value that gets added only in qualy, same for driver and team.
        public int? QualyPace { get; set; }
        // The value that gets add up whenever the driver value counts during the race.
        public int? DriverRacePace { get; set; }
        // The value that gets add up whenever the chassis value counts during the race.
        public int? ChassisRacePace { get; set; }
        // The value that gets add up whenever the engine power counts during the race.
        public int? EngineRacePace { get; set; }
        // The value that impacts the reliability of the chassis.
        public int? ChassisReliability { get; set; }
        // The value that impacts the reliablity of the driver.
        public int? DriverReliability { get; set; }
        // Impacts the upper value of the maximum amount of possible dev for drivers/teams or RNG per stint for tracks.
        public int? MaximumRNG { get; set; }
        // Impacts the lower value of the minimum amount of possible dev for drivers/teams or RNG per stint for tracks.
        public int? MinimumRNG { get; set; }
        // Determines if the trait is still in use in the application.
        public bool Archived { get; set; }
        public IList<DriverTrait> DriverTraits { get; } = new List<DriverTrait>();
        public IList<TeamTrait> TeamTraits { get; } = new List<TeamTrait>();
        public IList<TrackTrait> TrackTraits { get; } = new List<TrackTrait>();
    }

    public enum TraitGroup { Driver, Team, Track }
}
