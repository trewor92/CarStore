using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private async Task<IRestResponse> SendRequestAsync(Method method, int? carID=null, object obj=null)
        {
            RestRequest request = new RestRequest(method);
            
            if (carID.HasValue)
            {
                request.Resource = "{carID}";
                request.AddUrlSegment("carID", carID.Value.ToString());
            }

            request.AddHeader("Accept", "application/json");

            if (obj != null)
                request.AddJsonBody(obj);

            var response = await  _client.ExecuteTaskAsync(request);

            bool tokenExpired = 
                Convert.ToBoolean(response.Headers.FirstOrDefault(x => x.Name == "Token-Expired")?.Value ?? false);

            if (!response.IsSuccessful && tokenExpired)
            {
                _authenticator.RefreshTokens();    
                response = await SendRequestAsync(method, carID, obj);
            }
            return response;
        }
        
        public async Task<IEnumerable<Car>> GetCarsAsync()
        {
            IRestResponse restResponse = await SendRequestAsync(Method.GET);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<IEnumerable<Car>>(restResponse.Content);
        }
    
        public async Task<Car> AddCarAsync(Car car)
        {
            IRestResponse restResponse = await SendRequestAsync(Method.POST, null, car); 
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }

        public async Task<Car> DeleteCarAsync(int carID)
        {
            IRestResponse restResponse = await SendRequestAsync(Method.DELETE, carID);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }

        public async Task EditCarAsync(Car car) 
        {
            IRestResponse restResponse = await SendRequestAsync(Method.PUT, car.CarID, car);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
        }
        public async Task<Car> FindCarAsync(int carID)
        {
            IRestResponse restResponse = await SendRequestAsync(Method.GET, carID);
            if (!restResponse.IsSuccessful)
            {
                throw new Exception($"Some Error Occured {restResponse.Content}" +
                    $"{restResponse.StatusDescription}");
            }
            return JsonConvert.DeserializeObject<Car>(restResponse.Content);
        }
    }
}
