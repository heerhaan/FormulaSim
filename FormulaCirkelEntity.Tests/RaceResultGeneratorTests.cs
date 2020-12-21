using FormuleCirkelEntity.Utility;
using FormuleCirkelEntity.Models;
using System.Collections.Generic;
using Xunit;

namespace FormulaCirkelEntity.Tests
{
    public class RaceResultGeneratorTests
    {
        [Theory]
        [InlineData(4, Specification.Topspeed)]
        [InlineData(0, Specification.Handling)]
        public void ChassisTrackSpec_Calculation_Correct(int expected, Specification spec)
        {
            // Arrange
            Dictionary<string, int> teamSpecs = new Dictionary<string, int>
            {
                { "Topspeed", 4 },
                { "Acceleration", 0 },
                { "Handling", 0 }
            };
            Track track = new Track
            {
                Specification = spec
            };

            // Act
            int chassisBonus = Helpers.GetChassisBonus(teamSpecs, track.Specification.ToString());

            // Assert
            Assert.Equal(expected, chassisBonus);
        }

        [Theory]
        [InlineData(1, 75)]
        [InlineData(26, 0)]
        [InlineData(5, 63)]
        [InlineData(10, 48)]
        [InlineData(15, 33)]
        [InlineData(20, 18)]
        public void DriverQualifyingBonus_Calculation_Correct(int qualifyingPosition, int expected)
        {
            // Arrange
            // Act
            var qualifyingBonus = Helpers.GetQualifyingBonus(qualifyingPosition, 26, 3);

            // Assert
            Assert.Equal(expected, qualifyingBonus);
        }
    }
}
