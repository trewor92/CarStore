using CarStoreWeb.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Xunit;
using CarStoreWeb.Models;

namespace CarStoreWeb.Tests.InfrastructureTests
{
    public class AuthorAuthorizationHandlerTest : AuthorAuthorizationHandler
    {
        private ClaimsPrincipal _user;
        AuthorAuthorizationRequirement _requirement;
        private static string _apiUser="apiTest";
       
        private static Mock<AppSettingsServiceRepository> GetMockAppSettingsServiceRepository()
        {
            Mock<AppSettingsServiceRepository> mockAppSettingsServiceRepository = new Mock<AppSettingsServiceRepository>(null);
            mockAppSettingsServiceRepository.Setup(m => m.GetUserName()).Returns(_apiUser);

            return mockAppSettingsServiceRepository;
        }

        public AuthorAuthorizationHandlerTest() : base(GetMockAppSettingsServiceRepository().Object)
        {
            _requirement = new AuthorAuthorizationRequirement { AllowAuthors=true};       
        }

        private void SetupCurrentTestUserAndRole(string userName, string roleName)
        {
            _user = new ClaimsPrincipal(
                         new ClaimsIdentity(
                             new Claim[] {
                                new Claim(ClaimTypes.Name, userName),
                                new Claim(ClaimTypes.Role, roleName)
                             })
                         );
        }

        [Fact]
        public void AdminAndApiUserAuthorizationHandler_Should_Succeed()
        {
            string authorName = "Admin";
            SetupCurrentTestUserAndRole(authorName, "Admin");
            AuthorAuthorizationRequirement[] requirements = new[] { _requirement };
            Car testCar = new Car
            {
                Author = "OtherAuthor",
                ApiUser = _apiUser
            };
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(requirements, _user, testCar);

            this.HandleRequirementAsync(context, _requirement);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public void AuthorAndApiUserAuthorizationHandler_Should_Succeed()
        {
            string authorName = "Test";
            SetupCurrentTestUserAndRole(authorName, "user");
            AuthorAuthorizationRequirement[] requirements = new[] { _requirement };
            Car testCar = new Car
            {
                Author = authorName,
                ApiUser = _apiUser
            };
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(requirements, _user, testCar);

            this.HandleRequirementAsync(context, _requirement);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public void NotAuthorOrNotApiUserAuthorizationHandler_Should_Not_Succeed()
        {
            string authorName = "Test";
            SetupCurrentTestUserAndRole(authorName, "user");
            AuthorAuthorizationRequirement[] requirements = new[] { _requirement };
            Car testCarNotAuthor = new Car
            {
                Author = "NotAuthor",
                ApiUser = _apiUser
            };
            AuthorizationHandlerContext contextNotAuthor = new AuthorizationHandlerContext(requirements, _user, testCarNotAuthor);
            this.HandleRequirementAsync(contextNotAuthor, _requirement);
            Car testCarNotApiUser = new Car
            {
                Author = authorName,
                ApiUser = "NotApiUser"
            };
            AuthorizationHandlerContext contextNotApiUser = new AuthorizationHandlerContext(requirements, _user, testCarNotApiUser);

            this.HandleRequirementAsync(contextNotApiUser, _requirement);

            Assert.False(contextNotApiUser.HasSucceeded);
            Assert.False(contextNotAuthor.HasSucceeded);
        }
    }
}

