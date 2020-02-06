using FormuleCirkeltrek.Domain.Entities.Registrations;
using FormuleCirkeltrek.Domain.ValueObjects;
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
            builder.Property(x => x.Number)
                .HasConversion(
                v => v.ToString(),
                v => new DriverNumber(v));
        }
    }
}
