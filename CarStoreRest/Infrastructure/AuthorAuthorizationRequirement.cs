using CarStoreRest.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarStoreRest.Infrastructure
{
    public class AuthorAuthorizationRequirement : IAuthorizationRequirement
    {
        
    }
    public class AuthorAuthorizationHandler : AuthorizationHandler<AuthorAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorAuthorizationRequirement requirement)
        {

            var identity = context.User.Identity as ClaimsIdentity;
            var user = identity.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            Car car = context.Resource as Car;

            if (car.ApiUser==user)
                context.Succeed(requirement);
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
            
        }
    }
}
