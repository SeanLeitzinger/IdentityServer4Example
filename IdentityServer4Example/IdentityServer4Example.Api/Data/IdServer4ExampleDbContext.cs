using IdentityServer4Example.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityServer4Example.Api.Data
{
    public class IdServer4ExampleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public IdServer4ExampleDbContext(DbContextOptions<IdServer4ExampleDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasKey(p => p.Id);
            builder.Entity<ApplicationUser>().ToTable("ApplicationUser");

            base.OnModelCreating(builder);
        }
    }
}
