using CarStoreWeb.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public class CarDescription
    {
        [Required(ErrorMessage = "Please enter а color")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Please enter year of manufacture")]
        [IntYearMustBePast]
        public int? YearOfManufacture { get; set; }
        
        [Required(ErrorMessage = "Please enter fuel type")]
        public string FuelType { get; set; }

        [Range(0.1, 8, ErrorMessage = "Engine capacity must be from 0.1 to 8")]
        [Required(ErrorMessage = "Please enter engine capacity")]
        public double? EngineСapacity { get; set; }
    }
}
