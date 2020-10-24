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

        public RaceBuilder AddAllDrivers(Track track)
        {
            if (_race.Season != null)
                AddDrivers(_race.Season.Drivers, track);
            return this;
        }

        public RaceBuilder AddDrivers(IEnumerable<SeasonDriver> drivers, Track track)
        {
            // Check if given parameters aren't null
            if (drivers is null || track is null)
                throw new NullReferenceException();

            foreach (var driver in drivers)
            {
                if (driver.Dropped)
                    continue;

                DriverResult driverResult = new DriverResult
                {
                    SeasonDriver = driver
                };

                _race.DriverResults.Add(SetTraitMods(driverResult, track));
            }
                
            return this;
        }

        private static DriverResult SetTraitMods(DriverResult driver, Track track)
        {
            try
            {
                if (driver.SeasonDriver.Traits.Any())
                {
                    foreach (var trait in driver.SeasonDriver.Traits.Values)
                    {
                        SetIndividualTraitMod(driver, trait);
                    }
                }

                if (driver.SeasonDriver.SeasonTeam.Traits.Any())
                {
                    foreach (var trait in driver.SeasonDriver.SeasonTeam.Traits.Values)
                    {
                        SetIndividualTraitMod(driver, trait);
                    }
                }

                if (track.Traits.Any())
                {
                    foreach (var trait in track.Traits.Values)
                    {
                        SetIndividualTraitMod(driver, trait);
                    }
                }

                return driver;
            }
            catch (NullReferenceException)
            {
                return driver;
            }
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
