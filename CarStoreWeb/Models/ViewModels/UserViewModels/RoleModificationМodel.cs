using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels.UserViewModels
{
    public class RoleModificationМodel
    {    
        [Required]
        public string RoleName {get; set;}
        public string RoleId {get; set;}
        public string[] IdsToAdd {get; set;}
        public string[] IdsToDelete{get; set;}
    }

}
