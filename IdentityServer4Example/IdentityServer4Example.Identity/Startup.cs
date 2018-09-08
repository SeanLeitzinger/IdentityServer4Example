using IdentityServer4Example.Core.Models;
using IdentityServer4Example.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace IdentityServer4Example.Identity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("IdServer4ExampleConnection");
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration.GetSection("ApplicationOptions"));
            services.AddDbContext<IdServer4ExampleDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            var lockoutOptions = new LockoutOptions()
            {
                AllowedForNewUsers = true,
                DefaultLockoutTimeSpan = TimeSpan.FromDays(99999),
                MaxFailedAccessAttempts = 5
            };

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(option =>
            {
                option.Lockout = lockoutOptions;
                option.User = new UserOptions { RequireUniqueEmail = true };
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 12;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireLowercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<IdServer4ExampleDbContext>()
            .AddDefaultTokenProviders();
            var migrationsAssembly = typeof(IdServer4ExampleDbContext).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var options = new RewriteOptions()
               .AddRedirectToHttps();

            app.UseRewriter(options);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
