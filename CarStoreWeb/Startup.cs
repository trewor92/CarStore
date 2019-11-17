﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarStore
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

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
            services.AddTransient<ICarRepository>(s=>new RemoteCarRepository(_configuration["Data:CarStoreCars:WebApiUrl"]));
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            //app.UseMvcWithDefaultRoute();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "1",
                            template: "{controller}/{action}/{category}/{pageNum}/{sortOrder?}",
                            defaults: new { controller= "Declaration", action= "List" },
                            constraints:new { pageNum=new IntRouteConstraint()});

                routes.MapRoute(name: "2",
                            template: "{controller}/{action}/{pageNum?}/{sortOrder?}",
                            defaults: new { controller = "Declaration", action = "List" }
                            );
            //после сортировки в разделе home, вид сортировки переходит на другие разделы, только при такой маршрутизации
            });
        }
    }
}
