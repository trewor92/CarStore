using CarStoreWeb.Controllers;
using CarStoreWeb.Models.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarStoreWeb.Tests.ControllersTests
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private Mock<SignInManager<IdentityUser>> _mockSignInManager;

        public AccountControllerTests()
        {
            Mock<IUserStore<IdentityUser>> userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            Mock<IHttpContextAccessor> contextAccessor = new Mock<IHttpContextAccessor>();
            Mock<IUserClaimsPrincipalFactory<IdentityUser>> userPrincipalFactory =
                new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(_mockUserManager.Object,
                contextAccessor.Object, userPrincipalFactory.Object, null, null, null);

            _accountController = new AccountController(_mockUserManager.Object, _mockSignInManager.Object);
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as ViewResult)?.ViewData.Model as T;
        }

        [Fact]
        public void Can_Login_User()
        {
            LoginModel loginModel = new LoginModel { Name = "Test", Password = "Secret" };
            _mockUserManager.Setup(m => m.FindByNameAsync(loginModel.Name)).ReturnsAsync(new IdentityUser(loginModel.Name));
            _mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);
            IdentityUser resultUser = null;
            string resultPassword = null;
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), false, false))
                             .Callback<IdentityUser, string, bool, bool>((i, s, b1, b2) => { resultUser = i; resultPassword = s; })
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
         
            IActionResult result = _accountController.Login(loginModel).Result;

            Assert.Equal(loginModel.Name, resultUser.UserName);
            Assert.Equal(loginModel.Password, resultPassword);
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public void Cannot_Login_Incorrect_Login_Or_Password()
        {
            LoginModel loginModel = new LoginModel { Name = "Test", Password = "IncorrectPassword" };
            _mockUserManager.Setup(m => m.FindByNameAsync(loginModel.Name)).ReturnsAsync((IdentityUser)null);

            IActionResult result = _accountController.Login(loginModel).Result;

            LoginModel returnedLoginModel = GetViewModel<LoginModel>(result);
            Assert.Equal(loginModel.Name, returnedLoginModel.Name);
            Assert.Equal(loginModel.Password, returnedLoginModel.Password);
        }

        [Fact]
        public void Can_Create_User()
        {
            CreateModel createModel = new CreateModel { Name = "Test", Password = "Secret123$"};
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), createModel.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.FindByNameAsync(createModel.Name)).ReturnsAsync(new IdentityUser(createModel.Name));
            _mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);
            IdentityUser resultUser = null;
            string resultPassword = null;
            _mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), false, false))
                             .Callback<IdentityUser, string, bool, bool>((i, s, b1, b2) => { resultUser = i; resultPassword = s; })
                             .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            IActionResult result = _accountController.Create(createModel).Result;

            Assert.Equal(createModel.Name, resultUser.UserName);
            Assert.Equal(createModel.Password, resultPassword);
            Assert.IsType<LocalRedirectResult>(result);
        }

        [Fact]
        public void Cannot_Create_Incorrect_User()
        {
            CreateModel createModel = new CreateModel { Name = "Test", Password = "Secret123$"};
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), createModel.Password))
                            .ReturnsAsync(IdentityResult.Failed());

            IActionResult result = _accountController.Create(createModel).Result;

            CreateModel returnedCreateModel = GetViewModel<CreateModel>(result);
            Assert.Equal(createModel.Name, returnedCreateModel.Name);
            Assert.Equal(createModel.Password, returnedCreateModel.Password);
        }

        private void SetFakeHttpContext(string userName)
        {
            ClaimsIdentity fakeIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userName) });
            Mock<HttpContext> mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(hc => hc.User.Identity).Returns(fakeIdentity);
            _accountController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
        }

        [Fact]
        public void Cannot_Delete_Himself()
        {
            string userName = "Test";
            IdentityUser testUser = new IdentityUser(userName);
            SetFakeHttpContext(userName);
            _mockUserManager.Setup(m => m.FindByNameAsync(userName)).ReturnsAsync(testUser);
            _mockUserManager.SetupGet(u => u.Users).Returns((IQueryable<IdentityUser>)null);

            RedirectToActionResult result = (RedirectToActionResult)_accountController.Delete(userName).Result;

            Assert.Equal("Index", result.ActionName);
            _mockUserManager.Verify(x => x.DeleteAsync(testUser), Times.Never);
        }

        [Fact]
        public void Cannot_Delete_Other_User()
        {
            string otherUserName = "OtherUser";
            IdentityUser otherUser = new IdentityUser(otherUserName);
            SetFakeHttpContext("Test");
            _mockUserManager.Setup(m => m.FindByNameAsync(otherUserName)).ReturnsAsync(otherUser);
            _mockUserManager.SetupGet(u => u.Users).Returns((IQueryable<IdentityUser>)null);
            _mockUserManager.Setup(u => u.DeleteAsync(otherUser)).ReturnsAsync(IdentityResult.Success);

            RedirectToActionResult result = (RedirectToActionResult)_accountController.Delete(otherUserName).Result;

            Assert.Equal("Index", result.ActionName);
            _mockUserManager.Verify(x => x.DeleteAsync(otherUser), Times.Once);
        }
    }
}
