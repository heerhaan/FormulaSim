using FormuleCirkelEntity.Utility;
using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.ResultGenerators
{
    public class RaceResultGenerator
    {
        private readonly Random _rng;

        public RaceResultGenerator(Random randomGen)
        {
            _rng = randomGen;
        }

        /// <summary>
        /// Gets the points result of a single <see cref="StintResult"/>, with any enabled <see cref="StintResult"/> modifiers applied.
        /// </summary>
        /// <param name="stint">The <see cref="Stint"/> supplying the modifiers to use.</param>
        /// <param name="driverResult">The partial <see cref="DriverResult"/> from which to derive certain modifiers.</param>
        public void UpdateStintResult(StintResult stintResult,
            Stint stint,
            DriverResult driverResult,
            SeasonTeam team,
            Weather weather,
            Specification trackSpec,
            int driverCount,
            int qualyBonus,
            int pitMin,
            int pitMax,
            AppConfig appConfig)
        {
            if (stintResult is null || driverResult is null || stint is null || team is null || appConfig is null)
                throw new NullReferenceException();

            // Applies the increased or decreased odds for the specific track.
            double engineWeatherMultiplier = 1;
            int weatherRNG = 0;
            int weatherDNF = 0;

            var driver = driverResult.SeasonDriver;

            stintResult.StintStatus = StintStatus.Running;
            switch (weather)
            {
                case Weather.Sunny:
                    engineWeatherMultiplier = appConfig.SunnyEngineMultiplier;
                    break;
                case Weather.Overcast:
                    engineWeatherMultiplier = appConfig.OvercastEngineMultiplier;
                    break;
                case Weather.Rain:
                    weatherRNG += appConfig.RainAdditionalRNG;
                    weatherDNF += appConfig.RainDriverReliabilityModifier;
                    engineWeatherMultiplier = appConfig.WetEngineMultiplier;
                    break;
                case Weather.Storm:
                    weatherRNG += appConfig.StormAdditionalRNG;
                    weatherDNF += appConfig.StormDriverReliabilityModifier;
                    engineWeatherMultiplier = appConfig.WetEngineMultiplier;
                    break;
            }

            if (stint.ApplyReliability)
            {
                // Check for the reliability of the chassis.
                if (GetReliabilityResult(team.Reliability + driverResult.ChassisRelMod) == -1)
                {
                    stintResult.StintStatus = StintStatus.ChassisDNF;
                }
                // Check for the reliability of the driver.
                else if (GetReliabilityResult(driver.Reliability + weatherDNF + driverResult.DriverRelMod) == -1)
                {
                    stintResult.StintStatus = StintStatus.DriverDNF;
                    if (driver.SeasonDriverId == 1894)
                        stintResult.StintStatus = StintStatus.DriverDNF;
                }
            }

            if (stintResult.StintStatus == StintStatus.Running || stintResult.StintStatus == StintStatus.Mistake)
            {
                // Add one because Random.Next() has an exclusive upper bound.
                int result = _rng.Next(stint.RNGMinimum + driverResult.MinRNG, stint.RNGMaximum + weatherRNG + driverResult.MaxRNG + 1);
                // Iterate through GetReliabilityResult twice to check if a driver made a mistake or not, requires two consequential true's to return a mistake
                bool mistake = false;
                for (int i = 0; i < appConfig.MistakeAmountRolls; i++)
                {
                    mistake = GetReliabilityResult(driver.Reliability + weatherDNF + driverResult.DriverRelMod) == -1;
                    if (!mistake) { break; }
                }
                // If the bool mistake is true then we have to subtract from the result
                if (mistake)
                {
                    result += _rng.Next(appConfig.MistakeLowerValue, appConfig.MistakeUpperValue);
                    stintResult.StintStatus = StintStatus.Mistake;
                }
                // In here we loop through the strategy of the driver to see if it is time for a pitstop
                foreach (var tyreStrat in driverResult.Strategy.Tyres.OrderBy(t => t.StintNumberApplied))
                {
                    // The value for the tyre in iteration matches the current stint number, so it is time for a pitstop
                    if (tyreStrat.StintNumberApplied == stint.Number && stint.Number != 1)
                    {
                        driverResult.TyreLife = tyreStrat.Tyre.Pace;
                        driverResult.CurrTyre = tyreStrat.Tyre;
                        stintResult.Pitstop = true;
                    }
                }
                // Current status tells us there is a pitstop so calculate pitstop RNG over the result
                if (stintResult.Pitstop)
                {
                    result += _rng.Next(pitMin, pitMax + 1);
                }
                // Deals with the tyre wear for this driver and changes tyres if it is needed
                result += driverResult.TyreLife;
                // Current status tells us the driver is still running so we apply some of the wear to the tyre
                driverResult.TyreLife += _rng.Next(driverResult.CurrTyre.MaxWear + driverResult.MaxTyreWear, driverResult.CurrTyre.MinWear + driverResult.MinTyreWear);

                // Applies the qualifying bonus based on the amount of drivers for the current stint
                if (stint.ApplyQualifyingBonus)
                {
                    result += Helpers.GetQualifyingBonus(driverResult.Grid, driverCount, qualyBonus);
                }
                // Applies the quality of the driver to the current stint if it is relevant
                if (stint.ApplyDriverLevel)
                {
                    result += driver.Skill + driverResult.DriverRacePace;
                }
                // Applies the power of the chassis to the result when it applies
                if (stint.ApplyChassisLevel)
                {
                    int bonus = Helpers.GetChassisBonus(Helpers.CreateTeamSpecDictionary(team), trackSpec.ToString());
                    int statusBonus = 0;
                    if (driver.DriverStatus == DriverStatus.First)
                        statusBonus = appConfig.ChassisModifierDriverStatus;
                    else if (driver.DriverStatus == DriverStatus.Second)
                        statusBonus = (appConfig.ChassisModifierDriverStatus * -1);

                    result += team.Chassis + driverResult.ChassisRacePace + bonus + statusBonus;
                }
                // Applies the power of the engine plus external factors when it is relevant for the current stint
                if (stint.ApplyEngineLevel)
                {
                    result += (int)Math.Round((team.Engine.Power + driverResult.EngineRacePace) * engineWeatherMultiplier);
                }
                // Finally adds the result of the stint to the stintresult
                stintResult.Result = result;
            }
        }

        // Performs a reliability check based on the given values
        public int GetReliabilityResult(int reliability)
        {
            var reliabilityCheckValue = _rng.Next(1, 101);
            return reliability.CompareTo(reliabilityCheckValue);
        }

        // Calculate a score generated in qualifying
        public int GetQualifyingResult(SeasonDriver driver, int qualyRNG, Track track, int qualypace)
        {
            if (driver is null || track is null) { throw new NullReferenceException(); }

            var result = 0;
            result += driver.Skill;
            result += qualypace;
            result += driver.SeasonTeam.Chassis;
            result += driver.SeasonTeam.Engine.Power;
            result += Helpers.GetChassisBonus(Helpers.CreateTeamSpecDictionary(driver.SeasonTeam), track.Specification.ToString());
            result += _rng.Next(0, qualyRNG + 1);
            return result;
        }

        /// <summary>
        /// Get a dictionary of the results' position values based on their points total relative to each other.
        /// </summary>
        /// <param name="driverResults">The <see cref="DriverResult"/>s to determine the relative positions of.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of the driverResult ID's and the corresponding positions.</returns>
        /// <remarks>When two driver points totals are equal, their position is determined based on their original grid position.</remarks>
        public static void GetPositionsBasedOnRelativePoints(List<DriverResult> driverResults, int stintProgress)
        {
            driverResults = driverResults
                .OrderByDescending(d => d.Points)
                .ThenBy(d => d.Grid)
                .ToList();

            // Swap drivers when the driver above is the second driver for the driver below
            foreach (var driver in driverResults)
            {
                if (driver.SeasonDriver.DriverStatus == DriverStatus.First && driver.Status == Status.Finished)
                {
                    int index = driverResults.IndexOf(driver);
                    if (index != 0)
                    {
                        var aboveDriver = driverResults[(index - 1)];
                        if (driver.SeasonDriver.SeasonTeamId == aboveDriver.SeasonDriver.SeasonTeamId 
                            && aboveDriver.SeasonDriver.DriverStatus == DriverStatus.Second)
                        {
                            int firstDriverPoints = driver.Points;
                            driver.Points = aboveDriver.Points;
                            aboveDriver.Points = firstDriverPoints;
                        }
                    }
                }
            }

            // Quickly sort all the driverResults again
            driverResults.Sort((l, r) => -1 * l.Points.CompareTo(r.Points));
            int position = 1;
            foreach (var result in driverResults)
            {
                result.Position = position;
                result.StintResults.Single(sr => sr.Number == stintProgress).Position = position;
                position++;
            }
        }
    }
}
