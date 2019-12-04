﻿using System.ComponentModel.DataAnnotations;

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
