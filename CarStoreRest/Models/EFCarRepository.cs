using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public class EFCarRepository : ICarRepository
    {
        private ApplicationDbContext _context;
        public EFCarRepository(ApplicationDbContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<Car> Cars => _context.Cars;
        public Car FindCar(int carID)
        {
            return _context.Cars
                .FirstOrDefault(c => c.CarID == carID);
        }

        public Car AddCar(Car car)
        {
            Car newCar = new Car
            {
                Brand = car.Brand,
                Model = car.Model,
                CarDescription = car.CarDescription,
                Price = car.Price,
                Author=car.Author 
            };
            _context.Cars.Add(newCar);
            _context.SaveChanges();
            return newCar;
        }
        public void EditCar(Car car)
        {
            Car currentCar = FindCar(car.CarID);
            if (currentCar!=null)
            {
                currentCar.Brand = car.Brand;
                currentCar.Model = car.Model;
                currentCar.CarDescription = car.CarDescription;
                currentCar.Price = car.Price;
            }
            _context.SaveChanges();
        }
        public Car DeleteCar(int carID)
        {
            Car currentCar = _context.Cars
                    .FirstOrDefault(c => c.CarID == carID);
            if (currentCar != null)
            {
                _context.Cars.Remove(currentCar);
                _context.SaveChanges();
            }
            return currentCar;
        }
    }
}
