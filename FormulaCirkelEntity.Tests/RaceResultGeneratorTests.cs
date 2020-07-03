using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ResultGenerators;
using System;
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
            RaceResultGenerator generator = new RaceResultGenerator(new Random(1));
            SeasonTeam team = new SeasonTeam
            {
                Topspeed = 4
            };
            Track track = new Track
            {
                Specification = spec
            };

            // Act
            int chassisBonus = generator.GetChassisBonus(team, track);

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
            RaceResultGenerator generator = new RaceResultGenerator(new Random(1));

            // Act
            var qualifyingBonus = generator.GetQualifyingBonus(qualifyingPosition, 26, 3);

            // Assert
            Assert.Equal(expected, qualifyingBonus);
        }

        class StaticRandom : Random
        {
            readonly int _staticValue;

            public StaticRandom(int staticValue)
            {
                _staticValue = staticValue;
            }

            public override int Next()
            {
                return _staticValue;
            }

            public override int Next(int maxValue)
            {
                return Next();
            }

            public override int Next(int minValue, int maxValue)
            {
                return Next();
            }
        }
    }
}
