using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FormuleCirkelEntity.Areas.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;

namespace FormuleCirkelEntity.Areas.Identity.Pages.Account.Manage
{
    public class UsersIndexModel : PageModel
    {
        private readonly UserManager<SimUser> _userManager;
        private readonly IAuthorizationService _authorize;

        public UsersIndexModel(UserManager<SimUser> userManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _authorize = authorizationService;
        }

        public List<SimUser> Users { get; set; }

        private async Task LoadAsync()
        {
            var allusers = await _userManager.Users.ToListAsync();
            Users = allusers;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Checks if the current logged-in user is the sim owner
            SimUser simuser = await _userManager.GetUserAsync(User);
            var result = (await _authorize.AuthorizeAsync(User, simuser, UserOperations.ManageSim)).Succeeded;
            if (!result)
                return Forbid();

            await LoadAsync();
            return Page();
        }
    }
}
