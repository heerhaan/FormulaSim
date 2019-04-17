using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ResultGenerators;
using System;
using Xunit;

namespace FormulaCirkelEntity.Tests
{
    public class RaceResultGeneratorTests
    {
        [Theory]
        [InlineData(Style.Agressief, 25)]
        [InlineData(Style.Neutraal, 20)]
        [InlineData(Style.Defensief, 15)]
        public void DriverLevelBonus_StyleAddition_Correct(Style style, int expected)
        {
            // Arrange
            RaceResultGenerator generator = new RaceResultGenerator();
            SeasonDriver driver = new SeasonDriver()
            {
                Skill = 20,
                Style = style
            };

            // Act
            int driverLevelBonus = generator.GetDriverLevelBonus(driver);

            // Assert
            Assert.Equal(expected, driverLevelBonus);
        }

        [Theory]
        [InlineData(1, 50)]
        [InlineData(26, 0)]
        [InlineData(5, 42)]
        [InlineData(10, 32)]
        [InlineData(15, 22)]
        [InlineData(20, 12)]
        public void DriverQualifyingBonus_Calculation_Correct(int qualifyingPosition, int expected)
        {
            // Arrange
            RaceResultGenerator generator = new RaceResultGenerator();

            // Act
            var qualifyingBonus = generator.GetQualifyingBonus(qualifyingPosition, 26);

            // Assert
            Assert.Equal(expected, qualifyingBonus);
        }
    }
}
