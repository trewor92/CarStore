using CarStoreWeb.Models;

namespace CarStoreRest.Models.ApiModels
{
    public class CarAddApiModel
    {
        public string ApiUser { get; set; }
        public string Author { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarDescription CarDescription { get; set; }
        public decimal Price { get; set; }
    }

    
}
