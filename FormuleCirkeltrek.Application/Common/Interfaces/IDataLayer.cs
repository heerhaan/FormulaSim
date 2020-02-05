using FormuleCirkeltrek.Domain.Entities.Registrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Application.Common.Interfaces
{
    public interface IFormuleCirkeltrekDataLayer
    {
        DbSet<DriverRegistration> DriverRegistrations { get; }
    }
}
