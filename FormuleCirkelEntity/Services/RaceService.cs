using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.Services
{
    public static class RaceService
    {
        private static readonly Random rng = new Random();

        public static void AddSeasonDriversToRace(Race race, IEnumerable<SeasonDriver> drivers)
        {
            // Check if given parameters aren't null
            if (race is null || drivers is null) throw new NullReferenceException();

            foreach (var driver in drivers)
            {
                DriverResult driverResult = new DriverResult { SeasonDriver = driver };
                foreach (var stint in race.Stints)
                {
                    StintResult driverStint = new StintResult
                    {
                        StintStatus = StintStatus.Concept,
                        Number = stint.Number
                    };
                    driverResult.StintResults.Add(driverStint);
                }
                race.DriverResults.Add(driverResult);
            }
        }

        // Sets the driverresults for all drivers for one race
        public static void SetDriverResultsOnRace(
            Race race, 
            List<DriverTrait> driverTraits, 
            List<TeamTrait> teamTraits, 
            List<TrackTrait> trackTraits,
            List<SeasonTeam> seasonTeams,
            List<Strategy> strategies)
        {
            foreach (var driverRes in race.DriverResults)
            {
                // Gets the traits from the driver in the loop and sets them
                var thisDriverTraits = driverTraits.Where(drt => drt.DriverId == driverRes.SeasonDriver.DriverId);
                RaceService.SetDriverTraitMods(driverRes, thisDriverTraits);
                // Gets the seasonteam of the driver in the loop
                var thisDriverTeam = seasonTeams.First(st => st.SeasonDrivers.Contains(driverRes.SeasonDriver));
                // Gets the traits from the team of the driver in the loop and sets them
                var thisTeamTraits = teamTraits.Where(ttr => ttr.TeamId == thisDriverTeam.TeamId);
                RaceService.SetTeamTraitMods(driverRes, thisTeamTraits);
                // Sets the traits from the track to the driver in the loop
                RaceService.SetTrackTraitMods(driverRes, trackTraits);

                // Set a random strategy
                int stratIndex = rng.Next(0, strategies.Count);
                RaceService.SetRandomStrategy(driverRes, strategies[stratIndex]);
            }
        }

        public static void SetDriverTraitMods(DriverResult driver, IEnumerable<DriverTrait> driverTraits)
        {
            // Null-check, since I don't like warnings
            if (driver is null || driverTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits a driver may have and adds them to the driverresult modifiers
            foreach (var trait in driverTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
            }
        }

        public static void SetTeamTraitMods(DriverResult driver, IEnumerable<TeamTrait> teamTraits)
        {
            // Null-check, since I don't like warnings
            if (driver is null || teamTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits the team of a driver may have and adds them to the driverresult modifiers
            foreach (var trait in teamTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
            }
        }

        public static void SetTrackTraitMods(DriverResult driver, List<TrackTrait> trackTraits)
        {
            // Null-check, since I don't like warnings
            if (driver is null || trackTraits is null) { throw new NullReferenceException(); }
            // Loops through all the traits a track may have and adds them to the driverresult modifiers
            foreach (var trait in trackTraits)
            {
                SetIndividualTraitMod(driver, trait.Trait);
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

            if (trait.MaxTyreWear.HasValue)
                driver.MaxTyreWear += trait.MaxTyreWear.Value;

            if (trait.MinTyreWear.HasValue)
                driver.MinTyreWear += trait.MinTyreWear.Value;
        }

        public static void SetRandomStrategy(DriverResult driverRes, Strategy strategy)
        {
            if (driverRes is null || strategy is null) { throw new NullReferenceException(); }

            var currentTyre = strategy.Tyres.Single(t => t.StintNumberApplied == 1).Tyre;
            driverRes.Strategy = strategy;
            driverRes.CurrTyre = currentTyre;
            driverRes.TyreLife = currentTyre.Pace;
        }
    }
}
