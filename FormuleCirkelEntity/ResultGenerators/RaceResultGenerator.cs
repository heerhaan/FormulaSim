using FormuleCirkelEntity.Models;
using System;
using System.Linq;

namespace FormuleCirkelEntity.ResultGenerators
{
    public class RaceResultGenerator
    {
        public int GetStintResult(DriverResult driverResult, Stint stint)
        {
            var result = 0;
            if (stint.ApplyDriverLevel)
                result = result + GetDriverLevelBonus(driverResult.SeasonDriver);

            if (stint.ApplyQualifyingBonus)
                result = result + GetQualifyingBonus(driverResult.Grid, driverResult.SeasonDriver.Season.Drivers.Count);

            return result;
        }

        public int GetDriverLevelBonus(SeasonDriver driver)
        {
            return driver.Skill + (5 - (5 * (int)driver.Style));
        }

        public int GetQualifyingBonus(int qualifyingPosition, int totalDriverCount)
        {
            return (totalDriverCount * 2) - (qualifyingPosition * 2);
        }
    }
}
