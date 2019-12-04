using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CarStoreWeb.Models.ViewModels.UserViewModels
{
    public class RoleEditModel
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<IdentityUser> Members { get; set; }
        public IEnumerable<IdentityUser> NonМembers { get; set; }   
    }
}
