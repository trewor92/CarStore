using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CarStoreRest.Infrastructure;
using CarStoreRest.Models;
using CarStoreWeb.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                           .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
           // services.AddCors(); //???

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(_configuration["Data:CarStoreCars:ApplicationDbContext:ConnectionString"]));
            services.AddDbContext<AppIdentityDbContext>(options =>
               options.UseSqlServer(_configuration["Data:CarStoreCars:AppIdentityDbContext:ConnectionString"]));

           services.AddMvc();//.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAutoMapper();

            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("Data").GetSection("CarStoreCars").GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);
                        var user = userService.GetById(userId);
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddScoped<IUserService, UserService>();
         

            services.AddTransient<ICarRepository, EFCarRepository>();
            
        }
      
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            
            app.UseStatusCodePages();
            app.UseStaticFiles();

     /*       app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

    */
            app.UseAuthentication();

            app.UseMvc();
            CarsSeedData.EnsurePopulated(app);
        }
    }
}