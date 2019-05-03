using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
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
        public DbSet<Race> Races { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<SeasonTeam> SeasonTeams { get; set; }
        public DbSet<SeasonDriver> SeasonDrivers { get; set; }
        public DbSet<DriverResult> DriverResults { get; set; }
        public DbSet<TeamResult> TeamResults { get; set; }
        public DbSet<Qualification> Qualification { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Removes the Cascade Delete functionality related to relations between tables
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            //Makes table property unique
            builder.Entity<Driver>()
                .HasIndex(d => d.Abbreviation)
                .IsUnique();
            builder.Entity<Team>()
                .HasIndex(t => t.Abbreviation)
                .IsUnique();
            builder.Entity<Engine>()
                .HasIndex(e => e.Name)
                .IsUnique();
            builder.Entity<Race>()
                .Property(r => r.Stints)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, Stint>>(json) ?? new Dictionary<int, Stint>());
            builder.Entity<DriverResult>()
                .Property(r => r.StintResults)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, int?>>(json) ?? new Dictionary<int, int?>());
        }
    }
}
