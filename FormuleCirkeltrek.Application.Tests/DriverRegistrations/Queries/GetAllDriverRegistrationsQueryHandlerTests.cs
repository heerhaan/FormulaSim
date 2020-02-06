using FormuleCirkeltrek.Application.Tests.Common;
using FormuleCirkeltrek.Domain.Entities.Registrations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static FormuleCirkeltrek.Application.Registrations.DriverRegistrations.Queries.GetAllDriverRegistrations;

namespace FormuleCirkeltrek.Application.Tests.DriverRegistrations.Queries
{
    public class GetAllDriverRegistrationsQueryHandlerTests
    {
        [Fact]
        public async Task Returns_Results()
        {
            // Arrange
            using var testContext = new QueryTestContext();
            using (var dataLayer = testContext.GetNewDataLayer())
            {
                var registration1 = new DriverRegistration()
                {
                    Name = "Hamilton",
                    Number = 33,
                    DateOfBirth = new DateTime(1990, 04, 02),
                    Abbreviation = "HAM",
                    Biography = "Best driver on the grid."
                };
                await dataLayer.AddAsync(registration1);
                await dataLayer.SaveChangesAsync();
            }

            // Act
            using (var dataLayer = testContext.GetNewDataLayer())
            {
                var sut = new GetAllDriverRegistrationsHandler(dataLayer, testContext.GetMapper());
                var result = await sut.Handle(new Registrations.DriverRegistrations.Queries.GetAllDriverRegistrations(), CancellationToken.None);

                Assert.Single(result);
                Assert.Single(result, (dr) =>
                    dr.Name == "Hamilton"
                    && dr.Number == 33
                    && dr.DateOfBirth == new DateTime(1990, 4, 2)
                    && dr.Abbreviation == "HAM"
                    && dr.Biography == "Best driver on the grid.");
            }
        }
    }
}
