using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
  
    public static class IdentitySeedData
    {
        private const string adminUser1 = "user1";
        private const string adminUser2 = "user2";
        private const string adminUser3 = "user3";

        private const string adminPassword = "Secret123$";

        

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();

            UserManager<IdentityUser> userManager = serviceScope.ServiceProvider
                .GetService<UserManager<IdentityUser>>();

            IdentityUser user1 = await userManager.FindByIdAsync(adminUser1);
            IdentityUser user2 = await userManager.FindByIdAsync(adminUser2);
            IdentityUser user3 = await userManager.FindByIdAsync(adminUser3);
            if (user1 == null)
            {
                user1 = new IdentityUser(adminUser1);
                await userManager.CreateAsync(user1, adminPassword);
            }
            if (user2 == null)
            {
                user2 = new IdentityUser(adminUser2);
                await userManager.CreateAsync(user2, adminPassword);
            }
            if (user3 == null)
            {
                user3 = new IdentityUser(adminUser3);
                await userManager.CreateAsync(user3, adminPassword);
            }

        }
    }
}

