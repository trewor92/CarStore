using System;
using System.Threading.Tasks;
using CarStoreRest.Models;
using CarStoreRest.Models.ApiModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
using CarStoreRest.Infrastructure;


namespace CarStoreRest.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenManager _tokenRepository;
        

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ITokenManager tokenRepository
           
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenRepository = tokenRepository;
            
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] CreateModel createModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser()
                {
                    UserName = createModel.Name
                };
                IdentityResult result = await _userManager.CreateAsync(user, createModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return Ok(createModel.Password.Crypt()); // return Crypt for test, in really project return UserName and Password!!!
                }
            }
            throw new ApplicationException("UNKNOWN_REGISTER_ERROR");
        }

        [HttpPost]
        public async Task<object> Login([FromBody] LoginModel loginModel)
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
                        return Ok(_tokenRepository.GenerateToken(user).Result);
                    }
                }
            }
            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        [HttpPost]
        public async Task<object> Refresh([FromBody] Token token)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _tokenRepository.GetIdentityUserFromExpireTokenAsync(token.AccessToken);

                if (user != null)
                {
                    var savedRefreshToken = await _tokenRepository.GetSavedTokenAsync(user); //retrieve the refresh token from a data store

                    if (savedRefreshToken != token.RefreshToken)
                        throw new ApplicationException("INVALID_REFRESHTOKEN_ATTEMPT");

                    return Ok(_tokenRepository.GenerateToken(user).Result);
                    
                }
                throw new ApplicationException("INVALID_DATA_IN_TOKEN");
            }
            throw new ApplicationException("INVALID_REFRESHTOKEN_ATTEMPT");
        }

        [Authorize]
        [HttpGet]
        public async Task<object> Protected()
        {
            return "Protected area";
        }
    }
}