using CarStoreWeb.Components;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CarStoreWeb.Tests.ComponentsTests
{
    public class NavigationMenuViewComponentTests
    {
        private List<Car> _carsList;
        private void SetSeedData()
        {
            _carsList = new List<Car> {

               new Car
               {
                   Author = "Admin",
                   ApiUser = "user1",
                   Brand = "BMW",
                   Model = "X5",
                   MobileNumber = "+375331234567",
                   CarDescription = new CarDescription
                   {
                       Color = "red",
                       FuelType = "diesel",
                       EngineСapacity = 3.5,
                       YearOfManufacture = 2015
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
               }};
        }

        [Fact]
        public void Can_Select_Categories_With_Counts()
        {
            Mock<ICarRepository> mock = new Mock<ICarRepository>();
            SetSeedData();
            mock.Setup(m => m.GetCarsAsync()).ReturnsAsync(_carsList);
            NavigationMenuViewComponent target =
                new NavigationMenuViewComponent(mock.Object);

            IDictionary<string,int> results = ((NavigationMenuViewComponentViewModel)(target.InvokeAsync().Result as ViewViewComponentResult)
             .ViewData.Model).Categories;

            string[] resultBrands = results.Keys.ToArray();
            int[] resultCounts = results.Values.ToArray();
            Assert.True(Enumerable.SequenceEqual((new string[] {"Audi","BMW", "Toyota" }).OrderBy(x=>x), resultBrands));
            Assert.True(Enumerable.SequenceEqual((new int[] { 1, 1, 1 }), resultCounts));
        }

        
        [Fact]
        public void Indicates_Select_Categories()
        {
            string categoryToSelect = "Audi";
            SetSeedData();
            Mock<ICarRepository> mock = new Mock<ICarRepository>();
            mock.Setup(m => m.GetCarsAsync()).ReturnsAsync(_carsList);
            NavigationMenuViewComponent target =
                new NavigationMenuViewComponent(mock.Object);
            target.ViewComponentContext = new ViewComponentContext
            {
                ViewContext = new ViewContext
                {
                    RouteData = new RouteData()
                }
            };
            target.RouteData.Values["category"] = categoryToSelect;

            string results = ((NavigationMenuViewComponentViewModel)(target.InvokeAsync().Result as ViewViewComponentResult)
             .ViewData.Model).CurrentCategory;

            Assert.Equal(categoryToSelect, results);            
        }
    }
}
