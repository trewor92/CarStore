using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public class RemoteCarRepository : ICarRepository
    {
        public IEnumerable<Car> Cars
        {
            get
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers["Accept"] = "application/json";
                    string returnedString = client.DownloadString(new Uri("https://localhost:44379/api/car"));
                    return JsonConvert.DeserializeObject<IEnumerable<Car>>(returnedString);
                }
            }
        }
    
        public Car AddCar(Car car)
        {
            throw new NotImplementedException();
        }

        public Car DeleteCar(int carID)
        {
            throw new NotImplementedException();
        }

        public void EditCar(Car car)
        {
            throw new NotImplementedException();
        }

        public Car FindCar(int carID)
        {
            throw new NotImplementedException();
        }
    }
}
