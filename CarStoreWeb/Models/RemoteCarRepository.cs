using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;

namespace CarStoreWeb.Models
{
    public class RemoteCarRepository : ICarRepository
    {
        RestClient _client;
        private ITokenAuthenticator _authenticator;
        
        public RemoteCarRepository(string url, ITokenAuthenticator authenticator)
        {
            _client = new RestClient(url)
            {
                Authenticator = authenticator,
                Timeout = 60000
            };
            _authenticator = authenticator;
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

            var tokenExpired = 
                Convert.ToBoolean(response.Headers.FirstOrDefault(x => x.Name == "Token-Expired")?.Value ?? false);

            if (!response.IsSuccessful && tokenExpired)
            {
                _authenticator.RefreshTokens();    
                response = SendRequest(method, carID, obj);
            }
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
            IRestResponse restResponse = SendRequest(Method.POST, null, car); 
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
            IRestResponse restResponse = SendRequest(Method.PUT, car.CarID, car);
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
