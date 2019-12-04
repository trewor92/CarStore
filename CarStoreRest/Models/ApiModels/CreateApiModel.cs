using System.ComponentModel.DataAnnotations;

namespace CarStoreRest.Models.ApiModels
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
