using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using FormuleCirkelEntity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FormuleCirkelEntity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : FormulaController
    {
        public AdminController(FormulaContext context, 
            IdentityContext identityContext, 
            UserManager<SimUser> userManager)
            : base(context, identityContext, userManager)
        { }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SimUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
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
            var simuser = await _userManager.FindByIdAsync(userId);
            if (simuser is null)
                return NotFound();

            return View(simuser);
        }
    }
}
