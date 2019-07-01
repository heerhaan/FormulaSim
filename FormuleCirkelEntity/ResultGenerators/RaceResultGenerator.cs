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
        public int? GetStintResult(DriverResult driverResult, Stint stint, Track track, Race race)
        {
            // Applies the increased or decreased odds for the specific track.
            int rngTrack = 0;
            int dnfTrack = 0;
            double engineWeatherMultiplier = 0;
            int tireWeatherBonus = 0;
            int tireWeatherWear = 0;
            if (stint.RNGMaximum > 0)
            {
                if (track.RNGodds == RNGodds.Increased) { rngTrack += 5; } else if (track.RNGodds == RNGodds.Decreased) { rngTrack += -5; }

                // Determines the effect weather has during a stint on-track
                // Engine may be bonusified
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
                        rngTrack += 5;
                        dnfTrack += -1;
                        break;
                    case Weather.Storm:
                        rngTrack += 8;
                        dnfTrack += -2;
                        break;
                }
            }
            if (track.DNFodds == DNFodds.Increased) { dnfTrack += -1; } else if (track.DNFodds == DNFodds.Decreased) { dnfTrack += 1; }

            // Add one because Random.Next() has an exclusive upper bound.
            var result = _rng.Next(stint.RNGMinimum, (stint.RNGMaximum + rngTrack) + 1);

            if (stint.ApplyDriverLevel)
                result = result + GetDriverLevelBonus(driverResult.SeasonDriver);

            if (stint.ApplyQualifyingBonus)
                result += GetQualifyingBonus(driverResult.Grid, driverResult.SeasonDriver.Season.Drivers.Count);

            if (stint.ApplyTireLevel && driverResult.SeasonDriver.Tires == Tires.Softs)
                result += (10 + tireWeatherBonus);

            if (stint.ApplyEngineLevel)
                result += (int)Math.Round(driverResult.SeasonDriver.SeasonTeam.Engine.Power * engineWeatherMultiplier);

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
                var reliablityResult = GetDriverReliabilityResult(driverResult.SeasonDriver, dnfTrack);
                if (reliablityResult == -1)
                    return null;
                else if (reliablityResult == 0)
                    result += -20;
            }

            if (stint.ApplyChassisLevel)
            {
                int bonus = GetChassisBonus(driverResult.SeasonDriver.SeasonTeam, track);
                int statusBonus = (((int)driverResult.SeasonDriver.DriverStatus) * -2) + 2;
                result += (driverResult.SeasonDriver.SeasonTeam.Chassis + bonus + statusBonus);
            }

            return result;
        }

        public int GetDriverLevelBonus(SeasonDriver driver)
        {
            return driver.Skill + (2 - (2 * (int)driver.Style));
        }

        public int GetQualifyingBonus(int qualifyingPosition, int totalDriverCount)
        {
            return (totalDriverCount * 3) - (qualifyingPosition * 3);
        }

        public int GetChassisBonus(SeasonTeam team, Track track)
        {
            int bonus = 0;
            Dictionary<string, int> specs = new Dictionary<string, int>();
            specs.Add("Topspeed", team.Topspeed);
            specs.Add("Acceleration", team.Acceleration);
            specs.Add("Stability", team.Stability);
            specs.Add("Handling", team.Handling);

            var spec = (specs.SingleOrDefault(k => k.Key == track.Specification.ToString()));
            bonus = spec.Value;

            return bonus;
        }

        /// <summary>
        /// Performs a <see cref="SeasonDriver"/> reliability check.
        /// </summary>
        /// <param name="driver">The <see cref="SeasonDriver"/> to perform the reliability check on.</param>
        /// <returns>-1 if the reliability check fails, 1 if it succeeds, and 0 if it's neutral.</returns>
        public int GetDriverReliabilityResult(SeasonDriver driver, int dnfTrack)
        {
            var driverStyleModifier = ((int)driver.Style - 1);
            var reliabilityScore = driver.SeasonTeam.Reliability + driverStyleModifier + dnfTrack;
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

            // Swap drivers when the driver above is the second driver for the driver below
            foreach (var driver in orderedResults)
            {
                if (driver.SeasonDriver.DriverStatus == DriverStatus.First)
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

            return orderedResults.ToDictionary((res => res.DriverResultId), (res => orderedResults.IndexOf(res) + 1));
        }
    }
}
