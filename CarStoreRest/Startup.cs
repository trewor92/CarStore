using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CarStoreRest.Infrastructure;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CarStoreRest
{
    public class Startup
    {
        IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            _configuration = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile($"appsettings.json")
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                           .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(_configuration["Data:ApplicationDbContext:ConnectionString"]));

            services.AddDbContext<AppIdentityDbContext>(options =>
               options.UseSqlServer(_configuration["Data:AppIdentityDbContext:ConnectionString"]));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders().AddTokenProvider(_configuration["Data:AppSettings:LoginProviderName"], typeof(DataProtectorTokenProvider<IdentityUser>));
        
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>{
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;})
                    .AddJwtBearer(cfg =>{
                        cfg.RequireHttpsMetadata = false;
                        cfg.SaveToken = true;
                        cfg.TokenValidationParameters = new TokenValidationParameters{
                            ValidateIssuer =false,
                            ValidateAudience = false,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Data:AppSettings:JwtKey"])),
                            ClockSkew = TimeSpan.Zero}; // remove delay of token when expire                                                
                        cfg.Events = new JwtBearerEvents{
                            OnAuthenticationFailed = context => { 
                                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                {
                                    context.Response.Headers.Add("Token-Expired", "true");
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });
            services.AddTransient<IAuthorizationHandler, AuthorAuthorizationHandler>();
            services.AddAuthorization(opts => {
                opts.AddPolicy("Authors", policy =>
                {
                    policy.AddRequirements(new AuthorAuthorizationRequirement
                    {
                       
                    });
                });
            });
            services.AddAutoMapper();
            services.AddMvc();
            services.AddTransient<ICarRepository, EFCarRepository>();
            services.AddTransient<ITokenManager, TokenManagerOnUserManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
            CarsSeedData.EnsurePopulated(app);
            IdentitySeedData.EnsurePopulated(app);
        }
    }
}