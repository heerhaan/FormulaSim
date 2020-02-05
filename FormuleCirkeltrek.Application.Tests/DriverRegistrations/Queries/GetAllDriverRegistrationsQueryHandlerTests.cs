using FormuleCirkeltrek.Application.Tests.Common;
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
            using var testContext = new QueryTestContext();

            using (var dataLayer = testContext.GetNewDataLayer())
            {
                var sut = new GetAllDriverRegistrationsHandler(dataLayer, testContext.GetMapper());
                var result = await sut.Handle(new Registrations.DriverRegistrations.Queries.GetAllDriverRegistrations(), CancellationToken.None);
            }
        }
    }
}
