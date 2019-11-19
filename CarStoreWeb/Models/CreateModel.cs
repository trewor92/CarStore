using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Compare(nameof(ConfirmPassword), ErrorMessage="Password and Confirm Password cannot be different")]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

    }
}
