using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CarStoreWeb.Models
{
    public class RemoteCarRepository : ICarRepository
    {
        RestClient _client;
        public RemoteCarRepository(string url)
        {
            _client = new RestClient(url);
        }       
        private IRestResponse SendRequest(Method method, int? carID=null, object obj=null)
        {
            var request= new RestRequest(method);
            
            if (carID.HasValue)
            {
                request.Resource = "{carID}";
                request.AddUrlSegment("carID", carID.Value.ToString());
            }

            request.AddHeader("Accept", "application/json");

            if (obj != null)
                request.AddJsonBody(obj);

            IRestResponse response = _client.Execute(request);
            
            return response;
        }

        public IEnumerable<Car> Cars
        {
            get
            {
                IRestResponse restResponse = SendRequest(Method.GET);
                if (!restResponse.IsSuccessful)
                {
                    throw new Exception($"Some Error Occured {restResponse.Content}" +
                        $"{restResponse.StatusDescription}");
                }
                return JsonConvert.DeserializeObject<IEnumerable<Car>>(restResponse.Content);
            }
        }
    
        public Car AddCar(Car car)
        {
            IRestResponse restResponse = SendRequest(Method.POST, null, new Car{
                Brand = car.Brand,
                Model = car.Model,
                CarDescription = car.CarDescription,
                Price = car.Price
            });
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }

        public Car DeleteCar(int carID)
        {
            IRestResponse restResponse = SendRequest(Method.DELETE, carID);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }

        public void EditCar(Car car) //
        {
            IRestResponse restResponse = SendRequest(Method.PUT, null, car);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
        }
        public Car FindCar(int carID)
        {
            IRestResponse restResponse = SendRequest(Method.GET, carID);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }
    }
}
