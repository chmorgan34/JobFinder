using JobFinder.Models;
using JobFinder.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobFinder.Controllers
{
    [Route("")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [Route("Login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                    return LocalRedirect(loginVM.ReturnUrl);
                else
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            
            return View(loginVM);
        }

        [HttpPost]
        [Authorize]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await signInManager.SignOutAsync();
            return LocalRedirect(returnUrl ?? "/");
        }

        [Route("Register")]
        public IActionResult Register(string returnUrl) => View(new RegisterViewModel { ReturnUrl = returnUrl ?? "/" });

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerVM.Username };
                var result = await userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return LocalRedirect(registerVM.ReturnUrl);
                }
                else foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(registerVM);
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied() => View();
    }
}
