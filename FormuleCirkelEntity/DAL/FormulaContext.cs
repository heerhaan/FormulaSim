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

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Engine> Engines { get; set; }
        public DbSet<Qualification> Qualifications { get; set; }
        public DbSet<Race> Races { get; set; }

        public DbSet<Season> Seasons { get; set; }


        public DbSet<Team> Teams { get; set; }

        public DbSet<Track> Tracks { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DriverResult>()
                .HasOne(a => a.Qualification)
                .WithOne(ab => ab.DriverResult)
                .HasForeignKey<Qualification>(c => c.DriverRef);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        
        public DbSet<FormuleCirkelEntity.Models.SeasonTeam> SeasonTeam { get; set; }

        
        public DbSet<FormuleCirkelEntity.Models.SeasonEngine> SeasonEngine { get; set; }
    }
}
