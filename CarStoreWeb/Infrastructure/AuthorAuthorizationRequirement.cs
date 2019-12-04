using CarStoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace CarStoreWeb.Infrastructure
{
    public class AuthorAuthorizationRequirement : IAuthorizationRequirement
    {
        public bool AllowAuthors { get; set; }
    }
    public class AuthorAuthorizationHandler : AuthorizationHandler<AuthorAuthorizationRequirement>
    {
        private  readonly string _userName;
        public AuthorAuthorizationHandler(IConfiguration configuration)
        {
            _userName = configuration["Data:WebApiSettings:UserName"];
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorAuthorizationRequirement requirement)
        {
            Car car = context.Resource as Car;
            string user = context.User.Identity.Name;

            bool isInAdmin = context.User.IsInRole("Admin");
            bool isApiUser = car.ApiUser== _userName;
            bool isAuthor = car.Author== user;

            if (car != null && user != null && isApiUser && (isAuthor || isInAdmin)) 
                context.Succeed(requirement);
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
