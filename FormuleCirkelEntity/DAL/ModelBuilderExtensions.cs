using Microsoft.EntityFrameworkCore;
using FormuleCirkelEntity.Models;
using System;

namespace FormuleCirkelEntity.DAL
{
    public static class ModelBuilderExtensions
    {
        public static void SeedStrategy(this ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) return;

            // Seeds a default grooved tyre for the application
            modelBuilder.Entity<Tyre>().HasData(
                new Tyre
                {
                    Id = 1,
                    TyreName = "Grooved",
                    TyreColour = "#666699",
                    StintLen = 20,
                    Pace = 0,
                    MinWear = 0,
                    MaxWear = 0
                }
            );
            // Creates a default strategy
            modelBuilder.Entity<Strategy>().HasData(
                new Strategy { StrategyId = 1, RaceLen = 20 });
            // Connects the default strategy with the default grooved tyre
            modelBuilder.Entity<TyreStrategy>().HasData(
                new TyreStrategy() { TyreStrategyId = 1, StrategyId = 1, TyreId = 1, StintNumberApplied = 1 });
        }
    }
}
