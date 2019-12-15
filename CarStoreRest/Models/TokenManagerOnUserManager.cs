using CarStoreRest.Infrastructure;
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
        private AppSettingsServiceRepository _appSettingsServiceRepository;

        public TokenManagerOnUserManager(UserManager<IdentityUser> userManager, AppSettingsServiceRepository appSettingsServiceRepository)
        {
            _userManager = userManager;
            _appSettingsServiceRepository = appSettingsServiceRepository;
            _loginProvider = _appSettingsServiceRepository.GetLoginProviderName();
        }

        public  async Task<Token> GenerateToken(IdentityUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
            string newRefreshToken = await _userManager.GenerateUserTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
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
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id)
            };


            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingsServiceRepository.GetJwtKey()));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddSeconds(_appSettingsServiceRepository.GetJwtExpireSec());

            JwtSecurityToken token = new JwtSecurityToken(
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettingsServiceRepository.GetJwtKey())),
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        public async Task<IdentityUser> GetIdentityUserFromExpireTokenAsync(string accessToken)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(accessToken);
            string userName = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;   // извлекаем имя из claim sub
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<string> GetSavedTokenAsync(IdentityUser user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken));
        }
    }
}
