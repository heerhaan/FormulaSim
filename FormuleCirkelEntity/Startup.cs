using System;
using FormuleCirkelEntity.Builders;
﻿using FluentValidation.AspNetCore;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.ResultGenerators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FormuleCirkelEntity.Validation;
using FormuleCirkelEntity.Services;
using Microsoft.Extensions.Hosting;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace FormuleCirkelEntity
{
    public class Startup
    {
        public Startup(IHostEnvironment environment)
        {
            if (environment is null) { throw new NullReferenceException(); }
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc()
                .WithRazorPagesAtContentRoot()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson()
                .AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddDbContext<FormulaContext>(options => options.UseSqlServer(Configuration["DatabaseSettings:ConnectionString"]));
            services.AddDefaultIdentity<SimUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<FormulaContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 4;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "FormulaCookie";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(3);

                options.LoginPath = "/Accounts/Login";
                options.AccessDeniedPath = "/Accounts/AccessDenied";
                options.SlidingExpiration = true;
            });

            // Singleton services only get made once
            services.AddSingleton(new Random());
            // Scoped services
            services.AddScoped<IDataInitializer, DataInitializer>();
            services.AddScoped(typeof(IDataService<>), typeof(DataService<>));
            services.AddScoped(typeof(IChampionshipService), typeof(ChampionshipService));
            services.AddScoped(typeof(IDriverService), typeof(DriverService));
            services.AddScoped(typeof(IDriverResultService), typeof(DriverResultService));
            services.AddScoped(typeof(IEngineService), typeof(EngineService));
            services.AddScoped(typeof(IRubberService), typeof(RubberService));
            services.AddScoped(typeof(IRaceService), typeof(RaceService));
            services.AddScoped(typeof(ITeamService), typeof(TeamService));
            services.AddScoped(typeof(ITrackService), typeof(TrackService));
            services.AddScoped(typeof(ISeasonService), typeof(SeasonService));
            services.AddScoped(typeof(ISeasonDriverService), typeof(SeasonDriverService));
            services.AddScoped(typeof(ISeasonTeamService), typeof(SeasonTeamService));
            services.AddScoped(typeof(ITraitService), typeof(TraitService));
            services.AddScoped(typeof(ITyreStrategyService), typeof(TyreStrategyService));

            services.AddTransient<RaceResultGenerator>();
            services.AddTransient<RaceBuilder>();
            services.AddTransient<SeasonSettingsValidator>();
            services.AddTransient<PagingHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
                RequestPath = new PathString("/node_modules"),
                EnableDirectoryBrowsing = true
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
