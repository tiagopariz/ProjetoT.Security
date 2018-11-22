using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetoT.Security.Services.WebApi.Controllers;
using Microsoft.Extensions.Configuration;
using ProjetoT.Security.Domain.Entities.Identity;
using ProjetoT.Security.Infra.Data.Context;
using ProjetoT.Security.Infra.Data.Policies;
using ProjetoT.Security.Infra.Data.Providers;
using IEmailSender = ProjetoT.Security.Infra.Messages.Email.Interfaces.IEmailSender;
using ProjetoT.Security.Infra.Messages.Services;

namespace ProjetoT.Security.Services.WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Identity an OAuth2 Server
            
            // ---------------------------
            services.AddDbContext<ProjetoTIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ProjetoTIdentityConnection")));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<ProjetoTIdentityDbContext>()
                .AddDefaultTokenProviders();

            // Add application services
            services.AddTransient<IEmailSender, EmailSender>();

            // Identity settings
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
            // ---------------------------


            services.AddAuthentication()
                .AddOAuthValidation()
                .AddOpenIdConnectServer(options =>
                {
                    options.UserinfoEndpointPath = $"api/v{ApiBaseController.CurrentApiVersion}/public/test/me";
                    options.TokenEndpointPath = $"api/v{ApiBaseController.CurrentApiVersion}/public/token";
                    options.AuthorizationEndpointPath = "/authorize/";
                    options.UseSlidingExpiration = false; // False means that new Refresh tokens aren't issued. Our implementation will be doing a no-expiry refresh, and this is one part of it.
                    options.AllowInsecureHttp = true; // ONLY FOR TESTING
                    options.AccessTokenLifetime = TimeSpan.FromHours(1); // An access token is valid for an hour - after that, a new one must be requested.
                    options.RefreshTokenLifetime = TimeSpan.FromDays(365 * 1000); //NOTE - Later versions of the ASOS library support `TimeSpan?` for these lifetime fields, meaning no expiration. 
                    // The version we are using does not, so a long running expiration of one thousand years will suffice.
                    options.AuthorizationCodeLifetime = TimeSpan.FromSeconds(60);
                    options.IdentityTokenLifetime = options.AccessTokenLifetime;
                    options.ProviderType = typeof(ProjetoTProvider);
                });

            services.AddScoped<ProjetoTProvider>();
            services.AddTransient<ValidationService>();
            services.AddTransient<TokenService>();
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            

            // MVC Settings
            // ---------------------------
            services
                .AddApiVersioning(o => o.ApiVersionReader =
                    new HeaderApiVersionReader("api-version"))
                .AddCors(
                    options => options
                        .AddPolicy("AllowLocalhostOrigin",
                            builder => builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()))
                .AddMvc(options =>
                {
                    options.OutputFormatters.RemoveType<TextOutputFormatter>();
                    options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            // ---------------------------
        }

        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app
                .UseHttpsRedirection()
                .UseCors("AllowLocalhostOrigin")
                .UseMvc(routes =>
                {
                    routes.MapRoute("default", "api/v" +
                                               ApiBaseController.CurrentApiVersion +
                                               "/public/{controller=register}/{action=register}/{id?}");
                });
        }
    }
}