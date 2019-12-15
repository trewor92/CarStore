using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using CarStoreWeb.Infrastructure;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CarStoreWeb.Controllers
{
    [Authorize]
    public class NoticeController : Controller
    {
        private IAuthorizationService _authService;
        private ICarRepository _repository;
        private readonly int _pageSize;
        private readonly IMapper _mapper;

        public NoticeController(ICarRepository repo, IAuthorizationService auth, AppSettingsServiceRepository serviceRepository, IMapper mapper)
        {
            _repository = repo;
            _authService = auth;
            _pageSize= serviceRepository.GetPageSize();
            _mapper = mapper;
        }
        [AllowAnonymous]
        public async Task<IActionResult> List(string category, int pageNum, SortState? sortOrder)
        {
            IEnumerable<Car> allCars = await _repository.GetCarsAsync();
            IEnumerable<Car> cars = allCars.Where(c => category == null || c.Brand == category);

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
            IEnumerable<CarViewModel> carsViewModel = cars.Select( c=>
            {
                CarViewModel carViewModel = GetCarViewModel(c);
                return carViewModel;
            });
            return View(new NoticeListViewModel()
            {
                CarViewModels = carsViewModel,
                CurrentCategory = category,
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = pageNum,
                    ItemsPerPage = _pageSize,
                    TotalItems = category == null ? allCars.Count() :
                    allCars.Count(c => c.Brand == category)
                },
                SortViewModel = new SortViewModel(sortOrder),
                IsAuthenticated = HttpContext.User.Identity.IsAuthenticated
            });
        }
        
        [HttpGet]
        public IActionResult AddItem()
        {
            return View("EditItem",new CarViewModel()); 
        }

        [HttpGet]
        public async Task<IActionResult> EditItem(int CarID)
        {
            bool isCarAuthor = AuthorizeSucceeded(CarID).Result;

            if (!isCarAuthor)
                return StatusCode(403);

            Car curentCar = await _repository.FindCarAsync(CarID);
            CarViewModel carViewModel = _mapper.Map<CarViewModel>(curentCar);

            return View(carViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditItem(CarViewModel carViewModel)
        {
            if (!ModelState.IsValid)
                return View(carViewModel);

            Car car = _mapper.Map<Car>(carViewModel);

            if (car.CarID == 0)
            {
                car.Author = HttpContext.User.Identity.Name;
                car=await _repository.AddCarAsync(car);
                TempData["message"] = $"{car.Brand} {car.Model} has been saved!";
                return RedirectToAction("Completed", new { CarID = car.CarID });
            }
            else  
            {
                bool isCarAuthor = AuthorizeSucceeded(car.CarID).Result;
              
                if (!isCarAuthor)
                    return StatusCode(403);

                await _repository.EditCarAsync(car);
                TempData["message"] = $"{car.Brand} {car.Model} has been edited!";
                return RedirectToAction(nameof(Completed),new {CarID= car.CarID });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int CarID)
        {
            Car car = await _repository.FindCarAsync(CarID);
            bool isCarAuthor = AuthorizeSucceeded(CarID).Result;
            if (car == null) 
                return StatusCode(404);
            if (isCarAuthor)
            {
                await _repository.DeleteCarAsync(CarID);
                TempData["message"] = $"{car.Brand} {car.Model} was deleted!";
                return RedirectToAction(nameof(List)); //its safety
            }
            else
                return StatusCode(403);
        }

        [HttpGet]
        public async Task<IActionResult> Completed(int CarID)
        {
            bool isCarAuthor = AuthorizeSucceeded(CarID).Result;
            if (isCarAuthor)
            {
                var car = await _repository.FindCarAsync(CarID);
                var carViewModel = GetCarViewModel(car);
                return View(carViewModel);
            }
            else
                return StatusCode(403);
        }

        [NonAction]
        private async Task<bool> AuthorizeSucceeded(int CarID)
        {
            Car car = await _repository.FindCarAsync(CarID);
            return await AuthorizeSucceeded(car);
        }

        [NonAction]
        private async Task<bool> AuthorizeSucceeded(Car car)
        {
            var authorized = await _authService.AuthorizeAsync(User, car, "Authors");
            return authorized.Succeeded;
        }
        [NonAction]
        private CarViewModel GetCarViewModel(Car car)
        {
            CarViewModel carViewModel = _mapper.Map<CarViewModel>(car);
            carViewModel.IsAuthorized = AuthorizeSucceeded(car).Result;
            return carViewModel;
        }

    }
}