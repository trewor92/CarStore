using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetCarsAsync();
        Task<Car> FindCarAsync(int carID);
        Task<Car> AddCarAsync(Car car);
        Task EditCarAsync(Car car);
        Task<Car> DeleteCarAsync(int carID);

    }
}
