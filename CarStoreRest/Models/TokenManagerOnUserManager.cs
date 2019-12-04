using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CarStoreRest.Models
{
    public class TokenManagerOnUserManager : ITokenManager
    {
        private UserManager<IdentityUser> _userManager;
        private readonly string _loginProvider;
        private IConfiguration _configuration;

        public TokenManagerOnUserManager(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _loginProvider = _configuration["Data:AppSettings:LoginProviderName"];
        }

        public  async Task<Token> GenerateToken(IdentityUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
            await _userManager.SetAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken), newRefreshToken);

            string newAccessToken = GenerateAccessJwtToken(user);

            return new Token
            {
                AccessToken= newAccessToken,
                RefreshToken= newRefreshToken
            };
        }

        private string GenerateAccessJwtToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Data:AppSettings:JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddSeconds(Convert.ToDouble(_configuration["Data:AppSettings:JwtExpireSec"]));

            var token = new JwtSecurityToken(
                null,
                null,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Data:AppSettings:JwtKey"])),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);///
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<IdentityUser> GetIdentityUserFromExpireTokenAsync(string accessToken)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(accessToken);
            var userName = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;   // извлекаем имя из claim sub
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<string> GetSavedTokenAsync(IdentityUser user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
        }
    }
}
