using AutoMapper;
using CarStoreRest.Models.ApiModels;
using System.Collections.Generic;
using System.Linq;

namespace CarStoreWeb.Models
{
    public class EFCarRepository : ICarRepository
    {
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public EFCarRepository(ApplicationDbContext ctx, IMapper mapper)
        {
            _context = ctx;
            _mapper = mapper;
        }

        public IEnumerable<Car> Cars => _context.Cars;
        public Car FindCar(int carID)
        {
            return _context.Cars
                .FirstOrDefault(c => c.CarID == carID);
        }
        public Car AddCar(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
            return car;
        }
        public void EditCar(Car car, int carID)
        {
            Car currentCar = FindCar(carID);
            if (currentCar!=null)
            {
                _mapper.Map<Car, Car>(car, currentCar);
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
