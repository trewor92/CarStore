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
            return View(_repository.Cars);
        }
    }
}