using AutoMapper;
using AutoMapper.QueryableExtensions;
using FormuleCirkeltrek.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormuleCirkeltrek.Application.Registrations.DriverRegistrations.Queries
{
    public class GetAllDriverRegistrations : IRequest<IEnumerable<DriverRegistrationDto>>
    {
        public class GetAllDriverRegistrationsHandler : IRequestHandler<GetAllDriverRegistrations, IEnumerable<DriverRegistrationDto>>
        {
            readonly IFormuleCirkeltrekDataLayer _dataLayer;
            readonly IMapper _mapper;

            public GetAllDriverRegistrationsHandler(IFormuleCirkeltrekDataLayer dataLayer, IMapper mapper)
            {
                _dataLayer = dataLayer;
                _mapper = mapper;
            }

            public async Task<IEnumerable<DriverRegistrationDto>> Handle(GetAllDriverRegistrations request, CancellationToken cancellationToken)
            {
                return await _dataLayer.DriverRegistrations
                    .ProjectTo<DriverRegistrationDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
        }
    }
}
