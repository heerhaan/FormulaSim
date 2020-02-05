using FormuleCirkeltrek.Domain.Entities.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Persistence.Configurations
{
    public class DriverRegistrationConfiguration : IEntityTypeConfiguration<DriverRegistration>
    {
        public void Configure(EntityTypeBuilder<DriverRegistration> builder)
        {
            builder.Ignore(x => x.Number);
        }
    }
}
