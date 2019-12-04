using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CarStoreRest.Models
{
    public static class IdentitySeedData
    {
        private const string User1 = "user1";
        private const string User2 = "user2";

        private const string passwords = "Secret123$";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();

            UserManager<IdentityUser> userManager = serviceScope.ServiceProvider
                .GetService<UserManager<IdentityUser>>();

            RoleManager<IdentityRole> roleManager = serviceScope.ServiceProvider
                .GetService<RoleManager<IdentityRole>>();

            if (userManager.Users.Count() == 0)
            {
                IdentityUser Usr1 = new IdentityUser(User1);
                await userManager.CreateAsync(Usr1, passwords);

                IdentityUser Usr2 = new IdentityUser(User2);
                await userManager.CreateAsync(Usr2, passwords);

            }
        }
    }
}
