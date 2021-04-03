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
        protected FormulaController(FormulaContext context,
            UserManager<SimUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        protected FormulaContext Context { get; }
        protected UserManager<SimUser> UserManager { get; }

        protected static Task<IActionResult> AsTask(IActionResult result)
        {
            return Task.FromResult(result);
        }
    }
}
