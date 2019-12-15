using CarStoreWeb.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace CarStoreWeb.Tests.ComponentsTests
{
    public class LoginStatusViewComponentTests
    {
        Mock<UserManager<IdentityUser>> _mockUserManager;

        Mock<SignInManager<IdentityUser>> _mockSignInManager;

        LoginStatusViewComponent _loginStatusViewComponent;

        public LoginStatusViewComponentTests()
        {
            Mock<IUserStore<IdentityUser>> userStoreMock = new Mock<IUserStore<IdentityUser>>();

            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);

            Mock<IHttpContextAccessor> contextAccessor = new Mock<IHttpContextAccessor>();
            Mock<IUserClaimsPrincipalFactory<IdentityUser>> userPrincipalFactory =
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(_mockUserManager.Object,
            contextAccessor.Object, userPrincipalFactory.Object, null, null, null);

            _loginStatusViewComponent = new LoginStatusViewComponent(_mockSignInManager.Object, _mockUserManager.Object);

        }

        private void SetFakeHttpContext(Mock<HttpContext> mockHttpContext)
        {
            ViewContext viewContext = new ViewContext();
           
            viewContext.HttpContext = mockHttpContext.Object;
            _loginStatusViewComponent.ViewComponentContext = new ViewComponentContext()
            {
                ViewContext = viewContext
            };
        }

        [Fact]
        public void Can_Return_Correct_View_For_Admin()
        {
            ClaimsPrincipal fakeClaimsPrincipal = new ClaimsPrincipal();
            fakeClaimsPrincipal.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }));
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.User).Returns(fakeClaimsPrincipal);
            SetFakeHttpContext(mockHttpContext);
            _mockSignInManager.Setup(m => m.IsSignedIn(fakeClaimsPrincipal)).Returns(true);
            _mockUserManager.Setup(m => m.GetUserAsync(fakeClaimsPrincipal)).ReturnsAsync((IdentityUser)null);

            ViewViewComponentResult result = (ViewViewComponentResult) _loginStatusViewComponent.InvokeAsync().Result;

            Assert.Equal("AdminLoggedIn", result.ViewName);
        }

        [Fact]
        public void Can_Return_Correct_View_For_Logged_User()
        {
            ClaimsPrincipal fakeClaimsPrincipal = new ClaimsPrincipal();
            fakeClaimsPrincipal.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "NotAdmin") }));
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.User).Returns(fakeClaimsPrincipal);
            SetFakeHttpContext(mockHttpContext);
            _mockSignInManager.Setup(m => m.IsSignedIn(fakeClaimsPrincipal)).Returns(true);
            _mockUserManager.Setup(m => m.GetUserAsync(fakeClaimsPrincipal)).ReturnsAsync((IdentityUser)null);

            ViewViewComponentResult result = (ViewViewComponentResult)_loginStatusViewComponent.InvokeAsync().Result;

            Assert.Equal("LoggedIn", result.ViewName);
        }

        [Fact]
        public void Can_Return_Correct_View_For_Not_Authorized()
        {
            Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.Request).Returns((HttpRequest)null);
            SetFakeHttpContext(mockHttpContext);
            _mockSignInManager.Setup(m => m.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(false);

            ViewViewComponentResult result = (ViewViewComponentResult)_loginStatusViewComponent.InvokeAsync().Result;

            Assert.Equal("Default", result.ViewName);
        }
    }
}
