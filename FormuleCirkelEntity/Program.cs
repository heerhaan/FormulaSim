using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace FormuleCirkelEntity
{
    public static class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbInit = services.GetService<IDataInitializer>();
                    var context = services.GetRequiredService<FormulaContext>();
                    var userManager = services.GetRequiredService<UserManager<SimUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    dbInit.Initialize();
                    dbInit.SeedData();
                    await DataInitializer.SeedIdentity(userManager, roleManager);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e}");
                }
            }
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
