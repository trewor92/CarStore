using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using CarStoreWeb.Models.ViewModels.UserViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CarStoreWeb.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles="Admin")]
        public ViewResult Index()
        {
            return View(_userManager.Users);
        }

       
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel { ReturnUrl = returnUrl });
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user =
                    await _userManager.FindByNameAsync(loginModel.Name);

                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    if ((await _signInManager.PasswordSignInAsync(user,
                        loginModel.Password, false, false)).Succeeded)
                    {
                        return LocalRedirect(loginModel?.ReturnUrl ?? "~/Declaration/List");
                    }
                }
            }

            ModelState.AddModelError("", "Invalid name or Password");
            return View(loginModel);
        }


      
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View(new CreateModel());
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser()
                {
                    UserName = model.Name
                };
                IdentityResult result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                    return await Login(new LoginModel() { Name=model.Name, Password= model.Password});

                //return RedirectToAction("List","Declaration");
                else
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = "~/Declaration/List")
        {
            await _signInManager.SignOutAsync();
            return LocalRedirect(returnUrl);
        }


        [Authorize(Roles="Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(string userName)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(userName);

                if(user.UserName == HttpContext.User.Identity.Name)
                    return RedirectToAction("Index");
                
                IdentityResult result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction("Index");  
        }
    }
}