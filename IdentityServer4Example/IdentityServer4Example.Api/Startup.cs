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
using System.Reflection;

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
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdServer4ExampleDbContext>()
            .AddDefaultTokenProviders();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
            .AddInMemoryIdentityResources(Config.GetIdentityResources())
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddAspNetIdentity<ApplicationUser>();

            services.AddCors(options =>
            {
                options.AddPolicy("corspolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:49717")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://localhost:57770";
                options.RequireHttpsMetadata = false;
                options.ApiName = "exampleapi";
                options.ApiSecret = "secret";
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromMinutes(10);
            });

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
            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
