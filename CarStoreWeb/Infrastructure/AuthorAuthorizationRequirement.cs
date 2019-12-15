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
       
        private AppSettingsServiceRepository _serviceRepository;
        public AuthorAuthorizationHandler(AppSettingsServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
            
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       AuthorAuthorizationRequirement requirement)
        {
            string userName = _serviceRepository.GetUserName();
            Car car = context.Resource as Car;
            string user = context.User.Identity.Name;

            bool isInAdmin = context.User.IsInRole("Admin");
            bool isApiUser = car.ApiUser== userName;
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
