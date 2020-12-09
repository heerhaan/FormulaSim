using FormuleCirkelEntity.Utility;
using FormuleCirkelEntity.Models;
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
        public void UpdateStintResult(StintResult stintResult, 
            Stint stint, 
            DriverResult driverResult,
            SeasonTeam team,
            Weather weather,
            Specification trackSpec,
            int driverCount,
            int qualyBonus,
            int pitMin,
            int pitMax)
        {
            if (stintResult is null || driverResult is null || stint is null || team is null)
                throw new NullReferenceException();

            // Applies the increased or decreased odds for the specific track.
            double engineWeatherMultiplier = 1;
            int tireWeatherBonus = 0;
            int tireWeatherWear = 0;

            int weatherRNG = 0;
            int weatherDNF = 0;

            var driver = driverResult.SeasonDriver;

            stintResult.StintStatus = StintStatus.Running;
            switch (weather)
            {
                case Weather.Sunny:
                    tireWeatherWear += 4;
                    engineWeatherMultiplier = 0.9;
                    break;
                case Weather.Overcast:
                    tireWeatherBonus += 2;
                    engineWeatherMultiplier = 1.1;
                    break;
                case Weather.Rain:
                    weatherRNG += 10;
                    weatherDNF += -3;
                    break;
                case Weather.Storm:
                    weatherRNG += 20;
                    weatherDNF += -5;
                    break;
            }

            if (stint.ApplyReliability)
            {
                // Check for the reliability of the chassis.
                if (GetChassisReliabilityResult(team.Reliability, driverResult.ChassisRelMod) == -1)
                {
                    stintResult.StintStatus = StintStatus.ChassisDNF;
                }
                // Check for the reliability of the driver.
                else if (GetDriverReliabilityResult(driver.Reliability, weatherDNF + driverResult.DriverRelMod) == -1)
                {
                    stintResult.StintStatus = StintStatus.DriverDNF;
                }
            }

            if (stintResult.StintStatus == StintStatus.Running || stintResult.StintStatus == StintStatus.Pitstop)
            {
                // Add one because Random.Next() has an exclusive upper bound.
                int result = _rng.Next((stint.RNGMinimum + driverResult.MinRNG), (stint.RNGMaximum + weatherRNG + driverResult.MaxRNG) + 1);

                if (stint.ApplyQualifyingBonus)
                {
                    result += Helpers.GetQualifyingBonus(driverResult.Grid, driverCount, qualyBonus);
                }

                if (stint.ApplyPitstop)
                {
                    result += _rng.Next(pitMin, pitMax);
                    stintResult.StintStatus = StintStatus.Pitstop;
                }

                if (stint.ApplyDriverLevel)
                {
                    result += driver.Skill + driverResult.DriverRacePace;
                }

                if (stint.ApplyChassisLevel)
                {
                    int bonus = Helpers.GetChassisBonus(Helpers.CreateTeamSpecDictionary(team), trackSpec.ToString());
                    int statusBonus = (((int)driver.DriverStatus) * -2) + 2;
                    result += (team.Chassis + driverResult.ChassisRacePace + bonus + statusBonus);
                }

                // Applies the power of the engine plus external factors when it is relevant for the current stint
                if (stint.ApplyEngineLevel)
                {
                    result += (int)Math.Round((team.Engine.Power + driverResult.EngineRacePace) * engineWeatherMultiplier);
                }

                if (stint.ApplyTireLevel && driver.Tires == Tire.Softs)
                {
                    result += (10 + tireWeatherBonus);
                }

                if (stint.ApplyTireWear && driver.Tires == Tire.Softs)
                {
                    // Calculates the extra wear a tire may have due to weather circumstances.
                    int maxWear = -20;
                    maxWear -= tireWeatherWear;
                    // Maximum of 1 because Random.Next() has an exclusive upper bound.
                    result += _rng.Next(maxWear, 1);
                }
                // Finally adds the result of the stint to the stintresult
                stintResult.Result = result;
            }
        }

        public int GetChassisReliabilityResult(int reliability, int additionalDNF)
        {
            var reliabilityScore = reliability + additionalDNF;
            var reliabilityCheckValue = _rng.Next(1, 101);
            return reliabilityScore.CompareTo(reliabilityCheckValue);
        }

        /// <summary>
        /// Performs a <see cref="SeasonDriver"/> reliability check.
        /// </summary>
        /// <param name="driver">The <see cref="SeasonDriver"/> to perform the reliability check on.</param>
        /// <returns>-1 if the reliability check fails, 1 if it succeeds, and 0 if it's neutral.</returns>
        public int GetDriverReliabilityResult(int reliability, int additionalDNF)
        {
            var reliabilityScore = reliability + additionalDNF;
            var reliabilityCheckValue = _rng.Next(1, 101); 
            return reliabilityScore.CompareTo(reliabilityCheckValue);
        }

        public int GetQualifyingResult(SeasonDriver driver, int qualyRNG, Track track, int qualypace)
        {
            if (driver is null || track is null)
                throw new NullReferenceException();

            var result = 0;
            result += driver.Skill;
            result += qualypace;
            result += driver.SeasonTeam.Chassis;
            result += driver.SeasonTeam.Engine.Power;
            result += Helpers.GetChassisBonus(Helpers.CreateTeamSpecDictionary(driver.SeasonTeam), track.Specification.ToString());
            result += _rng.Next(0, (qualyRNG + 1));
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
                        if (driver.SeasonDriver.SeasonTeam == aboveDriver.SeasonDriver.SeasonTeam)
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
            int position = 0;
            foreach (var result in driverResults)
            {
                position++;
                result.Position = position;
                result.StintResults.Single(sr => sr.Number == stintProgress).Position = position;
            }
        }
    }
}
