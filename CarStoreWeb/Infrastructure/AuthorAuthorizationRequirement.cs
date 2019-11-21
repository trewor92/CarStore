using CarStoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Infrastructure
{
    public class AuthorAuthorizationRequirement : IAuthorizationRequirement
    {
        public bool AllowAuthors { get; set; }
    }

    public class AuthorAuthorizationHandler : AuthorizationHandler<AuthorAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorAuthorizationRequirement requirement)
        {
            Car car = context.Resource as Car;
            string user = context.User.Identity.Name;
            bool IsInAdmin = context.User.IsInRole("Admin");
            StringComparison compare = StringComparison.OrdinalIgnoreCase;

            if (car != null && user != null && ((requirement.AllowAuthors && car.Author.Equals(user, compare)) || IsInAdmin))
                context.Succeed(requirement);
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
