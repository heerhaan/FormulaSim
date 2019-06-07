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
        public int? GetStintResult(DriverResult driverResult, Stint stint)
        {
            // Add one because Random.Next() has an exclusive upper bound.
            var result = _rng.Next(stint.RNGMinimum, stint.RNGMaximum + 1);

            if (stint.ApplyDriverLevel)
                result = result + GetDriverLevelBonus(driverResult.SeasonDriver);

            if (stint.ApplyQualifyingBonus)
                result += GetQualifyingBonus(driverResult.Grid, driverResult.SeasonDriver.Season.Drivers.Count);

            if (stint.ApplyTireLevel && driverResult.SeasonDriver.Tires == Tires.Zacht)
                result += 10;

            if (stint.ApplyEngineLevel)
                result += driverResult.SeasonDriver.SeasonTeam.Engine.Power;

            if (stint.ApplyTireWear && driverResult.SeasonDriver.Tires == Tires.Zacht)
                // Maximum of 1 because Random.Next() has an exclusive upper bound.
                result += _rng.Next(-20, 1);

            if (stint.ApplyReliability)
            {
                var reliablityResult = GetDriverReliabilityResult(driverResult.SeasonDriver);
                if (reliablityResult == -1)
                    return null;
                else if (reliablityResult == 0)
                    result += -20;
            }

            if (stint.ApplyChassisLevel)
            {
                var specificationPositive = driverResult.SeasonDriver.SeasonTeam.Specification == driverResult.Race.Track.Specification;
                var multiplier = specificationPositive ? 1.15 : 1;
                result += (int)Math.Round(driverResult.SeasonDriver.SeasonTeam.Chassis * multiplier);
            }

            return result;
        }

        public int GetDriverLevelBonus(SeasonDriver driver)
        {
            return driver.Skill + (3 - (3 * (int)driver.Style));
        }

        public int GetQualifyingBonus(int qualifyingPosition, int totalDriverCount)
        {
            return (totalDriverCount * 3) - (qualifyingPosition * 3);
        }

        public bool TrackSpecificationPositive(DriverResult driverResult)
            => driverResult.SeasonDriver.SeasonTeam.Specification == driverResult.Race.Track.Specification;

        /// <summary>
        /// Performs a <see cref="SeasonDriver"/> reliability check.
        /// </summary>
        /// <param name="driver">The <see cref="SeasonDriver"/> to perform the reliability check on.</param>
        /// <returns>-1 if the reliability check fails, 1 if it succeeds, and 0 if it's neutral.</returns>
        public int GetDriverReliabilityResult(SeasonDriver driver)
        {
            var driverStyleModifier = ((int)driver.Style - 1);
            var reliabilityScore = driver.SeasonTeam.Reliability + driverStyleModifier;
            var reliabilityCheckValue = _rng.Next(1, 26); 
            return reliabilityScore.CompareTo(reliabilityCheckValue);
        }

        public int GetQualifyingResult(SeasonDriver driver)
        {
            var result = 0;
            result += driver.Skill;
            result += driver.SeasonTeam.Chassis;
            result += _rng.Next(0, 60);
            return result;
        }

        /// <summary>
        /// Get a dictionary of the results' position values based on their points total relative to each other.
        /// </summary>
        /// <param name="driverResults">The <see cref="DriverResult"/>s to determine the relative positions of.</param>
        /// <returns>A <see cref="Dictionary{TKey, TValue}"/> of the driverResult ID's and the corresponding positions.</returns>
        /// <remarks>When two driver points totals are equal, their position is determined based on their original grid position.</remarks>
        public IDictionary<int, int> GetPositionsBasedOnRelativePoints(IEnumerable<DriverResult> driverResults)
        {
            var orderedResults = driverResults
                .OrderByDescending(d => d.Points)
                .ThenBy(d => d.Grid)
                .ToList();

            return orderedResults.ToDictionary((res => res.DriverResultId), (res => orderedResults.IndexOf(res) + 1));
        }
    }
}
