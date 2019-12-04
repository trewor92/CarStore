using Microsoft.EntityFrameworkCore;

namespace CarStoreWeb.Models
{
    [Owned]
    public class CarDescription
    {
        public string Color { get; set; }
        public int YearOfManufacture { get; set; }
        public string FuelType { get; set; }
        public double EngineСapacity { get; set; }
    }   
}
