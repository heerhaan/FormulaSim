using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FormuleCirkelEntity.Areas.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Areas.Identity.Data;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using System.ComponentModel.DataAnnotations;

namespace FormuleCirkelEntity.Areas.Identity.Pages.Account.Manage.Admin
{
    public class UserEditModel : PageModel
    {
        private readonly UserManager<SimUser> _userManager;
        private readonly IAuthorizationService _authorize;
        private readonly FormulaContext _context;

        public UserEditModel(UserManager<SimUser> userManager, IAuthorizationService authorizationService, FormulaContext context)
        {
            _userManager = userManager;
            _authorize = authorizationService;
            _context = context;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public List<Team> Teams { get; set; }
        public List<Driver> Drivers { get; set; }

        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "New username")]
            public string NewUsername { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        public async Task LoadAsync(SimUser user)
        {
            var username = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            Username = username;
            Email = email;
            UserId = user.Id;

            Input = new InputModel
            {
                NewUsername = username,
                NewEmail = email,
            };

            List<Team> userTeams = new List<Team>();
            var teams = _context.Teams.ToList();
            foreach (var team in user.Teams)
            {
                Team teamobj = teams.First(t => t.Id == team);
                userTeams.Add(teamobj);
            }

            List<Driver> userDrivers = new List<Driver>();
            var drivers = _context.Drivers.ToList();
            foreach (var driver in user.Drivers)
            {
                Driver driverobj = drivers.First(d => d.Id == driver);
                userDrivers.Add(driverobj);
            }

            Teams = userTeams;
            Drivers = userDrivers;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            // Checks if the current logged-in user is the sim owner
            SimUser simuser = await _userManager.GetUserAsync(User);
            var result = (await _authorize.AuthorizeAsync(User, simuser, UserOperations.ManageSim)).Succeeded;
            if (!result)
                return Forbid();

            var usermod = await _userManager.FindByIdAsync(id);
            if (usermod is null)
            {
                return NotFound($"Unable to find user with ID '{id}'.");
            }

            await LoadAsync(usermod);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            StatusMessage = "Nothing happened lol.";
            return RedirectToPage(new { UserId });
        }
    }
}
