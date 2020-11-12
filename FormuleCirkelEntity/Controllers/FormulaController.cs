using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Areas.Identity.Authorization;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Controllers
{
    public class FormulaController : Controller
    {
        protected readonly FormulaContext _context;
        protected readonly IdentityContext _identityContext;
        protected readonly IAuthorizationService _authorize;
        protected readonly UserManager<SimUser> _userManager;

        protected FormulaController(FormulaContext context,
            IdentityContext identityContext,
            IAuthorizationService authorizationService,
            UserManager<SimUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _authorize = authorizationService;
            _userManager = userManager;
        }

        protected static Task<IActionResult> AsTask(IActionResult result)
        {
            return Task.FromResult(result);
        }

        protected async Task<bool> IsUserOwner()
        {
            SimUser simuser = await _userManager.GetUserAsync(User);
            var result = (await _authorize.AuthorizeAsync(User, simuser, UserOperations.ManageSim)).Succeeded;
            return result;
        }
    }
}
