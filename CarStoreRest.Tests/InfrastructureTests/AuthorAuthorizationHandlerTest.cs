using CarStoreRest.Infrastructure;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace CarStoreRest.Tests.InfrastructureTests
{

    public class AuthorAuthorizationHandlerTest : AuthorAuthorizationHandler
    {
        private string _author = "Test";
        private ClaimsPrincipal _user;
        Mock<AuthorAuthorizationRequirement> _requirement;

        public AuthorAuthorizationHandlerTest()
        {
            _requirement = new Mock<AuthorAuthorizationRequirement>();
            _user = new ClaimsPrincipal(
                         new ClaimsIdentity(
                            new Claim[] {
                                new Claim(JwtRegisteredClaimNames.Sub, _author)
                                })
                       );
        }  

        [Fact]
        public  void AuthorAuthorizationHandler_Should_Succeed()
        {
            AuthorAuthorizationRequirement[] requirements = new[] { _requirement.Object };
            Car testCar = new Car
            {
                ApiUser = _author
            };
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(requirements, _user, testCar);

            this.HandleRequirementAsync(context, _requirement.Object);

            Assert.True(context.HasSucceeded);
        }

        [Fact]
        public void AuthorAuthorizationHandler_Should_Unsucceed()
        {
            AuthorAuthorizationRequirement[] requirements = new[] { _requirement.Object };
            Car testCar = new Car
            {
                ApiUser = "OtherAuthor"
            };
            AuthorizationHandlerContext context = new AuthorizationHandlerContext(requirements, _user, testCar);

            this.HandleRequirementAsync(context, _requirement.Object);

            Assert.False(context.HasSucceeded);
        }
    }

}
