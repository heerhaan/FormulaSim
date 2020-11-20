using FormuleCirkelEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.DAL
{
    public class FormulaContext : DbContext
    {
        public FormulaContext(DbContextOptions<FormulaContext> options) : base(options) { }

        public DbSet<Championship> Championships { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<SeasonTeam> SeasonTeams { get; set; }
        public DbSet<SeasonDriver> SeasonDrivers { get; set; }
        public DbSet<DriverResult> DriverResults { get; set; }
        public DbSet<Qualification> Qualification { get; set; }
        public DbSet<Trait> Traits { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            EnableArchivable(builder);
            if (builder is null)
                return;
            //Removes the Cascade Delete functionality related to relations between tables
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            builder.Entity<Engine>()
                .HasIndex(e => e.Name)
                .IsUnique();
            builder.Entity<Track>()
                .Property(t => t.LengthKM);
            builder.Entity<Race>()
                .Property(r => r.Stints)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, Stint>>(json) ?? new Dictionary<int, Stint>());
            builder.Entity<Championship>()
                .Property(c => c.AgeDevRanges)
                .HasConversion(
                dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                json => JsonConvert.DeserializeObject<Dictionary<int, MinMaxDevRange>>(json) ?? new Dictionary<int, MinMaxDevRange>());
            builder.Entity<Championship>()
                .Property(c => c.SkillDevRanges)
                .HasConversion(
                dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                json => JsonConvert.DeserializeObject<Dictionary<int, MinMaxDevRange>>(json) ?? new Dictionary<int, MinMaxDevRange>());
            builder.Entity<DriverResult>()
                .Property(r => r.StintResults)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, int?>>(json) ?? new Dictionary<int, int?>());
            builder.Entity<Season>()
                .Property(p => p.PointsPerPosition)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, int?>>(json) ?? new Dictionary<int, int?>());
            builder.Entity<Track>()
                .Property(t => t.Traits)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, Trait>>(json) ?? new Dictionary<int, Trait>());
            builder.Entity<SeasonDriver>()
                .Property(t => t.Traits)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, Trait>>(json) ?? new Dictionary<int, Trait>());
            builder.Entity<SeasonTeam>()
                .Property(t => t.Traits)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, Trait>>(json) ?? new Dictionary<int, Trait>());
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual void Restore(IArchivable archivable)
            => archivable.Archived = false;

        #region Soft delete configuration

        void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IArchivable sd)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            sd.Archived = false;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Modified;
                            sd.Archived = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Sets a query filter to hide soft-deleted objects on any entity in the EF Model which implements the <see cref="IArchivable"/> interface.
        /// Also, sets the IsDeleted property as Indexed on any entity in the EF Model which implements the <see cref="IArchivable"/> interface.
        /// </summary>
        static void EnableArchivable(ModelBuilder builder)
        {
            SetSoftDeleteQueryFilters(builder);
            SetSoftDeleteIndexes(builder);
        }

        static void SetSoftDeleteIndexes(ModelBuilder builder)
        {
            foreach (var entityType in GetSoftDeleteEntityTypes(builder))
            {
                builder.Entity(entityType.ClrType)
                    .HasIndex(nameof(IArchivable.Archived));
            }
        }

        // Code taken from: https://stackoverflow.com/questions/47673524/ef-core-soft-delete-with-shadow-properties-and-query-filters/
        static void SetSoftDeleteQueryFilters(ModelBuilder builder)
        {
            foreach (var entityType in GetSoftDeleteEntityTypes(builder))
            {
                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterExpression<IArchivable>(e => !e.Archived, entityType.ClrType));
            }
        }

        static LambdaExpression ConvertFilterExpression<TInterface>(
            Expression<Func<TInterface, bool>> filterExpression,
            Type entityType)
        {
            var filterParameter = Expression.Parameter(entityType);
            var expression = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), filterParameter, filterExpression.Body);

            return Expression.Lambda(expression, filterParameter);
        }

        static IEnumerable<IMutableEntityType> GetSoftDeleteEntityTypes(ModelBuilder builder)
        {
            return builder.Model.GetEntityTypes()
                .Where(et => typeof(IArchivable).IsAssignableFrom(et.ClrType));
        }

        #endregion
    }
}
