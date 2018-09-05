using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer4Example.Api.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
