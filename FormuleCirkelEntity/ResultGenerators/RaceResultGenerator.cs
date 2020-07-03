﻿using FormuleCirkelEntity.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FormuleCirkelEntity.ResultGenerators
{
    public class RaceResultGenerator
    {
        Random _rng;

        public RaceResultGenerator(Random randomGen)
        {
            _rng = randomGen;
        }

        /// <summary>
        /// Gets the points result of a single <see cref="Stint"/>, with any enabled <see cref="Stint"/> modifiers applied.
        /// </summary>
        /// <param name="driverResult">The partial <see cref="DriverResult"/> from which to derive certain modifiers.</param>
        /// <param name="stint">The <see cref="Stint"/> supplying the modifiers to use.</param>
        /// <returns>A <see cref="int"/> points value, or <see cref="int.MinValue"/> if a DNF result occured.</returns>
        public int? GetStintResult(DriverResult driverResult, Stint stint, Track track, Race race)
        {
            // Applies the increased or decreased odds for the specific track.
            double engineWeatherMultiplier = 1;
            int tireWeatherBonus = 0;
            int tireWeatherWear = 0;

            int weatherRNG = 0;
            int weatherDNF = 0;

            int result = 0;

            // If the stint is not a pitstop then it applies the RNG modifiers and the weather effects.
            if (stint.RNGMaximum > 0)
            {
                switch (race.Weather)
                {
                    case Weather.Sunny:
                        tireWeatherWear += 5;
                        engineWeatherMultiplier = 0.9;
                        break;
                    case Weather.Overcast:
                        tireWeatherBonus += 3;
                        engineWeatherMultiplier = 1.1;
                        break;
                    case Weather.Rain:
                        weatherRNG += 10;
                        weatherDNF += -3;
                        break;
                    case Weather.Storm:
                        weatherRNG += 20;
                        weatherDNF += -4;
                        break;
                }

                // Add one because Random.Next() has an exclusive upper bound.
                result = _rng.Next((stint.RNGMinimum + driverResult.MinRNG), (stint.RNGMaximum + weatherRNG + driverResult.MaxRNG) + 1);
            }
            else
            {
                result = _rng.Next((stint.RNGMinimum), (stint.RNGMaximum) + 1);
            }

            if (stint.ApplyDriverLevel)
            {
                result += driverResult.SeasonDriver.Skill + driverResult.DriverRacePace;
            }

            if (stint.ApplyQualifyingBonus)
                result += GetQualifyingBonus(driverResult.Grid, driverResult.SeasonDriver.Season.Drivers.Count, driverResult.SeasonDriver.Season.QualyBonus);

            if (stint.ApplyTireLevel && driverResult.SeasonDriver.Tires == Tires.Softs)
                result += (10 + tireWeatherBonus);

            if (stint.ApplyEngineLevel)
                result += (int)Math.Round((driverResult.SeasonDriver.SeasonTeam.Engine.Power + driverResult.EngineRacePace) * engineWeatherMultiplier);

            if (stint.ApplyTireWear && driverResult.SeasonDriver.Tires == Tires.Softs)
            {
                // Calculates the extra wear a tire may have due to weather circumstances.
                int maxWear = -20;
                maxWear -= tireWeatherWear;
                // Maximum of 1 because Random.Next() has an exclusive upper bound.
                result += _rng.Next(maxWear, 1);
            }

            if (stint.ApplyReliability)
            {
                // Check for the reliability of the chassis.
                var reliabilityResult = GetChassisReliabilityResult(driverResult.SeasonDriver.SeasonTeam, driverResult.ChassisRelMod);
                if (reliabilityResult == -1)
                    return null;

                // Check for the reliability of the driver.
                reliabilityResult = GetDriverReliabilityResult(driverResult.SeasonDriver, weatherDNF + driverResult.DriverRelMod);
                if (reliabilityResult == -1)
                    return -1000;
            }

            if (stint.ApplyChassisLevel)
            {
                int bonus = GetChassisBonus(driverResult.SeasonDriver.SeasonTeam, track);
                int statusBonus = (((int)driverResult.SeasonDriver.DriverStatus) * -2) + 2;
                result += (driverResult.SeasonDriver.SeasonTeam.Chassis + driverResult.ChassisRacePace + bonus + statusBonus);
            }

            return result;
        }

        public int GetQualifyingBonus(int qualifyingPosition, int totalDriverCount, int qualyBonus)
        {
            return (totalDriverCount * qualyBonus) - (qualifyingPosition * qualyBonus);
        }

        public int GetChassisBonus(SeasonTeam team, Track track)
        {
            int bonus = 0;
            Dictionary<string, int> specs = new Dictionary<string, int>
            {
                { "Topspeed", team.Topspeed },
                { "Acceleration", team.Acceleration },
                { "Handling", team.Handling }
            };

            var spec = (specs.SingleOrDefault(k => k.Key == track.Specification.ToString()));
            bonus = spec.Value;

            return bonus;
        }

        public int GetChassisReliabilityResult(SeasonTeam team, int additionalDNF)
        {
            var reliabilityScore = team.Reliability + additionalDNF;
            var reliabilityCheckValue = _rng.Next(1, 101);
            return reliabilityScore.CompareTo(reliabilityCheckValue);
        }

        /// <summary>
        /// Performs a <see cref="SeasonDriver"/> reliability check.
        /// </summary>
        /// <param name="driver">The <see cref="SeasonDriver"/> to perform the reliability check on.</param>
        /// <returns>-1 if the reliability check fails, 1 if it succeeds, and 0 if it's neutral.</returns>
        public int GetDriverReliabilityResult(SeasonDriver driver, int additionalDNF)
        {
            var reliabilityScore = driver.Reliability + additionalDNF;
            var reliabilityCheckValue = _rng.Next(1, 101); 
            return reliabilityScore.CompareTo(reliabilityCheckValue);
        }

        public int GetQualifyingResult(SeasonDriver driver, int qualyRNG, Track track, int qualypace)
        {
            var result = 0;
            result += driver.Skill;
            result += qualypace;
            result += driver.SeasonTeam.Chassis;
            result += driver.SeasonTeam.Engine.Power;
            result += GetChassisBonus(driver.SeasonTeam, track);
            result += _rng.Next(0, (qualyRNG + 1));
            return result;
        }

        /// <summary>
        /// Get a dictionary of the results' position values based on their points total relative to each other.
        /// </summary>
        /// <param name="driverResults">The <see cref="DriverResult"/>s to determine the relative positions of.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of the driverResult ID's and the corresponding positions.</returns>
        /// <remarks>When two driver points totals are equal, their position is determined based on their original grid position.</remarks>
        public DriverSwap GetPositionsBasedOnRelativePoints(IEnumerable<DriverResult> driverResults)
        {
            var orderedResults = driverResults
                .OrderByDescending(d => d.Points)
                .ThenBy(d => d.Grid)
                .ToList();

            // Swap drivers when the driver above is the second driver for the driver below
            foreach (var driver in orderedResults)
            {
                if (driver.SeasonDriver.DriverStatus == DriverStatus.First && driver.Status == Status.Finished)
                {
                    int index = orderedResults.IndexOf(driver);
                    if (index != 0)
                    {
                        var aboveDriver = orderedResults[(index - 1)];
                        if (driver.SeasonDriver.SeasonTeam == aboveDriver.SeasonDriver.SeasonTeam)
                        {
                            int firstDriverPoints = driver.Points;
                            driver.Points = aboveDriver.Points;
                            aboveDriver.Points = firstDriverPoints;
                        }
                    }
                }
            }
            orderedResults.Sort((l, r) => -1 * l.Points.CompareTo(r.Points));
            DriverSwap swap = new DriverSwap
            {
                DriverResults = orderedResults,
                OrderedResults = orderedResults.ToDictionary((res => res.DriverResultId), (res => orderedResults.IndexOf(res) + 1))
            };

            return swap;
        }
    }

    public class DriverSwap
    {
        public IDictionary<int, int> OrderedResults { get; set; }
        public List<DriverResult> DriverResults { get; set; }
    }
}
