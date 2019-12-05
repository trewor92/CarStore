using AutoMapper;
using CarStoreWeb.Infrastructure;
using CarStoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
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
                                 .AddJsonFile($"appsettings.json")
                                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json")
                                 .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddTransient<ICarRepository>(s => new RemoteCarRepository(
                _configuration["Data:WebApiHostUrl"] + _configuration["Data:WebApiSettings:CarPath"],
                s.GetRequiredService<ITokenAuthenticator>()));              

            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITokenAuthenticator, CarStoreRestAuthenticator>(c=> { return new CarStoreRestAuthenticator(
                _configuration["Data:WebApiSettings:UserName"],
                _configuration["Data:WebApiSettings:CryptPassword"].Decrypt(),
                _configuration["Data:WebApiHostUrl"] + _configuration["Data:WebApiSettings:LoginPath"],
                _configuration["Data:WebApiHostUrl"] + _configuration["Data:WebApiSettings:RefreshPath"]);
            });

            services.AddMemoryCache();
            services.AddSession();
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(_configuration["Data:AppIdentityDbContext:ConnectionString"]));
            services.AddIdentity<IdentityUser, IdentityRole>()
               .AddEntityFrameworkStores<AppIdentityDbContext>();
            services.AddTransient<IAuthorizationHandler,AuthorAuthorizationHandler>();
            services.AddAuthorization(opts => {
                opts.AddPolicy("Authors", policy =>
                {
                    policy.AddRequirements(new AuthorAuthorizationRequirement
                    {
                        AllowAuthors = true
                    });
                });
            });
            services.AddAutoMapper();
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
            app.UseIdentity(); //obsolete

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "1",
                            template: "{controller}/{action}/{category}/{pageNum}/{sortOrder?}",
                            defaults: new { controller= "Notice", action= "List" },
                            constraints:new { pageNum=new IntRouteConstraint()});

                routes.MapRoute(name: "2",
                            template: "{controller}/{action}/{pageNum?}/{sortOrder?}",
                            defaults: new { controller = "Notice", action = "List" }
                            );
            
            });
            IdentitySeedData.EnsurePopulated(app);
        }
    }
}
