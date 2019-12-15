using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.InMemory;
using CarStoreRest.Models;
using AutoMapper;
using CarStoreRest.Tests.ControllersTests;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Tests.ModelsTests
{
    public class EFCarRepositoryTests
    {
        private EFCarRepository _fakeCarRepository = null;
        private ApplicationDbContext _context;
        private IMapper _mapper;

        public EFCarRepositoryTests()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "CarFakeDatabase")
            .Options;
            _context = new ApplicationDbContext(options);
            MapperConfiguration mapperConf = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TestAutoMapperProfile());
            });
            _mapper = mapperConf.CreateMapper();
            _fakeCarRepository = new EFCarRepository(_context, _mapper);
            SetSeedData(_context);
        }
        private void RemoveAllData(ApplicationDbContext context)
        {
            _context.Database.EnsureDeleted();
        }

        private void SetSeedData(ApplicationDbContext context)
        {
            RemoveAllData(context);
            context.Cars.AddRange(
               new Car
               {
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
               },
               new Car
               {
                   Author = "Admin",
                   ApiUser = "user2",
                   Brand = "Toyota",
                   Model = "Prius",
                   MobileNumber = "+375331234567",
                   CarDescription = new CarDescription
                   {
                       Color = "white",
                       FuelType = "gasoline",
                       EngineСapacity = 1.5,
                       YearOfManufacture = 2007
                   },
                   Price = 10000
               },
               new Car
               {
                   Author = "Admin",
                   ApiUser = "user1",
                   Brand = "Audi",
                   Model = "A4",
                   MobileNumber = "+375331234567",
                   CarDescription = new CarDescription
                   {
                       Color = "blue",
                       FuelType = "gasoline",
                       EngineСapacity = 1.6,
                       YearOfManufacture = 2003
                   },
                   Price = 8000
               });
            context.SaveChanges();
        }

        [Fact]
        public void Can_Return_Cars()
        {
            IEnumerable<Car> result = _fakeCarRepository.GetCarsListAsync().Result;
           
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void Can_Return_Car()
        {
            Car result1 = _fakeCarRepository.FindCarAsync(1).Result;
            Car result2 = _fakeCarRepository.FindCarAsync(2).Result;
            Car result3 = _fakeCarRepository.FindCarAsync(3).Result;

            Assert.Equal(1, result1.CarID);
            Assert.Equal(2, result2.CarID);
            Assert.Equal(3, result3.CarID);
        }

        [Fact]
        public void Can_Add_Car()
        {
            Car newCar = new Car
            {
                Author = "Admin",
                ApiUser = "user1",
                Brand = "BMW",
                Model = "740",
                MobileNumber = "+375331234567",
                CarDescription = new CarDescription
                {
                    Color = "blue",
                    FuelType = "gasoline",
                    EngineСapacity = 3,
                    YearOfManufacture = 2003
                },
                Price = 15000
            };

            Car addedCar =  _fakeCarRepository.AddCarAsync(newCar).Result;

            Assert.True(addedCar.CarID != 0);
            Assert.True(_context.Cars.Count(c => c.CarID == addedCar.CarID) == 1);
        }

        [Fact]
        public async Task Can_Edit_Car()
        {
            Car forEditCar = new Car
            {
                Author = "User",
                ApiUser = "user3",
                Brand = "Lada",
                Model = "Vesta",
                MobileNumber = "+375331111111",
                CarDescription = new CarDescription
                {
                    Color = "pink",
                    FuelType = "gas",
                    EngineСapacity = 1.3,
                    YearOfManufacture = 2019
                },
                Price = 12000
            };

            await _fakeCarRepository.EditCarAsync(forEditCar, 1);
            Car editedCar = _context.Cars.FirstOrDefault(c => c.CarID == 1);

            Assert.Equal(forEditCar.Brand, editedCar.Brand);
            Assert.Equal(forEditCar.Model, editedCar.Model);
            Assert.Equal(forEditCar.Author, editedCar.Author);
            Assert.Equal(forEditCar.ApiUser, editedCar.ApiUser);
            Assert.Equal(forEditCar.MobileNumber, editedCar.MobileNumber);
            Assert.Equal(forEditCar.Price, editedCar.Price);
            Assert.Equal(forEditCar.CarDescription.Color, editedCar.CarDescription.Color);
            Assert.Equal(forEditCar.CarDescription.EngineСapacity, editedCar.CarDescription.EngineСapacity);
            Assert.Equal(forEditCar.CarDescription.FuelType, editedCar.CarDescription.FuelType);
            Assert.Equal(forEditCar.CarDescription.YearOfManufacture, editedCar.CarDescription.YearOfManufacture);
        }

        [Fact]
        public async Task Can_Delete_Car()
        {
            await _fakeCarRepository.DeleteCarAsync(1);

            Assert.True(_context.Cars.Count(c => c.CarID == 1) == 0);
        }

    }
}
