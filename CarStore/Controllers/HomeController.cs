using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Models;
using Microsoft.AspNetCore.Mvc;


namespace CarStoreWeb.Controllers
{
    public class HomeController : Controller
    {
        ICarRepository _repository;
        public HomeController(ICarRepository repo)
        {
            _repository = repo;
        }
        public IActionResult Index()
        {

            /*
            var a=_repository.AddCar(new Car { CarID = 15, Brand = "VW", Model = "Golf5", Description = "2005 year", Price = 3000 });
            var b = a;

            var  c = _repository.DeleteCar(33);
            var d = c;

            var k = _repository.FindCar(8);
            var x = k;
            

            _repository.EditCar(new Car { CarID = 10, Brand = "VW1", Model = "Golf5", Description = "2005 year", Price = 3000 });
           
            
            
            var m = _repository.FindCar(8);
            var n = m;
            */
            return View(_repository.Cars);
        }
    }
}