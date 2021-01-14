using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.DAL
{
    public class FormulaContext : IdentityDbContext<SimUser>
    {
        public FormulaContext(DbContextOptions<FormulaContext> options) : base(options) { }

        public DbSet<Championship> Championships { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<Race> Races { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Stint> Stints { get; set; }
        public DbSet<SeasonTeam> SeasonTeams { get; set; }
        public DbSet<SeasonDriver> SeasonDrivers { get; set; }
        public DbSet<DriverResult> DriverResults { get; set; }
        public DbSet<StintResult> StintResults { get; set; }
        public DbSet<Qualification> Qualification { get; set; }
        public DbSet<Trait> Traits { get; set; }
        public DbSet<DriverTrait> DriverTraits { get; set; }
        public DbSet<TeamTrait> TeamTraits { get; set; }
        public DbSet<TrackTrait> TrackTraits { get; set; }
        public DbSet<Tyre> Tyres { get; set; }
        public DbSet<Strategy> Strategies { get; set; }
        public DbSet<TyreStrategy> TyreStrategies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Regular null-check so no stuff that breaks will get called if builders appears to be null
            if (builder is null) return;
            // Uses the base class from Identity
            base.OnModelCreating(builder);
            // Makes it so that archived elements are usually hidden unless specified otherwise
            EnableArchivable(builder);
            //Removes the Cascade Delete functionality related to relations between tables
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            // Modify entities
            builder.Entity<Track>()
                .Property(t => t.LengthKM);
            builder.Entity<Season>()
                .Property(p => p.PointsPerPosition)
                .HasConversion(
                    dictionary => JsonConvert.SerializeObject(dictionary, Formatting.None),
                    json => JsonConvert.DeserializeObject<Dictionary<int, int?>>(json) ?? new Dictionary<int, int?>());
            builder.Entity<DriverTrait>()
                .HasKey(dt => new { dt.DriverId, dt.TraitId });
            builder.Entity<TeamTrait>()
                .HasKey(tt => new { tt.TeamId, tt.TraitId });
            builder.Entity<TrackTrait>()
                .HasKey(tr => new { tr.TrackId, tr.TraitId });

            // Applies seeded data for strategy and tyre, which are required
            builder.SeedStrategy();
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

        public DbSet<MinMaxDevRange> MinMaxDevRange { get; set; }

        #endregion
    }
}
