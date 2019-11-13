using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private ICarRepository _repository;
        public NavigationMenuViewComponent(ICarRepository repo)
        {
            _repository = repo;
        }
        public IViewComponentResult Invoke()
        {
            return View(new NavigationMenuViewComponentViewModel()
            {
                Categories = _repository.Cars.Select(x => x.Brand)
                                             .OrderBy(x => x)
                                             .GroupBy(x => x)
                                             .ToDictionary(x => x.Key, x => x.Count()),
                CurrentCategory = RouteData?.Values["category"]?.ToString()
            }); ; ; 
        }
    }
}
