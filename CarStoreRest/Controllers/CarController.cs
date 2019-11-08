using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarStoreRest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : Controller
    {
        private ICarRepository _repository;
        public CarController(ICarRepository repo)
        {
            _repository = repo;
        }

        // GET: api/Car
        [HttpGet]
        public IActionResult Get()
        {
            var toReturn = _repository.Cars;

            if (toReturn == null)
                return NotFound();
            
            return Ok(toReturn);
        }

        // GET: api/Car/5
        [HttpGet("{carID}", Name = "Get")]
        public IActionResult Get(int carID)
        {
            var car = _repository.FindCar(carID);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/Car
        [HttpPost]
        public IActionResult Add([FromBody] Car car)
        {
            if (car == null)
                return BadRequest();

            var newCar = _repository.AddCar(car);
            return CreatedAtRoute(nameof(Get), new { carID = newCar.CarID }, newCar);
        }

        // PUT: api/Car/5
        [HttpPut]
        public IActionResult Edit([FromBody] Car car)
        {
            if (car == null)
                return BadRequest();

            var currentCar = _repository.FindCar(car.CarID);
            if (currentCar == null)
                return NotFound();

            _repository.EditCar(car);
            return CreatedAtRoute(nameof(Get), new { carID = currentCar.CarID }, currentCar);
        }

        // DELETE: api/Car/5
        [HttpDelete("{carID}")]
        public IActionResult Delete(int carID)
        {
            var currentCar = _repository.FindCar(carID);
            if (currentCar == null)
                return NotFound();

            _repository.DeleteCar(carID);

            return new NoContentResult();
        }
    }
}
