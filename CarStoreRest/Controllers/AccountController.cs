using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CarStoreRest.Infrastructure;
using CarStoreRest.Models;
using CarStoreRest.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarStoreRest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : Controller
    {
        private IUserService _userService;
        private readonly AppSettings _appSettings;
        private IMapper _mapper;

        public AccountController(IUserService userService, IOptions<AppSettings> appSettings, IMapper mapper)
        {
            _userService = userService;
            _appSettings = appSettings.Value
                ;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserViewModel userViewModel)
        {
            var user = _userService.Authenticate(userViewModel.Username, userViewModel.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserViewModel userViewModel)
        {
            var user = _mapper.Map<User>(userViewModel);
            try
            {
                // save 
                _userService.Create(user, userViewModel.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
                var userViewModels = _mapper.Map<IList<UserViewModel>>(users);
            return Ok(userViewModels);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            
            var userViewModel = _mapper.Map<UserViewModel>(user);
            return Ok(userViewModel);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserViewModel userViewModel)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userViewModel);
            user.Id = id;

            try
            {
                // save 
                _userService.Update(user, userViewModel.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}