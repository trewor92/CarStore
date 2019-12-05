
namespace CarStoreWeb.Models
{
    public class Car
    {
        public int CarID { get; set; }
        public string Author { get; set; }
        public string ApiUser { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public CarDescription CarDescription { get; set; }
        public decimal Price { get; set; }
        public string MobileNumber { get; set; }
    }
}
