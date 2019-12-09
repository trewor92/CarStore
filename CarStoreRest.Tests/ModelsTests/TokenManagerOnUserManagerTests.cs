using AutoMapper.Configuration;
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

            var inMemorySettings = new Dictionary<string, string>() { { "Data:AppSettings:LoginProviderName", _loginProvider },
                                                                        {"Data:AppSettings:JwtKey", "SOME_RANDOM_KEY_MUST_BE_HERE" },
                                                                        {"Data:AppSettings:JwtExpireSec", "100" } };
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenManagerOnUserManager = new TokenManagerOnUserManager(_mockUserManager.Object, configuration);
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
            var result = _tokenManagerOnUserManager.GenerateToken(user).Result;

            Assert.False(String.IsNullOrEmpty(result.RefreshToken));

           
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(result.AccessToken) as JwtSecurityToken;
            var sub = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value;
            var nameId = token.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.NameId).Value;
            
            Assert.True(sub== user.UserName);            
            Assert.True(nameId == user.Id);
        }

        [Fact]
        public void Can_Get_Identity_User_FromExpireTokenAsync()
        {
            var user = new IdentityUser(_userName);

            SetupUserManager(user);
            var result = _tokenManagerOnUserManager.GenerateToken(user).Result;
            IdentityUser resultUser = _tokenManagerOnUserManager.GetIdentityUserFromExpireTokenAsync(result.AccessToken).Result;

            Assert.Equal(user.UserName, resultUser.UserName);
        }

    }
}
