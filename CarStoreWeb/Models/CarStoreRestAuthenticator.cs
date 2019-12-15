using CarStoreWeb.Models.ViewModels.UserViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace CarStoreWeb.Models
{
    public sealed class CarStoreRestAuthenticator : ITokenAuthenticator
    {
        private ApiLoginModel _apiLoginModel { get; set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        private readonly string _loginUrl;   
        private readonly string _refreshUrl;  

        public CarStoreRestAuthenticator(string consumerName, string consumerPassword, string loginUrl, string refreshUrl)
        {
            _apiLoginModel = new ApiLoginModel { Name = consumerName, Password = consumerPassword };
            _loginUrl = loginUrl;
            _refreshUrl = refreshUrl;
        }

        private WebRequest CreateRequestWithObject(object obj, string url)
        {
            string jsonString = JsonConvert.SerializeObject(obj);
            byte[] objBytes = Encoding.UTF8.GetBytes(jsonString);
            WebRequest request = (WebRequest)HttpWebRequest.Create(new Uri(url, UriKind.RelativeOrAbsolute));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = objBytes.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(objBytes, 0, objBytes.Length);
            }

            return request;
        }

        private WebRequest CreateRefreshRequest()
        {
            return CreateRequestWithObject(new { accessToken = AccessToken, refreshToken = RefreshToken },  _refreshUrl);
        }
        private WebRequest CreateAuthenticationRequest()
        {
            return CreateRequestWithObject(_apiLoginModel, _loginUrl);
        }

        public void RefreshTokens()
        {
            GetTokens(true);
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (AccessToken == null || AccessToken.Length == 0)
            {
                GetTokens();
            }
            request.AddHeader("Authorization", $"Bearer {AccessToken.ToString()}");
        }
        private void GetTokens(bool IsRefresh=false)
        {

            WebRequest authRequest = IsRefresh? CreateRefreshRequest():CreateAuthenticationRequest();
            try
            {
                using (WebResponse response = (WebResponse)authRequest.GetResponse())
                { 
                    var authResult = GetAuthResponse(response);
                    string accessToken = authResult?.accessToken?.Value.ToString();
                    string refreshToken = authResult?.refreshToken?.Value.ToString();

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        throw new InvalidOperationException("RECEIVED_ACCESS_TOKEN_IS_NULL_OR_EMPTY");
                    }
                    if (string.IsNullOrEmpty(refreshToken))
                    {
                        throw new InvalidOperationException("RECEIVED_REFRESH_TOKEN_IS_NULL_OR_EMPTY");
                    }
                    AccessToken = accessToken;
                    RefreshToken = refreshToken;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (response != null && response.StatusCode != HttpStatusCode.InternalServerError)
                {
                    using (Stream data = response.GetResponseStream())
                    {
                        var errorResult = GetObjectFromResponse<dynamic>(data);
                        if (errorResult == null)
                        {
                            throw;
                        }
                        throw new Exception(errorResult);
                    }
                }
                throw;
            }
        }

        private static dynamic GetAuthResponse(WebResponse response)
        {
            using (Stream data = response.GetResponseStream())
            {
                if (data == null)
                {
                    throw new InvalidOperationException("RESPONSE_STREAM_IS_NULL");
                }
                return GetObjectFromResponse<dynamic>(data);
            }
        }
        
        private static T GetObjectFromResponse<T>(Stream data)
        {
            using (StreamReader reader = new StreamReader(data))
            {
                JsonSerializer serialiser = new JsonSerializer();
                using (JsonTextReader jsonTextReader = new JsonTextReader(reader))
                {
                    return serialiser.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }
}

