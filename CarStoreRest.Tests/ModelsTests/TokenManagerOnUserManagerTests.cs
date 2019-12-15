using AutoMapper.Configuration;
using CarStoreRest.Infrastructure;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace CarStoreRest.Tests.ModelsTests
{
    public class TokenManagerOnUserManagerTests
    {
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private TokenManagerOnUserManager _tokenManagerOnUserManager;
        private string _loginProvider = "Admin";
        private string _userName = "Test";

        public TokenManagerOnUserManagerTests()
        {            
            Mock<IUserStore<IdentityUser>> userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);
            Mock<AppSettingsServiceRepository> appSettingsServiceRepository = new Mock<AppSettingsServiceRepository>(null);
            appSettingsServiceRepository.Setup(a => a.GetLoginProviderName()).Returns(_loginProvider);
            appSettingsServiceRepository.Setup(a => a.GetJwtKey()).Returns("SOME_RANDOM_KEY_MUST_BE_HERE");
            appSettingsServiceRepository.Setup(a => a.GetJwtExpireSec()).Returns(100);
            _tokenManagerOnUserManager = new TokenManagerOnUserManager(_mockUserManager.Object, appSettingsServiceRepository.Object);
        }

        private void SetupUserManager(IdentityUser user)
        {
            var fakeRefreshToken = "newToken";

            _mockUserManager.Setup(m => m.FindByNameAsync(_userName)).ReturnsAsync(user);
            _mockUserManager.Setup(m => m.RemoveAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken)))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.GenerateUserTokenAsync(user, _loginProvider, nameof(Token.RefreshToken)))
                            .ReturnsAsync(fakeRefreshToken);
            _mockUserManager.Setup(m => m.SetAuthenticationTokenAsync(user, _loginProvider, nameof(Token.RefreshToken), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
        }

        [Fact]
        public void Can_Generate_Token()
        {
            var user = new IdentityUser(_userName);
            SetupUserManager(user);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            Token result = _tokenManagerOnUserManager.GenerateToken(user).Result;
            JwtSecurityToken token = handler.ReadToken(result.AccessToken) as JwtSecurityToken;
            string sub = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
            string nameId = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.NameId).Value;

            Assert.False(String.IsNullOrEmpty(result.RefreshToken));
            Assert.True(sub== user.UserName);            
            Assert.True(nameId == user.Id);
        }

        [Fact]
        public void Can_Get_Identity_User_FromExpireTokenAsync()
        {
            IdentityUser user = new IdentityUser(_userName);
            SetupUserManager(user);

            Token result = _tokenManagerOnUserManager.GenerateToken(user).Result;
            IdentityUser resultUser = _tokenManagerOnUserManager.GetIdentityUserFromExpireTokenAsync(result.AccessToken).Result;

            Assert.Equal(user.UserName, resultUser.UserName);
        }

    }
}
