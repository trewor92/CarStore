using AutoMapper;
using CarStoreRest.Models.ApiModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Models
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

        public async Task<List<Car>> GetCarsListAsync()
        {
            return  await _context.Cars.ToListAsync<Car>();
        }
       
        public async Task<Car> FindCarAsync(int carID)
        {
            return await _context.Cars
                .FirstOrDefaultAsync(c => c.CarID == carID);
        }
        public async Task<Car> AddCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }
        public async Task EditCarAsync(Car car, int carID)
        {
            Car currentCar =await  FindCarAsync(carID);
            if (currentCar!=null)
            {
                _mapper.Map<Car, Car>(car, currentCar);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<Car> DeleteCarAsync(int carID)
        {
            Car currentCar = await FindCarAsync(carID);
            if (currentCar != null)
            {
                _context.Cars.Remove(currentCar);
                await _context.SaveChangesAsync();
            }
            return currentCar;
        }
    }
}
