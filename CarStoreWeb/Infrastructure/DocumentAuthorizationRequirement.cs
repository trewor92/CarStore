using CarStoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Infrastructure
{
    public class DocumentAuthorizationRequirement : IAuthorizationRequirement
    {
        public bool AllowAuthors { get; set; }
    }

    public class DocumentAuthorizationHandler : AuthorizationHandler<DocumentAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       DocumentAuthorizationRequirement requirement)
        {
            Car car = context.Resource as Car;
            string user = context.User.Identity.Name;
            StringComparison compare = StringComparison.OrdinalIgnoreCase;

            if (car != null && user != null && (requirement.AllowAuthors && car.Author.Equals(user, compare)))
                context.Succeed(requirement);
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;

        }
    }
}
