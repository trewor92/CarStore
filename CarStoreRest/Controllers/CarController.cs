using AutoMapper;
using CarStoreRest.Models.ApiModels;
using CarStoreRest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public IActionResult Add([FromBody] CarAddApiModel carAddApiModel)
        {
            if (carAddApiModel == null)
                return BadRequest();

            var identity = HttpContext.User.Identity as ClaimsIdentity;
            carAddApiModel.ApiUser = identity.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

            Car car = _mapper.Map<Car>(carAddApiModel);
            Car newCar = _repository.AddCar(car);
            return CreatedAtRoute(nameof(Get), new { carID = newCar?.CarID }, newCar);
        }

        // PUT: api/Car/5
        [HttpPut("{carID}")]
        public IActionResult Edit([FromBody] CarEditApiModel carEditApiModel, int carID)
        {
            if (carEditApiModel == null)
                return BadRequest();

            var currentCar = _repository.FindCar(carID);
            if (currentCar == null)
                return NotFound();

            bool isCarAuthor = AuthorizeSucceeded(currentCar).Result;

            if (!isCarAuthor)
                return StatusCode(403);

            _mapper.Map<CarEditApiModel, Car>(carEditApiModel, currentCar);
            
            _repository.EditCar(currentCar, carID);
            return CreatedAtRoute(nameof(Get), new { carID = currentCar.CarID }, currentCar);
        }

        // DELETE: api/Car/5
        [HttpDelete("{carID}")]
        public IActionResult Delete(int carID)
        {
            var currentCar = _repository.FindCar(carID);
            if (currentCar == null)
                return NotFound();

            bool isCarAuthor = AuthorizeSucceeded(currentCar).Result;

            if (!isCarAuthor)
                return StatusCode(403);

            _repository.DeleteCar(carID);

            return new NoContentResult();
        }

        [NonAction]
        private async Task<bool> AuthorizeSucceeded(Car car)
        {

            var authorized = await _authService.AuthorizeAsync(User, car, "Authors");

            return authorized.Succeeded;

        }
    }
}
