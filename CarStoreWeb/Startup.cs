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
            services.AddSingleton<AppSettingsServiceRepository>();
            var servireProvider = services.BuildServiceProvider();
            var appSettingsServiceRepository = servireProvider.GetService<AppSettingsServiceRepository>();

            services.AddScoped<ICarRepository>(s => new RemoteCarRepository(
                s.GetRequiredService<AppSettingsServiceRepository>().GetCarUrl(),
                s.GetRequiredService<ITokenAuthenticator>()));              

            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITokenAuthenticator, CarStoreRestAuthenticator>(s=> { return new CarStoreRestAuthenticator(
                s.GetRequiredService<AppSettingsServiceRepository>().GetUserName(),
                s.GetRequiredService<AppSettingsServiceRepository>().GetHashPassword().Decrypt(),
                s.GetRequiredService<AppSettingsServiceRepository>().GetLoginUrl(),
                s.GetRequiredService<AppSettingsServiceRepository>().GetRefreshUrl());
            });
            services.AddMemoryCache();
            services.AddSession();
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(appSettingsServiceRepository.GetAppIdentityConnString()));
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseIdentity(); 

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
