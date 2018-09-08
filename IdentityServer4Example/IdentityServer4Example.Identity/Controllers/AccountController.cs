using IdentityServer4Example.Core.Models;
using IdentityServer4Example.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace IdentityServer4Example.Identity.Controllers
{
    public class AccountController : Controller
    {
        ApplicationOptions applicationOptions;
        string invalidUserIdOrPassword = "The user id or password was not correct.";
        SignInManager<ApplicationUser> signInManager;
        UserManager<ApplicationUser> userManager;

        public AccountController(IOptions<ApplicationOptions> applicationOptions, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            this.applicationOptions = applicationOptions.Value;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return Redirect(returnUrl);

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (returnUrl == null)
                returnUrl = applicationOptions.IdentityServer4ExampleWeb;

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, invalidUserIdOrPassword);

                    return View();
                }

                if (user.LockoutEnd != null)
                {
                    ModelState.AddModelError(string.Empty, "Account locked.");

                    return View(model);
                }

                var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                    return Redirect(returnUrl);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked.");

                    return View(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, invalidUserIdOrPassword);

                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    return RedirectToAction("Login");
                }

                AddErrors(result);
            }

            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}