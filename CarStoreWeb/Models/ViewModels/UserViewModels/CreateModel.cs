using System.ComponentModel.DataAnnotations;

namespace CarStoreWeb.Models.ViewModels.UserViewModels
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [UIHint("password")]
        [Compare(nameof(ConfirmPassword), ErrorMessage="Password and Confirm Password cannot be different")]
        public string Password { get; set; }

        [Required]
        [UIHint("password")]
        public string ConfirmPassword { get; set; }
    }
}
