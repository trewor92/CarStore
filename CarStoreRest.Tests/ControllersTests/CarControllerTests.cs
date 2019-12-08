using System.Collections.Generic;
using Xunit;
using CarStoreRest.Models;
using Moq;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using CarStoreRest.Controllers;
using Microsoft.AspNetCore.Mvc;
using CarStoreRest.Models.ApiModels;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace CarStoreRest.Tests.ControllersTests
{
    public class CarControllerTests
    {
        private class TestAutoMapperProfile : Profile
        {
            public TestAutoMapperProfile()
            {
                CreateMap<CarAddApiModel, Car>();
                CreateMap<Car, CarAddApiModel>();

                CreateMap<CarEditApiModel, Car>();
                CreateMap<Car, CarEditApiModel>();
            }
        }
        private CarController _carController;
        private Mock<ICarRepository> _mockRepository;
        private IMapper _mapper;
        private readonly string _testUser = "Test";

        public CarControllerTests()
        {
            _mockRepository = new Mock<ICarRepository>();

            var mapperConf = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TestAutoMapperProfile());
            });
            _mapper = mapperConf.CreateMapper();

            Mock<IAuthorizationService> mockAuthService = new Mock<IAuthorizationService>();
            mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Success()));
            _carController = new CarController(_mockRepository.Object, mockAuthService.Object, _mapper);

            List<Claim> fakeClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, _testUser)
            };
            ClaimsIdentity fakeIdentity = new ClaimsIdentity(fakeClaims);

            Mock<HttpContext> mockContext = new Mock<HttpContext>(MockBehavior.Strict);
            mockContext.SetupGet(hc => hc.User.Identity).Returns(fakeIdentity);
            _carController.ControllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
        }

        private void AddInitialCarCollectionInMockRepository()
        {
            Car car1 = new Car
            {
                CarID = 1,
                Author = "Admin",
                ApiUser = "user1",
                Brand = "BMW",
                Model = "X5",
                MobileNumber = "+375331234567",
                CarDescription = new CarDescription
                {
                    Color = "red",
                    FuelType = "diesel",
                    EngineСapacity = 3.5,
                    YearOfManufacture = 2015
                },
                Price = 40000
            };

            Car car2 = new Car
            {
                CarID = 1,
                Author = "Admin",
                ApiUser = "user1",
                Brand = "Audi",
                Model = "A6",
                MobileNumber = "+375337654321",
                CarDescription = new CarDescription
                {
                    Color = "blue",
                    FuelType = "gasoline",
                    EngineСapacity = 2,
                    YearOfManufacture = 2010
                },
                Price = 10000
            };
            _mockRepository.Setup(m => m.Cars).Returns(new Car[] { car1, car2 });
        }

        private void SetupSuccessFindCar1InMockRepository()
        {
            AddInitialCarCollectionInMockRepository();
            _mockRepository.Setup(m => m.FindCar(1)).Returns(_mockRepository.Object.Cars.FirstOrDefault(c => c.CarID == 1));
        }

        private void SetupUnsuccessFindCar100InMockRepository()
        {
            _mockRepository.Setup(m => m.FindCar(100)).Returns((Car)null);
        }

        [Fact]
        public void Can_Delete_Car()
        {
            AddInitialCarCollectionInMockRepository();
            SetupSuccessFindCar1InMockRepository();
            var result = _carController.Delete(1);
            _mockRepository.Verify(m => m.DeleteCar(1));

            Assert.IsType<NoContentResult>(result);

        }

        [Fact]
        public void Cannot_Delete_Nonexisting_Car()
        {
            SetupUnsuccessFindCar100InMockRepository();
            var result = _carController.Delete(100);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Can_Edit_Car()
        {
            SetupSuccessFindCar1InMockRepository();

            CarEditApiModel car1EditApiModel = new CarEditApiModel
            {
                Brand = "Ford",
                Model = "Galaxy",
                MobileNumber = "+375337654321",
                CarDescription = new CarDescription
                {
                    Color = "blue",
                    FuelType = "gasoline",
                    EngineСapacity = 2.0,
                    YearOfManufacture = 2005
                },
                Price = 7000
            };

            Car resultCar = null;
            
            _mockRepository.Setup(r => r.EditCar(It.IsAny<Car>(), 1)).Callback<Car, int>((c,i) => resultCar = c);

            var result = _carController.Edit(car1EditApiModel, 1);
            Assert.IsType<CreatedAtRouteResult>(result);

            Assert.Equal(car1EditApiModel.Brand, resultCar.Brand);
            Assert.Equal(car1EditApiModel.Model, resultCar.Model);
            Assert.Equal(car1EditApiModel.MobileNumber, resultCar.MobileNumber);
            Assert.Equal(car1EditApiModel.Price, resultCar.Price);
            Assert.Equal(car1EditApiModel.CarDescription.Color, resultCar.CarDescription.Color);
            Assert.Equal(car1EditApiModel.CarDescription.FuelType, resultCar.CarDescription.FuelType);
            Assert.Equal(car1EditApiModel.CarDescription.EngineСapacity, resultCar.CarDescription.EngineСapacity);
            Assert.Equal(car1EditApiModel.CarDescription.YearOfManufacture, resultCar.CarDescription.YearOfManufacture);

            _mockRepository.Verify(m => m.EditCar(It.IsAny<Car>(), 1));

        }

        [Fact]
        public void Cannot_Edit_Nonexisting_Car()
        {
            SetupUnsuccessFindCar100InMockRepository();
            CarEditApiModel car1EditApiModel = new CarEditApiModel
            {
                Brand = "Ford",
                Model = "Galaxy",
                MobileNumber = "+375337654321",
                CarDescription = new CarDescription
                {
                    Color = "blue",
                    FuelType = "gasoline",
                    EngineСapacity = 2.0,
                    YearOfManufacture = 2005
                },
                Price = 7000
            };

            IActionResult result = _carController.Edit(car1EditApiModel, 100);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Can_Add_Car()
        {
            CarAddApiModel car1AddApiModel = new CarAddApiModel
            {
                Author="Author",
                ApiUser="ApiUser",
                Brand = "Ford",
                Model = "Galaxy",
                MobileNumber = "+375337654321",
                CarDescription = new CarDescription
                {
                    Color = "blue",
                    FuelType = "gasoline",
                    EngineСapacity = 2.0,
                    YearOfManufacture = 2005
                },
                Price = 7000
            };

            Car resultCar = null;
            _mockRepository.Setup(r => r.AddCar(It.IsAny<Car>())).Callback<Car>(c => resultCar = c);

            IActionResult result = _carController.Add(car1AddApiModel);
            Assert.IsType<CreatedAtRouteResult>(result);

            Assert.Equal(car1AddApiModel.Author, resultCar.Author);
            Assert.Equal(_testUser, resultCar.ApiUser);
            Assert.Equal(car1AddApiModel.Brand, resultCar.Brand);
            Assert.Equal(car1AddApiModel.Model, resultCar.Model);
            Assert.Equal(car1AddApiModel.MobileNumber, resultCar.MobileNumber);
            Assert.Equal(car1AddApiModel.Price, resultCar.Price);
            Assert.Equal(car1AddApiModel.CarDescription.Color, resultCar.CarDescription.Color);
            Assert.Equal(car1AddApiModel.CarDescription.FuelType, resultCar.CarDescription.FuelType);
            Assert.Equal(car1AddApiModel.CarDescription.EngineСapacity, resultCar.CarDescription.EngineСapacity);
            Assert.Equal(car1AddApiModel.CarDescription.YearOfManufacture, resultCar.CarDescription.YearOfManufacture);
        }

        [Fact]
        public void Can_Get_One_Car()
        {
            SetupSuccessFindCar1InMockRepository();
            IActionResult result = _carController.Get(1);
            _mockRepository.Verify(m => m.FindCar(1));
            OkObjectResult okObject = (OkObjectResult)result;
            Car carResult = (Car)okObject.Value;
            Assert.Equal(1, carResult.CarID);
        }

        [Fact]
        public void Cannot_Get_One_Nonexisting_Car()
        {
            SetupUnsuccessFindCar100InMockRepository();
            var result = _carController.Get(100);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Can_Get_Car()
        {
            AddInitialCarCollectionInMockRepository();
            IActionResult result = _carController.Get();

            OkObjectResult okObject = (OkObjectResult)result;
            IEnumerable<Car> carsResult = (IEnumerable<Car>)okObject.Value;
            Assert.Equal(2, carsResult.Count());
        }

    }
}
