using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.DAL
{
    public class FormulaContext : DbContext
    {
        public FormulaContext(DbContextOptions<FormulaContext> options) : base(options) { }

        public DbSet<DriverDetails> DriverDetails { get; set; }
        public DbSet<Drivers> Drivers { get; set; }
        public DbSet<DriverStandings> DriverStandings { get; set; }
        public DbSet<EngineDetails> EngineDetails { get; set; }
        public DbSet<Engines> Engines { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Races> Races { get; set; }
        public DbSet<Results> Results { get; set; }
        public DbSet<Seasons> Seasons { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<TeamDetails> TeamDetails { get; set; }
        public DbSet<Teams> Teams { get; set; }
        public DbSet<TeamStandings> TeamStandings { get; set; }
        public DbSet<Tracks> Tracks { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
