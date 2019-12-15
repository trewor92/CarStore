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
using Xunit;

namespace CarStoreWeb.Tests.ControllersTests
{
    public class RoleAdminControllerTests
    {
        private Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private Mock<UserManager<IdentityUser>> _mockUserManager;
        private RoleAdminController _roleAdminController;
        public RoleAdminControllerTests()
        {
            Mock<IUserStore<IdentityUser>> userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStoreMock.Object,
                null, null, null, null, null, null, null, null);
            Mock<IRoleStore<IdentityRole>> roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(roleStoreMock.Object,
                null, null, null,null);
            _roleAdminController = new RoleAdminController(_mockRoleManager.Object, _mockUserManager.Object);
        }

        private void SetFakeHttpContext(string userName)
        {
            ClaimsIdentity fakeIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userName) });
            var user = new ClaimsPrincipal(fakeIdentity);
            HttpContext httpContext = new DefaultHttpContext() { User = user };
            _roleAdminController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        private void Get_Correct_RoleEditModel()
        {
            IdentityUser user1 = new IdentityUser();
            IdentityUser user2 = new IdentityUser();
            IdentityUser user3 = new IdentityUser();
            IdentityRole role = new IdentityRole("Admin");
            _mockUserManager.SetupGet(m => m.Users).Returns((new List<IdentityUser> { user1, user2, user3}).AsQueryable());
            _mockUserManager.Setup(m => m.IsInRoleAsync(user1, role.Name)).ReturnsAsync(true);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user2, role.Name)).ReturnsAsync(false);
            _mockUserManager.Setup(m => m.IsInRoleAsync(user3, role.Name)).ReturnsAsync(false);
            _mockRoleManager.Setup(m => m.FindByNameAsync("Admin")).ReturnsAsync(role);

            ViewResult result = (ViewResult)_roleAdminController.Edit().Result;
            RoleEditModel editModel = (RoleEditModel)result.Model;

            Assert.True(editModel.Members.Count()==1);
            Assert.True(editModel.NonМembers.Count() == 2);
            Assert.True(editModel.Role.Name == "Admin");
        }

        [Fact]
        private void Can_Edit_Other_Users_Roles()
        {
            SetFakeHttpContext("Tests");
            IdentityUser user1 = new IdentityUser("user1");
            IdentityUser user2 = new IdentityUser("user2");
            IdentityUser user3 = new IdentityUser("user3");
            IdentityRole role = new IdentityRole("Admin");
            RoleModificationМodel model = new RoleModificationМodel()
            {
                IdsToAdd = new string[] { user2.Id },
                IdsToDelete = new string[] { user1.Id },
                RoleName = "Admin"
            };
            _mockUserManager.Setup(m => m.FindByIdAsync(user1.Id)).ReturnsAsync(user1);
            _mockUserManager.Setup(m => m.FindByIdAsync(user2.Id)).ReturnsAsync(user2);
            _mockUserManager.Setup(m => m.FindByIdAsync(user3.Id)).ReturnsAsync(user3);
            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), model.RoleName)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), model.RoleName)).ReturnsAsync(IdentityResult.Success);

            IActionResult result = _roleAdminController.Edit(model).Result;

            _mockUserManager.Verify(m => m.AddToRoleAsync(user2, model.RoleName));
            _mockUserManager.Verify(m => m.RemoveFromRoleAsync(user1, model.RoleName));
        }

        [Fact]
        private void Cannot_Remove_Himself()
        {
            SetFakeHttpContext("user1");
            IdentityUser user1 = new IdentityUser("user1");
            IdentityUser user2 = new IdentityUser("user2");
            IdentityUser user3 = new IdentityUser("user3");
            IdentityRole role = new IdentityRole("Admin");
            RoleModificationМodel model = new RoleModificationМodel()
            {
                IdsToAdd = new string[] { user2.Id },
                IdsToDelete = new string[] { user1.Id },
                RoleName = "Admin"
            };
            _mockUserManager.Setup(m => m.FindByIdAsync(user1.Id)).ReturnsAsync(user1);
            _mockUserManager.Setup(m => m.FindByIdAsync(user2.Id)).ReturnsAsync(user2);
            _mockUserManager.Setup(m => m.FindByIdAsync(user3.Id)).ReturnsAsync(user3);
            _mockUserManager.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), model.RoleName)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), model.RoleName)).ReturnsAsync(IdentityResult.Success);

            IActionResult result = _roleAdminController.Edit(model).Result;

            _mockUserManager.Verify(m => m.AddToRoleAsync(user2, model.RoleName));
            _mockUserManager.Verify(m => m.RemoveFromRoleAsync(user1, model.RoleName),Times.Never);
        }
    }
}
