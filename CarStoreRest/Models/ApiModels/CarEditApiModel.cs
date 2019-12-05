using CarStoreWeb.Models;

namespace CarStoreRest.Models.ApiModels
{
    public class CarEditApiModel
    {
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarDescription CarDescription { get; set; }
        public decimal Price { get; set; }
        public string MobileNumber { get; set; }
    }
}
