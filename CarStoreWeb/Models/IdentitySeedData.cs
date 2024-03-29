﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CarStoreWeb.Models
{
    public static class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminRole = "Admin";

        private const string passwords = "Secret123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            IServiceScope serviceScope = app.ApplicationServices.CreateScope();

            UserManager<IdentityUser> userManager = serviceScope.ServiceProvider
                .GetService<UserManager<IdentityUser>>();

            RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider
                .GetService<RoleManager<IdentityRole>>();

            if (roleManager.Roles.Count() == 0 && userManager.Users.Count() == 0)
            {
                IdentityRole adminRl = new IdentityRole(adminRole);
                await roleManager.CreateAsync(adminRl);

                IdentityUser adminUsr = new IdentityUser(adminUser);
                await userManager.CreateAsync(adminUsr, passwords);
                await userManager.AddToRoleAsync(adminUsr, adminRole);
            }
        }
    }
}

