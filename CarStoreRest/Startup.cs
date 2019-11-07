using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarStoreRest
{
    public class Startup
    {
        IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            _configuration = new ConfigurationBuilder()
                           .SetBasePath(env.ContentRootPath)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json").Build();
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(_configuration["Data:CarStoreCars:ConnectionString"]));

            services.AddTransient<ICarRepository, EFCarRepository>();

            services.AddMvc();
        }

      
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            CarsSeedData.EnsurePopulated(app);

            /*
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
            */
        }
    }
}
