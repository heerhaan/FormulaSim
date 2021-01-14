using System;
using System.Text;
using System.Threading.Tasks;
using FormuleCirkelEntity.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using FormuleCirkelEntity.ViewModels;
using FormuleCirkelEntity.Models;
using FormuleCirkelEntity.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;

namespace FormuleCirkelEntity.Controllers
{
    public class AccountsController : FormulaController
    {
        private readonly SignInManager<SimUser> _signInManager;
        private readonly ILogger<AccountsController> _logger;
        // There is a possibility that the IEmailSender is setup as soon as I understand how that could work
        //private readonly IEmailSender _emailSender;

        public AccountsController(FormulaContext context, 
            UserManager<SimUser> userManager,
            SignInManager<SimUser> signInManager,
            ILogger<AccountsController> logger)
            : base(context, userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string statusmessage = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            ViewBag.statusmessage = statusmessage;
            return View(user);
        }

        public async Task<IActionResult> Login()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([Bind]LoginModel model)
        {
            if (ModelState.IsValid && model != null)
            {
                SimUser user = await _userManager.FindByNameAsync(model.Username);
                if (user is null)
                {
                    _logger.LogInformation("Unknown username attempted to log-in");
                    ModelState.AddModelError(string.Empty, "Username doesn't exist.");
                    return View();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    user.LastLogin = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _logger.LogInformation("User failed to log in.");
                    ModelState.AddModelError(string.Empty, "Password was incorrect.");
                    return View();
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid && registerModel != null)
            {
                var user = new SimUser { UserName = registerModel.Username, Email = registerModel.Email, LastLogin = DateTime.Now };
                var result = await _userManager.CreateAsync(user, registerModel.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    _ = await _userManager.AddToRoleAsync(user, Constants.RoleGuest);
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home", new { message = $"Registration was succesful.\nWelcome {registerModel.Username}!" });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            return View();
        }

        public async Task<IActionResult> ChangeAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            ChangeAccountModel viewModel = new ChangeAccountModel
            {
                Username = user.UserName,
                Email = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAccount(ChangeAccountModel model)
        {
            if (!ModelState.IsValid || model is null)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changeNameResult = await _userManager.SetUserNameAsync(user, model.Username);
            if (!changeNameResult.Succeeded)
            {
                foreach (var error in changeNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var changeEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!changeEmailResult.Succeeded)
            {
                foreach (var error in changeEmailResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // Checks if user also requested a password change
            if (!String.IsNullOrEmpty(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            // Returns the view if any of the previous changes experienced an error
            if (ModelState.ErrorCount > 0)
            {
                ChangeAccountModel viewModel = new ChangeAccountModel
                {
                    Username = user.UserName,
                    Email = user.Email
                };
                return View(viewModel);
            }
            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            return RedirectToAction("Index", new { statusmessage = "Your password has been changed." });
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            return RedirectToAction("Index", "Home", new { message = "Sorry, this feature is not implemented yet" });
            /*
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }
            return View();
            */
        }

        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                ResetPasswordModel viewModel = new ResetPasswordModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid || resetPasswordModel is null)
            {
                return View();
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Code, resetPasswordModel.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
