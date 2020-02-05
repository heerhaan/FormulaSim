using FormuleCirkeltrek.Application.Common.Interfaces;
using FormuleCirkeltrek.Domain.Entities.Registrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Persistence
{
    public class FormuleCirkeltrekDbContext : DbContext, IFormuleCirkeltrekDataLayer
    {
        public FormuleCirkeltrekDbContext(DbContextOptions<FormuleCirkeltrekDbContext> options)
            : base(options)
        {
        }

        public DbSet<DriverRegistration> DriverRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FormuleCirkeltrekDbContext).Assembly);
        }
    }
}
