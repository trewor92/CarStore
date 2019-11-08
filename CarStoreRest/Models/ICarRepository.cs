using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarStoreRest.Models
{
    public interface ICarRepository
    {
        IEnumerable<Car> Cars { get; }
        Car FindCar(int carID);
        Car AddCar(Car car);
        void EditCar(Car car);
        Car DeleteCar(int carID);
    }
}
