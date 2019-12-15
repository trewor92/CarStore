using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CarStoreRest.Models
{
    public class CarsSeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            IServiceScope _serviceScope = app.ApplicationServices.CreateScope();
            ApplicationDbContext _context = _serviceScope.ServiceProvider
                .GetService<ApplicationDbContext>();

            if (!_context.Cars.Any())
            {
                _context.Cars.AddRange(
                new Car
                {
                    Author ="Admin",
                    ApiUser="user1",
                    Brand = "BMW",
                    Model = "X5",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "red",
                        FuelType = "diesel",
                        EngineСapacity = 3.5,
                        YearOfManufacture=2015
                    },
                    Price = 40000
                },
                new Car
                {
                    Author = "Admin",
                    ApiUser = "user2",
                    Brand = "Toyota",
                    Model = "Prius",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "white",
                        FuelType = "gasoline",
                        EngineСapacity = 1.5,
                        YearOfManufacture = 2007
                    },
                    Price = 10000
                },
                new Car
                {
                    Author = "Admin",
                    ApiUser = "user1",
                    Brand = "Audi",
                    Model = "A4",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "blue",
                        FuelType = "gasoline",
                        EngineСapacity = 1.6,
                        YearOfManufacture = 2003
                    },
                    Price = 8000
                },
                new Car
                {
                    Author = "user1",
                    ApiUser = "user2",
                    Brand = "Toyota",
                    Model = "Avensis",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "black",
                        FuelType = "gasoline",
                        EngineСapacity = 2.0,
                        YearOfManufacture = 2015
                    },
                    Price =14500
                },
                new Car
                {
                    Author = "user1",
                    Brand = "BMW",
                    ApiUser = "user1",
                    Model = "X3",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "pink",
                        FuelType = "diesel",
                        EngineСapacity = 2.5,
                        YearOfManufacture = 2010
                    },
                    Price = 25000
                },
                new Car
                {
                    Author = "user3",
                    ApiUser = "user2",
                    Brand = "Audi",
                    Model = "A6",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "dark-blue",
                        FuelType = "diesel",
                        EngineСapacity = 2.5,
                        YearOfManufacture = 2009
                    },
                    Price = 12000
                },
                new Car
                {
                    Author = "user1",
                    ApiUser = "user1",
                    Brand = "BMW",
                    Model = "525",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "silver",
                        FuelType = "gasoline",
                        EngineСapacity = 2.3,
                        YearOfManufacture = 2002
                    },
                    Price = 8000
                },
                new Car
                {
                    Author = "user2",
                    ApiUser = "user2",
                    Brand = "BMW",
                    Model = "320",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "black",
                        FuelType = "diesel",
                        EngineСapacity = 2.0,
                        YearOfManufacture = 2002
                    },
                    Price = 10000
                },
                new Car
                {
                    Author = "user3",
                    ApiUser = "user1",
                    Brand = "BMW",
                    Model = "750",
                    MobileNumber = "+375331234567",
                    CarDescription = new CarDescription
                    {
                        Color = "white",
                        FuelType = "gasoline",
                        EngineСapacity = 3.0,
                        YearOfManufacture = 2017
                    },
                    Price = 38000
                }); 
                _context.SaveChanges();
            }
        }
    }
}
