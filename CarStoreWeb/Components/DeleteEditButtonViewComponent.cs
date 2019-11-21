using CarStoreWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Components
{
    public class DeleteEditButtonViewComponent : ViewComponent
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;

        public DeleteEditButtonViewComponent(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string author)
        {
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                bool IsInAdmin = HttpContext.User.IsInRole("Admin");

                if (user.UserName == author || IsInAdmin)
                    return View();
            }
            //return View(); - use for test
            return await Task.FromResult<IViewComponentResult>(Content(string.Empty));
        }
    }
}
