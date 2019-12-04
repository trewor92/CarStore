﻿using System.ComponentModel.DataAnnotations;

namespace CarStoreRest.Models.ApiModels
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
