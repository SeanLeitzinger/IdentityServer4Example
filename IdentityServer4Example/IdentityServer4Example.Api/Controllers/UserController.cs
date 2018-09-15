using IdentityServer4Example.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IdentityServer4Example.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        [Authorize]
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            return new OkObjectResult(new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    FirstName = "Mike",
                    LastName = "Stanford"
                },
                new ApplicationUser
                {
                    FirstName = "Jennfier",
                    LastName = "Smith"
                },
                new ApplicationUser
                {
                    FirstName = "John",
                    LastName = "Angel"
                }
            });
        }
    }
}