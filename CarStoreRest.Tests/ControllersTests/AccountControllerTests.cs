using CarStoreRest.Controllers;
using CarStoreRest.Models;
using CarStoreRest.Models.ApiModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;


namespace CarStoreRest.Tests.ControllersTests
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private Mock<ITokenManager> _mockTokenManager;

        public AccountControllerTests()
        {
            _mockTokenManager = new Mock<ITokenManager>();
            Mock<IUserStore<IdentityUser>> userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);
            Mock<IHttpContextAccessor> contextAccessor = new Mock<IHttpContextAccessor>();
            Mock<IUserClaimsPrincipalFactory<IdentityUser>> userPrincipalFactory = 
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(_mockUserManager.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null);
            _accountController = new AccountController(_mockUserManager.Object, _mockSignInManager.Object, _mockTokenManager.Object);
        }

        [Fact]
        public void Can_Register_User()
        {
            CreateModel createModel = new CreateModel { Name = "Test", Password = "Secret123$" };
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), createModel.Password))
                            .ReturnsAsync(IdentityResult.Success);
            IdentityUser resultUser = null;
            _mockSignInManager.Setup(m => m.SignInAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                              .Callback<IdentityUser, bool, string>((i, b, s) => resultUser = i).Returns(Task.CompletedTask);

            Task<IActionResult> result = _accountController.Register(createModel);

            Assert.Equal("Test", resultUser.UserName);
            Assert.IsType<OkObjectResult>(result.Result);
        }
        [Fact]
        public void Cannot_Register_Not_Valid_User()
        {
            CreateModel createModel = new CreateModel { Name = "NotValidName", Password = "NotValidPassword" };

            _accountController.ModelState.AddModelError("testKey","testError");

            Assert.ThrowsAnyAsync<Exception>(()=>  _accountController.Register(createModel));
        }

        [Fact]
        public void Can_Login_User()
        {
            LoginModel loginModel = new LoginModel { Name = "Test", Password = "Secret" };
            _mockUserManager.Setup(m => m.FindByNameAsync(loginModel.Name)).ReturnsAsync(new IdentityUser(loginModel.Name));
            IdentityUser resultUser = null;
            string resultPassword = null;
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), false,false))
                             .Callback<IdentityUser, string, bool, bool>((i, s, b1,b2) => { resultUser = i; resultPassword = s; })
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockTokenManager.Setup(m => m.GenerateToken(It.IsAny<IdentityUser>())).ReturnsAsync(new Token());


            Task<object> result = _accountController.Login(loginModel);

            Assert.Equal(loginModel.Name, resultUser.UserName);
            Assert.Equal(loginModel.Password, resultPassword);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void Cannot_Login_Not_Valid_User()
        {
            LoginModel loginModel = new LoginModel { Name = "NotValidName", Password = "NotValidPassword" };

            _accountController.ModelState.AddModelError("testKey", "testError");

            Assert.ThrowsAnyAsync<Exception>(() => _accountController.Login(loginModel));
        }

        [Fact]
        public void Can_Refresh_Token_RefreshTokenIsValid()
        {
            Token oldToken = new Token { AccessToken = "TestAccessToken", RefreshToken = "TestRefreshToken" };
            Token newToken = new Token { AccessToken = "NewTestAccessToken", RefreshToken = "NewTestRefreshToken" };
            IdentityUser user = new IdentityUser("User");
            _mockTokenManager.Setup(m => m.GetIdentityUserFromExpireTokenAsync(oldToken.AccessToken))
                .ReturnsAsync(user);
            _mockTokenManager.Setup(m => m.GetSavedTokenAsync(user)).ReturnsAsync(oldToken.RefreshToken);
            _mockTokenManager.Setup(m => m.GenerateToken(user)).ReturnsAsync(newToken);

            Task<object> result = _accountController.Refresh(oldToken);

            OkObjectResult okObjectResult = (OkObjectResult)result.Result;
            Token resultToken = (Token)okObjectResult.Value;
            Assert.Equal(newToken.AccessToken, resultToken.AccessToken);
            Assert.Equal(newToken.RefreshToken, resultToken.RefreshToken);   
        }

        [Fact]
        public void Cannot_Refresh_Token_RefreshTokenIsInvalid()
        {
            Token oldToken = new Token { AccessToken = "TestAccessToken", RefreshToken = "TestRefreshInvalidToken" };
            IdentityUser user = new IdentityUser("User");
            _mockTokenManager.Setup(m => m.GetIdentityUserFromExpireTokenAsync(oldToken.AccessToken))
                .ReturnsAsync(user);
            _mockTokenManager.Setup(m => m.GetSavedTokenAsync(user)).ReturnsAsync("TestUserValidToken");

            Assert.ThrowsAnyAsync<Exception>(() => _accountController.Refresh(oldToken));
        }

        [Fact]
        public void Cannot_Login_Not_Valid_Token()
        {
            Token oldToken = new Token { AccessToken = "NotValidAccessToken", RefreshToken = "NotValidRefreshToken" };
            _accountController.ModelState.AddModelError("testKey", "testError");

            Assert.ThrowsAnyAsync<Exception>(() => _accountController.Refresh(oldToken));
        }
    }
}
