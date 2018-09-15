using IdentityServer4Example.Api.Data;
using IdentityServer4Example.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityServer4Example.Api
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
            services.AddDbContext<IdServer4ExampleDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddDistributedMemoryCache();

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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(options =>
            {
                options.AddPolicy("corspolicy", policy =>
                {
                    policy.WithOrigins("https://localhost:44322")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:44386";
                options.RequireHttpsMetadata = false;
                options.ApiName = "exampleapi";
            });

            services.AddMvc(options => options.Filters.Add(new RequireHttpsAttribute()));

            services.AddAuthorization(c =>
            {
                c.AddPolicy("exampleapi", p => p.RequireClaim("scope", "exampleapi"));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors("corspolicy");
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCors("corspolicy");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseMvc();
        }
    }
}
