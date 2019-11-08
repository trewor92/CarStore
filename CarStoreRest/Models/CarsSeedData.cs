using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CarStoreRest.Models
{
    public class CarsSeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            var _serviceScope = app.ApplicationServices.CreateScope();
            ApplicationDbContext _context = _serviceScope.ServiceProvider
                .GetService<ApplicationDbContext>();

            if (!_context.Cars.Any())
            {
                _context.Cars.AddRange(
                new Car
                {
                    Brand = "BMW",
                    Model = "X5",
                    Description = "2005 year",
                    Price = 60000
                },
                new Car
                {   
                    Brand = "Toyota",
                    Model = "Prius",
                    Description = "2008 year",
                    Price = 10000
                },
                new Car
                {
                    Brand = "Audi",
                    Model = "A4",
                    Description = "2015 year",
                    Price = 12000
                },
                new Car
                {
                    Brand = "Toyota",
                    Model = "Avensis",
                    Description = "2000 year",
                    Price = 2500
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "X3",
                    Description = "2015 year",
                    Price = 25000
                },
                new Car
                {
                    Brand = "Audi",
                    Model = "A6",
                    Description = "2018 year",
                    Price = 30000
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "525",
                    Description = "2005 year",
                    Price = 8000
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "320",
                    Description = "2009 year",
                    Price = 10000
                },
                new Car
                {
                    Brand = "BMW",
                    Model = "750",
                    Description = "2019 year",
                    Price = 35000
                });
                _context.SaveChanges();
            }
        }
    }
}
