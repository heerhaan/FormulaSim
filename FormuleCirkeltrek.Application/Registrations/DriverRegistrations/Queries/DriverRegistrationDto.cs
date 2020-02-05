using AutoMapper;
using FormuleCirkeltrek.Application.Common.Mappings;
using FormuleCirkeltrek.Domain.Entities.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Application.Registrations.DriverRegistrations.Queries
{
    public class DriverRegistrationDto : IMapFrom<DriverRegistration>
    {
        public void Mapping(Profile profile) 
            => profile.CreateMap<DriverRegistration, DriverRegistrationDto>();
    }
}
