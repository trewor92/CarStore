using System.ComponentModel.DataAnnotations;


namespace CarStoreRest.Models
{
    public class Token
    {
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
