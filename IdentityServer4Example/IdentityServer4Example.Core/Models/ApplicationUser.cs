using Microsoft.AspNetCore.Identity;
using System;

namespace IdentityServer4Example.Core.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
