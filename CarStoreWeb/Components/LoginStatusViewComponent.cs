using CarStoreWeb.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarStoreWeb.Components
{
    public class LoginStatusViewComponent:ViewComponent
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;

        public LoginStatusViewComponent(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                if (HttpContext.User.IsInRole("Admin"))
                    return View("AdminLoggedIn", user);
                else
                    return View("LoggedIn", user);
            }
            else
            {
                var returnUrl = HttpContext.Request?.PathAndQuery();
                return View("Default", returnUrl);
            }
        }
    }
}
