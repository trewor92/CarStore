using CarStoreRest.Models.ApiModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarStoreRest.Models
{
    public interface ICarRepository
    {
        Task<List<Car>> GetCarsListAsync();
        Task<Car> FindCarAsync(int carID);
        Task<Car> AddCarAsync(Car car);
        Task EditCarAsync(Car car, int carID);
        Task<Car> DeleteCarAsync(int carID);
    }
}
