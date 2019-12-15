using AutoMapper;
using CarStoreWeb.Controllers;
using CarStoreWeb.Infrastructure;
using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CarStoreWeb.Tests.ControllersTests
{
    public class TestAutoMapperProfile : Profile
    {
        public TestAutoMapperProfile()
        {
            CreateMap<CarViewModel, Car>();
            CreateMap<Car, CarViewModel>();
        }
    }
    public class NoticeControllerTests
    {
        private Mock<ICarRepository> _mockRepository;
        private readonly IMapper _mapper;
        private NoticeController _noticeController;
        private Mock<IAuthorizationService> _mockAuthService;

        public NoticeControllerTests()
        {
            _mockAuthService = new Mock<IAuthorizationService>();

            MapperConfiguration mapperConf = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TestAutoMapperProfile());
            });
            _mapper = mapperConf.CreateMapper();
        
            Mock<AppSettingsServiceRepository> mockAppSettingsServiceRepository = new Mock<AppSettingsServiceRepository>(null);
            mockAppSettingsServiceRepository.Setup(m => m.GetPageSize()).Returns(2);

            _mockRepository = new Mock<ICarRepository>();

            HttpContext httpContext = new DefaultHttpContext();
            TempDataDictionary tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _noticeController = new NoticeController(_mockRepository.Object, _mockAuthService.Object, mockAppSettingsServiceRepository.Object, _mapper)
            {
                TempData = tempData,
            };
        }

        private void SetFakeHttpContext(string userName)
        {
            ClaimsIdentity fakeIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, userName) });
            var user = new ClaimsPrincipal(fakeIdentity);
            HttpContext httpContext = new DefaultHttpContext() { User = user };
            _noticeController.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
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
                CarID = 2,
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
            Car car3 = new Car
            {
                CarID = 3,
                Author = "user1",
                ApiUser = "user1",
                Brand = "VW",
                Model = "Golf5",
                MobileNumber = "+375339876321",
                CarDescription = new CarDescription
                {
                    Color = "white",
                    FuelType = "gasoline",
                    EngineСapacity = 1.5,
                    YearOfManufacture = 2010
                },
                Price = 7000
            };

            _mockRepository.Setup(m => m.GetCarsAsync()).ReturnsAsync(new Car[] { car1, car2, car3 });            
        }

        private void SetupSuccessFindCar1InMockRepository()
        {
            AddInitialCarCollectionInMockRepository();
            _mockRepository.Setup(m => m.FindCarAsync(1)).ReturnsAsync(_mockRepository.Object.GetCarsAsync().Result.FirstOrDefault(c => c.CarID == 1));
        }

        private void SetupUnsuccessFindCar100InMockRepository()
        {
            _mockRepository.Setup(m => m.FindCarAsync(100)).ReturnsAsync((Car)null);
        }

        [Fact]
        public void Can_Paginate()
        {
            AddInitialCarCollectionInMockRepository();
            SetFakeHttpContext("Test");
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));
            AddInitialCarCollectionInMockRepository();
            SetFakeHttpContext("Test");

            ViewResult result = (ViewResult)_noticeController.List(null, 2, null).Result;

            NoticeListViewModel viewModel = result.ViewData.Model as NoticeListViewModel;
            CarViewModel[] carArray = viewModel.CarViewModels.ToArray();
            Assert.True(carArray.Length == 1);
            Assert.Equal("VW", carArray[0].Brand);
        }

        [Fact]
        public void Can_Send_Pagination_View_Model()
        {
            AddInitialCarCollectionInMockRepository();
            SetFakeHttpContext("Test");
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            ViewResult result = (ViewResult)_noticeController.List(null, 2, null).Result;

            NoticeListViewModel viewModel = result.ViewData.Model as NoticeListViewModel;
            PagingInfo pageInfo = viewModel.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(2, pageInfo.ItemsPerPage);
            Assert.Equal(3, pageInfo.TotalItems);
            Assert.Equal(2, pageInfo.TotalPages);
        }

        [Fact]
        public void Can_Filter_Cars()
        {
            AddInitialCarCollectionInMockRepository();
            SetFakeHttpContext("Test");
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            ViewResult result = (ViewResult)_noticeController.List("Audi", 1, null).Result;

            NoticeListViewModel viewModel = result.ViewData.Model as NoticeListViewModel;
            CarViewModel[] carArray = viewModel.CarViewModels.ToArray();
            Assert.True(carArray.Length == 1);
            Assert.Equal("Audi", carArray[0].Brand);
        }

        [Fact]
        public void Generate_Category_Specific_Product_Count()
        {
            AddInitialCarCollectionInMockRepository();
            SetFakeHttpContext("Test");
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            Func<ViewResult, NoticeListViewModel> GetModel = (result => result.ViewData.Model as NoticeListViewModel);
            int? res1 = GetModel((ViewResult)_noticeController.List("Audi", 1, null).Result)?.PagingInfo.TotalItems;
            int? res2 = GetModel((ViewResult)_noticeController.List("BMW", 1, null).Result)?.PagingInfo.TotalItems;
            int? res3 = GetModel((ViewResult)_noticeController.List("VW", 1, null).Result)?.PagingInfo.TotalItems;
            int? resAll = GetModel((ViewResult)_noticeController.List(null, 1, null).Result)?.PagingInfo.TotalItems;

            Assert.Equal(1, res1);
            Assert.Equal(1, res2);
            Assert.Equal(1, res3);
            Assert.Equal(3, resAll);
        }

        [Fact]
        public void Can_Delete_Car()
        {
            AddInitialCarCollectionInMockRepository();
            SetupSuccessFindCar1InMockRepository();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Success()));

            IActionResult result = _noticeController.RemoveItem(1).Result;

            _mockRepository.Verify(m => m.DeleteCarAsync(1));
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void Cannot_Delete_If_Not_Car_Author()
        {
            SetupSuccessFindCar1InMockRepository();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            StatusCodeResult result = (StatusCodeResult)_noticeController.RemoveItem(1).Result;

            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public void Cannot_Delete_If_Not_Existing_Car()
        {
            SetupUnsuccessFindCar100InMockRepository();
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            StatusCodeResult result = (StatusCodeResult)_noticeController.RemoveItem(1).Result;

            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void Can_Edit_Car()
        {
            SetupSuccessFindCar1InMockRepository();
            CarViewModel carViewModelWIthNewProperties = new CarViewModel
            {
                CarID = 1,
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
            _mockRepository.Setup(r => r.EditCarAsync(It.IsAny<Car>())).Callback<Car>((c) => resultCar = c);
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
              It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Success()));

            IActionResult result = _noticeController.EditItem(carViewModelWIthNewProperties).Result;

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(carViewModelWIthNewProperties.Brand, resultCar.Brand);
            Assert.Equal(carViewModelWIthNewProperties.Model, resultCar.Model);
            Assert.Equal(carViewModelWIthNewProperties.MobileNumber, resultCar.MobileNumber);
            Assert.Equal(carViewModelWIthNewProperties.Price, resultCar.Price);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.Color, resultCar.CarDescription.Color);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.FuelType, resultCar.CarDescription.FuelType);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.EngineСapacity, resultCar.CarDescription.EngineСapacity);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.YearOfManufacture, resultCar.CarDescription.YearOfManufacture);
        }

        [Fact]
        public void Can_Add_New_Car_If_CarID_Zero()
        {
            string currentUserName = "Test";
            SetFakeHttpContext(currentUserName);
            SetupSuccessFindCar1InMockRepository();
            CarViewModel carViewModelWIthNewProperties = new CarViewModel
            {
                CarID = 0,
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
            _mockRepository.Setup(r => r.AddCarAsync(It.IsAny<Car>())).Callback<Car>((c) => resultCar = c).ReturnsAsync(new Car());
            _mockAuthService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
              It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Success()));

            IActionResult result = _noticeController.EditItem(carViewModelWIthNewProperties).Result;

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(currentUserName, resultCar.Author);
            Assert.Equal(carViewModelWIthNewProperties.Brand, resultCar.Brand);
            Assert.Equal(carViewModelWIthNewProperties.Model, resultCar.Model);
            Assert.Equal(carViewModelWIthNewProperties.MobileNumber, resultCar.MobileNumber);
            Assert.Equal(carViewModelWIthNewProperties.Price, resultCar.Price);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.Color, resultCar.CarDescription.Color);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.FuelType, resultCar.CarDescription.FuelType);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.EngineСapacity, resultCar.CarDescription.EngineСapacity);
            Assert.Equal(carViewModelWIthNewProperties.CarDescription.YearOfManufacture, resultCar.CarDescription.YearOfManufacture);
        }
    }
}
