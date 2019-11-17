using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace CarStoreWeb.Controllers
{
    public class DeclarationController : Controller
    {
        ICarRepository _repository;
        const int _pageSize = 3;
        public DeclarationController(ICarRepository repo)
        {
            _repository = repo;
        }
        public IActionResult List(string category, int pageNum, SortState? sortOrder)
        {
            var cars = _repository.Cars
                    .Where(p => category == null || p.Brand == category);

            switch (sortOrder)
            {
                case SortState.YearOfManufactureAsc:
                    cars = cars.OrderBy(c => c.CarDescription.YearOfManufacture);
                    break;
                case SortState.YearOfManufactureDesc:
                    cars = cars.OrderByDescending(c => c.CarDescription.YearOfManufacture);
                    break;
                case SortState.FuelTypeAsc:
                    cars = cars.OrderBy(c => c.CarDescription.FuelType);
                    break;
                case SortState.FuelTypeDesc:
                    cars = cars.OrderByDescending(c => c.CarDescription.FuelType);
                    break;
                case SortState.EngineCapacityAsc:
                    cars = cars.OrderBy(c => c.CarDescription.EngineСapacity); ;
                    break;
                case SortState.EngineCapacityDesc:
                    cars = cars.OrderByDescending(c => c.CarDescription.EngineСapacity);
                    break;
                case SortState.PriceAsc:
                    cars = cars.OrderBy(c => c.Price); ;
                    break;
                case SortState.PriceDesc:
                    cars = cars.OrderByDescending(c => c.Price);
                    break;
                default:
                    cars = cars.OrderBy(c => c.CarID);
                    break;
            }
            cars = cars.Skip((pageNum - 1) * _pageSize)
                    .Take(_pageSize);
            return View(new DeclarationListViewModel()
            {
                Cars = cars,
                CurrentCategory = category,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = _pageSize,
                    TotalItems = category == null ? _repository.Cars.Count() :
                    _repository.Cars.Count(c => c.Brand == category)
                },
                SortViewModel = new SortViewModel(sortOrder)
            });
        }
        [HttpGet]
        public IActionResult AddItem()
        {
            return View(new Car());
        }

        [HttpPost]
        public IActionResult AddItem(Car car)
        {
            if (ModelState.IsValid)
            {
                _repository.AddCar(car);
                return RedirectToAction("Completed");
            }
            else
                return View(car);
        }

        [HttpPost]
        public IActionResult RemoveItem(int CarID, string returnUrl)
        {
            var car = _repository.FindCar(CarID);

            if (car != null)
            {
                _repository.DeleteCar(CarID);
            }

            return LocalRedirect(returnUrl); //its safety
        }

        public ViewResult Completed()
        {
            return View();
        }
    }
}