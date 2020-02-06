using AutoMapper;
using FormuleCirkeltrek.Application.Common.Mappings;
using FormuleCirkeltrek.Common.ValueObjects;
using FormuleCirkeltrek.Domain.Entities.Registrations;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Application.Registrations.DriverRegistrations.Queries
{
    public class DriverRegistrationDto : IMapFrom<DriverRegistration>
    {
        // Disable warning for uninitialized fields, as this is an AutoMapper DTO we can assume that Automapper takes care of properties being set correctly.
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Guid Id { get; internal set; }
        public DriverNumber Number { get; internal set; }
        public string Name { get; internal set; }
        public string Abbreviation { get; internal set; }
        public DateTime DateOfBirth { get; internal set; }
        public string Biography { get; internal set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public void Mapping(Profile profile)
            => profile.CreateMap<DriverRegistration, DriverRegistrationDto>();
    }
}
