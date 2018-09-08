using IdentityServer4Example.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4Example.Api.Controllers
{
    public class UserController : Controller
    {
        UserManager<ApplicationUser> userManager;
        SignInManager<ApplicationUser> signInManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> Register(ApplicationUser user, string password)
        {
            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
                return new OkObjectResult(user);

            return new BadRequestObjectResult(result.Errors.Select(m => m.Description).ToList());
        }

        //public async Task<IActionResult> SignIn(string email, string password, bool isPersistent)
        //{
        //    var user = await userManager.FindByNameAsync(email);

        //    if (user == null)
        //        return new BadRequestObjectResult("User not found.");

        //    if (user.LockoutEnd != null)
        //        return new BadRequestObjectResult("Account locked.");

        //    var result = await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure: true);

        //    if(result.Succeeded)

        //}
    }
}