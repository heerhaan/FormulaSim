using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Identity;
using FormuleCirkelEntity.Models;

namespace FormuleCirkelEntity.Controllers
{
    public class FormulaController : Controller
    {
        protected readonly FormulaContext _context;
        protected readonly IdentityContext _identityContext;
        protected readonly UserManager<SimUser> _userManager;

        protected FormulaController(FormulaContext context,
            IdentityContext identityContext,
            UserManager<SimUser> userManager)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        protected static Task<IActionResult> AsTask(IActionResult result)
        {
            return Task.FromResult(result);
        }
    }
}
