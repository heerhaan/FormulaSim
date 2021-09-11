using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity
{
    public interface IDataInitializer
    {
        void Initialize();
        void SeedData();
    }

    public class DataInitializer : IDataInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DataInitializer (IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize ()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<FormulaContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        public void SeedData()
        {
            using (var serviceScope = _scopeFactory.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<FormulaContext>())
                {
                    // Sets the default setup for the application configuration
                    SeedConfig(context);
                    // Sets a default tyre and a strategy for it
                    SeedTyreData(context);
                    context.SaveChanges();
                }
            }
        }

        public static async Task SeedIdentity(UserManager<SimUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create the default Identity roles
            string[] defaultRoles = new string[] { "Admin", "Member", "Guest" };
            foreach (var elem in defaultRoles) { await AddRoleToIdentity(elem, roleManager); }

            // Create a default admin user
            if (!userManager.Users.Any())
            {
                var user = new SimUser { UserName = "admin" };
                var result = await userManager.CreateAsync(user, "password");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        private static async Task AddRoleToIdentity(string roleName, RoleManager<IdentityRole> roleManager)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                var newRole = new IdentityRole { Name = roleName };
                _ = await roleManager.CreateAsync(newRole);
            }
        }

        private static void SeedConfig(FormulaContext context)
        {
            if (!context.AppConfig.Any())
            {
                var appConfig = new AppConfig
                {
                    DisqualifyChance = 4,
                    MistakeLowerValue = -30,
                    MistakeUpperValue = -15,
                    RainAdditionalRNG = 10,
                    StormAdditionalRNG = 20,
                    SunnyEngineMultiplier = 0.9,
                    OvercastEngineMultiplier = 1.1,
                    WetEngineMultiplier = 1,
                    RainDriverReliabilityModifier = -3,
                    StormDriverReliabilityModifier = -5,
                    MistakeAmountRolls = 2,
                    ChassisModifierDriverStatus = 2
                };
                context.AppConfig.Add(appConfig);
            }
        }

        private static void SeedTyreData(FormulaContext context)
        {
            if (!context.Tyres.Any() && !context.Strategies.Any() && !context.TyreStrategies.Any())
            {
                // Add base Grooved tyre
                var groovedTyre = new Tyre
                {
                    Id = 1,
                    TyreName = "Grooved",
                    TyreColour = "#666699",
                    StintLen = 20,
                    Pace = 0,
                    MinWear = 0,
                    MaxWear = 0
                };
                context.Tyres.Add(groovedTyre);
                // Add default strategy
                var baseStrategy = new Strategy
                {
                    StrategyId = 1,
                    RaceLen = 20
                };
                context.Strategies.Add(baseStrategy);
                // Combine the default grooved tyre and strategy to a single TyreStrategy object
                var raceStrategy = new TyreStrategy
                {
                    TyreId = 1,
                    StrategyId = 1,
                    StintNumberApplied = 1
                };
                context.TyreStrategies.Add(raceStrategy);
            }
        }
    }
}
