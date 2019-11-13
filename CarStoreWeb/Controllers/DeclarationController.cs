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
        const int _pageSize = 5;
        public DeclarationController(ICarRepository repo)
        {
            _repository = repo;
        }
        public IActionResult List(string category, int pageNum)
        {            
            return View(new DeclarationListViewModel()
            {
                Cars = _repository.Cars
                    .Where(p => category == null || p.Brand == category)
                    .OrderBy(p => p.CarID)
                    .Skip((pageNum - 1) * _pageSize)
                    .Take(_pageSize),
                CurrentCategory = category,
                PagingInfo = new PagingInfo() {
                    CurrentPage = pageNum,
                    ItemsPerPage = _pageSize,
                    TotalItems = category == null ? _repository.Cars.Count() :
                    _repository.Cars.Count(c => c.Brand == category)
                }
            });
        }
    }
}