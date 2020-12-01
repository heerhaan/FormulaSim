using FormuleCirkelEntity.Models;
using GuardNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Builders
{
    public class RaceBuilder
    {
        Race _race;

        public RaceBuilder()
        {
            _race = new Race();
        }

        public RaceBuilder Use(Race race)
        {
            _race = race;
            return this;
        }

        public RaceBuilder InitializeRace(Track track, Season season)
        {
            Guard.NotNull(track, nameof(track));
            Guard.NotNull(season, nameof(season));

            _race.Name = track.Name;
            _race.Track = track;
            _race.Season = season;
            _race.Round = season.Races.Count + 1;
            return this;
        }

        public RaceBuilder AddDefaultStints()
        {
            return AddDefaultStartStint()
                .AddDefaultPitstopStint()
                .AddDefaultCloseStint();
        }

        public RaceBuilder AddModifiedStints(IList<Stint> settings)
        {
            foreach (var stint in settings)
            {
                _race.Stints.Add(_race.Stints.Count + 1, stint);
            }
            return this;
        }

        public RaceBuilder AddDefaultStartStint()
        {
            Stint stint = new Stint()
            {
                ApplyChassisLevel = true,
                ApplyDriverLevel = true,
                ApplyEngineLevel = true,
                ApplyQualifyingBonus = true,
                ApplyTireLevel = true,
                RNGMinimum = 0,
                RNGMaximum = 25
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddDefaultPitstopStint()
        {
            Stint stint = new Stint()
            {
                RNGMinimum = -12,
                RNGMaximum = -5
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddDefaultCloseStint()
        {
            Stint stint = new Stint()
            {
                ApplyReliability = true,
                ApplyTireWear = true,
                RNGMinimum = 0,
                RNGMaximum = 35
            };

            _race.Stints.Add(_race.Stints.Count + 1, stint);
            return this;
        }

        public RaceBuilder AddAllDrivers()
        {
            if (_race.Season != null)
                AddDrivers(_race.Season.Drivers);

            return this;
        }

        public RaceBuilder AddDrivers(IEnumerable<SeasonDriver> drivers)
        {
            // Check if given parameters aren't null
            if (drivers is null)
                throw new NullReferenceException();

            foreach (var driver in drivers)
            {
                // Ensures that dropped drivers won't be added to a race by not creating a result object for them
                if (driver.Dropped)
                    continue;

                DriverResult driverResult = new DriverResult { SeasonDriver = driver };
                _race.DriverResults.Add(SetTraitMods(driverResult, _race.Track.TrackTraits));
            }
            return this;
        }

        private static DriverResult SetTraitMods(DriverResult driver, IList<TrackTrait> trackTraits)
        {
            // Loops through all the traits a driver may have and adds them to the driverresult modifiers
            foreach (var trait in driver.SeasonDriver.Driver.DriverTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
            }

            // Loops through all the traits the team of a driver may have and adds them to the driverresult modifiers
            foreach (var trait in driver.SeasonDriver.SeasonTeam.Team.TeamTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
            }

            // Loops through all the traits a track may have and adds them to the driverresult modifiers
            foreach (var trait in trackTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
            }

            return driver;
        }

        private static void SetIndividualTraitMod(DriverResult driver, Trait trait)
        {
            if (trait.QualyPace.HasValue)
                driver.QualyMod += trait.QualyPace.Value;

            if (trait.DriverRacePace.HasValue)
                driver.DriverRacePace += trait.DriverRacePace.Value;

            if (trait.ChassisRacePace.HasValue)
                driver.ChassisRacePace += trait.ChassisRacePace.Value;

            if (trait.EngineRacePace.HasValue)
                driver.EngineRacePace += trait.EngineRacePace.Value;

            if (trait.ChassisReliability.HasValue)
                driver.ChassisRelMod += trait.ChassisReliability.Value;

            if (trait.DriverReliability.HasValue)
                driver.DriverRelMod += trait.DriverReliability.Value;

            if (trait.MaximumRNG.HasValue)
                driver.MaxRNG += trait.MaximumRNG.Value;

            if (trait.MinimumRNG.HasValue)
                driver.MinRNG += trait.MinimumRNG.Value;
        }
        
        public void Refresh() => _race = new Race();

        public Race GetResult() => _race;

        public Race GetResultAndRefresh()
        {
            var result = GetResult();
            Refresh();
            return result;
        }
    }
}
