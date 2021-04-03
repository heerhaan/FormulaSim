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
        void SeedIdentity(UserManager<SimUser> userManager, RoleManager<IdentityRole> roleManager);
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
                    // Add base Grooved tyre
                    if (!context.Tyres.Any())
                    {
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
                    }
                    // Add default strategy
                    if (!context.Strategies.Any())
                    {
                        var baseStrategy = new Strategy
                        {
                            StrategyId = 1,
                            RaceLen = 20
                        };
                        context.Strategies.Add(baseStrategy);
                    }
                    // Combine the default grooved tyre and strategy to a single TyreStrategy object
                    if (!context.TyreStrategies.Any())
                    {
                        var raceStrategy = new TyreStrategy
                        {
                            TyreId = 1,
                            StrategyId = 1,
                            StintNumberApplied = 1
                        };
                        context.TyreStrategies.Add(raceStrategy);
                    }
                    context.SaveChanges();
                }
            }
        }

        public void SeedIdentity(UserManager<SimUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create the default Identity roles
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                _ = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Member"
                };
                _ = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Guest").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Guest"
                };
                _ = roleManager.CreateAsync(role).Result;
            }
            // Create a default admin user
            if (!userManager.Users.Any())
            {
                var user = new SimUser();
                user.UserName = "admin";
                var res = userManager.CreateAsync(user, "password").Result;

                if (res.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
