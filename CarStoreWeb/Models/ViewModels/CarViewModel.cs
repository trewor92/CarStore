using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models.ViewModels
{
    public class CarViewModel
    {
        public int CarID { get; set; }
        public string ApiUser { get; set; }
        public string Author { get; set; }
        [Required(ErrorMessage = "Please enter а brand")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Please enter а model")]
        public string Model { get; set; }
        public CarDescription CarDescription { get; set; }
        [Required(ErrorMessage = "Please enter а price")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be positive")]
        public decimal? Price { get; set; }
        public bool IsAuthorized { get; set; }
    }
}
