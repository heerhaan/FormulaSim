using System;
using System.Collections.Generic;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Services;
using Xunit;

namespace FormulaCirkelEntity.Tests
{
    public class RaceServiceTests
    {
        [Fact]
        public void DriverTraits_SetTo_DriverResult()
        {
            // Arrange
            DriverResult driverResult = new DriverResult();
            DriverTrait driverTrait1 = new DriverTrait { Trait = new Trait { QualyPace = 1 } };
            DriverTrait driverTrait2 = new DriverTrait { Trait = new Trait { QualyPace = 2 } };
            List<DriverTrait> driverTraits = new List<DriverTrait> { driverTrait1, driverTrait2 };
            // Act
            RaceService.SetDriverTraitMods(driverResult, driverTraits);
            // Assert
            int expected = 3; // QualyMod should add up all QualyPace which results in an expected of 3
            Assert.Equal(expected, driverResult.QualyMod);
        }

        [Fact]
        public void TeamTraits_SetTo_DriverResult()
        {
            // Arrange
            DriverResult driverResult = new DriverResult();
            TeamTrait teamTrait1 = new TeamTrait { Trait = new Trait { QualyPace = 1 } };
            TeamTrait teamTrait2 = new TeamTrait { Trait = new Trait { QualyPace = 2 } };
            List<TeamTrait> teamTraits = new List<TeamTrait> { teamTrait1, teamTrait2 };
            // Act
            RaceService.SetTeamTraitMods(driverResult, teamTraits);
            // Assert
            int expected = 3; // QualyMod should add up all QualyPace which results in an expected of 3
            Assert.Equal(expected, driverResult.QualyMod);
        }

        [Fact]
        public void TrackTraits_SetTo_DriverResult()
        {
            // Arrange
            DriverResult driverResult = new DriverResult();
            TrackTrait trackTrait1 = new TrackTrait { Trait = new Trait { QualyPace = 1 } };
            TrackTrait trackTrait2 = new TrackTrait { Trait = new Trait { QualyPace = 2 } };
            List<TrackTrait> trackTraits = new List<TrackTrait> { trackTrait1, trackTrait2 };
            // Act
            RaceService.SetTrackTraitMods(driverResult, trackTraits);
            // Assert
            int expected = 3; // QualyMod should add up all QualyPace which results in an expected of 3
            Assert.Equal(expected, driverResult.QualyMod);
        }
    }
}
