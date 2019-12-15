using AutoMapper;
using CarStoreRest.Models.ApiModels;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CarStoreRest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : Controller
    {
        private readonly ICarRepository _repository;
        private IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public CarController(ICarRepository repo, IAuthorizationService authService, IMapper mapper)
        {
            _repository = repo;
            _authService = authService;
            _mapper = mapper;
        }

        // GET: api/Car
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Car> toReturn = await  _repository.GetCarsListAsync();

            if (toReturn == null)
                return NotFound();
            
            return Ok(toReturn);
        }

        // GET: api/Car/5
        [HttpGet("{carID}", Name = "Get")]
        public async Task<IActionResult> Get(int carID)
        {
            Car car = await _repository.FindCarAsync(carID);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: api/Car
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CarAddApiModel carAddApiModel)
        {
            if (carAddApiModel == null)
                return BadRequest();

            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            carAddApiModel.ApiUser = identity.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            Car car = _mapper.Map<Car>(carAddApiModel);
            Car newCar = await _repository.AddCarAsync(car);
            return CreatedAtRoute(nameof(Get), new { carID = newCar?.CarID }, newCar);
        }

        // PUT: api/Car/5
        [HttpPut("{carID}")]
        public async Task<IActionResult> Edit([FromBody] CarEditApiModel carEditApiModel, int carID)
        {
            if (carEditApiModel == null)
                return BadRequest();

            Car currentCar =await  _repository.FindCarAsync(carID);
            if (currentCar == null)
                return NotFound();

            bool isCarAuthor = AuthorizeSucceeded(currentCar).Result;

            if (!isCarAuthor)
                return StatusCode(403);

            _mapper.Map<CarEditApiModel, Car>(carEditApiModel, currentCar); 
            
            await _repository.EditCarAsync(currentCar, carID);
            return CreatedAtRoute(nameof(Get), new { carID = currentCar.CarID }, currentCar);
        }

        // DELETE: api/Car/5
        [HttpDelete("{carID}")]
        public async Task<IActionResult> Delete(int carID)
        {
            Car currentCar = await _repository.FindCarAsync(carID);
            if (currentCar == null)
                return NotFound();

            bool isCarAuthor = AuthorizeSucceeded(currentCar).Result;

            if (!isCarAuthor)
                return StatusCode(403);

            await _repository.DeleteCarAsync(carID);

            return new NoContentResult();
        }

        [NonAction]
        private async Task<bool> AuthorizeSucceeded(Car car)
        {
            AuthorizationResult authorized = await _authService.AuthorizeAsync(User, car, "Authors");
            return authorized.Succeeded;
        }
    }
}
