using CarStoreRest.Models.ApiModels;
using System.Collections.Generic;

namespace CarStoreRest.Models
{
    public interface ICarRepository
    {
        IEnumerable<Car> Cars { get; }
        Car FindCar(int carID);
        Car AddCar(Car car);
        void EditCar(Car car, int carID);
        Car DeleteCar(int carID);
    }
}
