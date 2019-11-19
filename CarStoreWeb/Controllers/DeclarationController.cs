using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CarStoreWeb.Controllers
{
    [Authorize]
    public class DeclarationController : Controller
    {
        private IAuthorizationService _authService;
        ICarRepository _repository;
        const int _pageSize = 3;
        public DeclarationController(ICarRepository repo, IAuthorizationService auth)
        {
            _repository = repo;
            _authService = auth;
        }
        [AllowAnonymous]
        public IActionResult List(string category, int pageNum, SortState? sortOrder)
        {
            var cars = _repository.Cars
                    .Where(c => category == null || c.Brand == category);

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
                Cars=cars,// = cars.Select(c => { c.ThisAuthor = HttpContext.User.Identity.Name == c.Author; return c; }),
                CurrentCategory = category,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = _pageSize,
                    TotalItems = category == null ? _repository.Cars.Count() :
                    _repository.Cars.Count(c => c.Brand == category)
                },
                SortViewModel = new SortViewModel(sortOrder),
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated
            });
        }
        
        [HttpGet]
        public IActionResult AddItem()
        {
            return View("EditItem",new Car()); 
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int CarID)
        {
            var car = _repository.FindCar(CarID);
            var authorized = await _authService.AuthorizeAsync(User, car, "Authors");

            if (!authorized.Succeeded)
                return StatusCode(403);

            return View(_repository.FindCar(CarID));
        }


        [HttpPost]
        public async Task<IActionResult> EditItem(Car car)
        {
            if (!ModelState.IsValid)
                return View(car);            

            if (car.CarID == 0)
            {
                car.Author = HttpContext.User.Identity.Name;
                car=_repository.AddCar(car);
                TempData["message"] = $"{car.Brand} {car.Model} has been saved!";
                return RedirectToAction("Completed", new { CarID = car.CarID });
            }
            else  //(car.CarID != 0)
            {
                var authorized = await _authService.AuthorizeAsync(User, car, "Authors");

                if (!authorized.Succeeded)
                    return StatusCode(403);

                _repository.EditCar(car);
                TempData["message"] = $"{car.Brand} {car.Model} has been edited!";
                return RedirectToAction("Completed",new {CarID= car.CarID });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int CarID)
        {
            var car = _repository.FindCar(CarID);
            var authorized = await _authService.AuthorizeAsync(User, car, "Authors");

            if (car != null && authorized.Succeeded)
            {
                _repository.DeleteCar(CarID);
                TempData["message"] = $"{car.Brand} {car.Model} was deleted!";
                return RedirectToAction("List"); //its safety
            }
            else
                return StatusCode(403);

        }

        public ViewResult Completed(int CarID)
        {
            var car = _repository.FindCar(CarID);
            return View(car);
        }
    }
}