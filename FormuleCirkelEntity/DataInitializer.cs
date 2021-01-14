using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity
{
    public static class DataInitializer
    {
        public static void SeedData(UserManager<SimUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUser(userManager);
        }

        public static void SeedUser(UserManager<SimUser> userManager)
        {
            if (userManager is null) { return; }
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

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager is null) { return; }
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole();
                role.Name = "Admin";
                _ = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                var role = new IdentityRole();
                role.Name = "Member";
                _ = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Guest").Result)
            {
                var role = new IdentityRole();
                role.Name = "Guest";
                _ = roleManager.CreateAsync(role).Result;
            }
        }
    }
}
