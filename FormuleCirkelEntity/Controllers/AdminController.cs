﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FormuleCirkelEntity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : FormulaController
    {
        public AdminController(FormulaContext context, 
            UserManager<SimUser> userManager)
            : base(context, userManager)
        { }

        public async Task<IActionResult> Index()
        {
            // Get the users, albeit not through the usermanager because of wanting to use Includes
            var users = await Context.Users
                .Include(res => res.Drivers)
                .Include(res => res.Teams)
                .ToListAsync();
            return View(users);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SimUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Modify(string userId)
        {
            var simuser = await UserManager.FindByIdAsync(userId);
            if (simuser is null) { return NotFound(); }
            return View(simuser);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            SimUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
            {
                ModelState.AddModelError("", "User couldn't be found");
            }

            return View("Index");
        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public async Task<IActionResult> AddDriverToUser(string userId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            AddDriverToUserModel viewModel = new AddDriverToUserModel(user, user.Drivers, GetDriversNotOwnedbyUser(user));
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDriverToUser(string userId, [Bind("driverId")] int driverId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            Driver driver = await Context.Drivers.FindAsync(driverId);
            if (user is null || driver is null)
                return NotFound();

            user.Drivers.Add(driver);
            _ = await UserManager.UpdateAsync(user);
            return RedirectToAction(nameof(AddDriverToUser), new { userId });
        }

        public async Task<IActionResult> RemoveDriverFromUser(string userId, int driverId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            Driver driver = await Context.Drivers.FindAsync(driverId);
            if (user is null || driver is null)
                return NotFound();

            user.Drivers.Remove(driver);
            _ = await UserManager.UpdateAsync(user);
            return RedirectToAction(nameof(AddDriverToUser), new { userId });
        }

        private List<Driver> GetDriversNotOwnedbyUser(SimUser user)
        {
            if (user is null)
                return null;

            var drivers = Context.Drivers
                .AsEnumerable()
                .Where(t => !user.Drivers.Contains(t))
                .ToList();

            return drivers;
        }

        public async Task<IActionResult> AddTeamToUser(string userId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            if (user is null)
                return NotFound();

            AddTeamToUserModel viewModel = new AddTeamToUserModel(user, user.Teams, GetTeamsNotOwnedByUser(user));
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddTeamToUser(string userId, [Bind("teamId")] int teamId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            Team team = await Context.Teams.FindAsync(teamId);
            if (user is null || team is null)
                return NotFound();

            user.Teams.Add(team);
            _ = await UserManager.UpdateAsync(user);
            return RedirectToAction(nameof(AddTeamToUser), new { userId });
        }

        public async Task<IActionResult> RemoveTeamFromUser(string userId, int teamId)
        {
            SimUser user = await UserManager.FindByIdAsync(userId);
            Team team = user.Teams.FirstOrDefault(t => t.Id == teamId);
            if (user is null || team is null)
                return NotFound();

            user.Teams.Remove(team);
            _ = await UserManager.UpdateAsync(user);
            return RedirectToAction(nameof(AddTeamToUser), new { userId });
        }

        private List<Team> GetTeamsNotOwnedByUser(SimUser user)
        {
            if (user is null)
                return null;

            var teams = Context.Teams
                .AsEnumerable()
                .Where(t => !user.Teams.Contains(t))
                .ToList();

            return teams;
        }

        public async Task<IActionResult> ModifyAppConfig()
        {
            return View(await Context.AppConfig.FirstOrDefaultAsync());
        }

        [HttpPost]
        public async Task<IActionResult> ModifyAppConfig(AppConfig appConfig)
        {
            if (ModelState.IsValid)
            {
                Context.Update(appConfig);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appConfig);
        }
    }
}
