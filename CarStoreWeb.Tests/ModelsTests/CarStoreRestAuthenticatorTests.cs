using CarStoreWeb.Models;
using CarStoreWeb.Models.ViewModels.UserViewModels;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xunit;


namespace CarStoreWeb.Tests.ModelsTests
{
    public class CarStoreRestAuthenticatorTests
    {
        CarStoreRestAuthenticator _restAuthenticator;
        string _consumerName;
        string _consumerPassword; 

        public CarStoreRestAuthenticatorTests()
        {
            _consumerName = "Test";
            _consumerPassword = "Static123$";
            string loginUrl = "test://";
            string refreshUrl = "test://";
            _restAuthenticator = new CarStoreRestAuthenticator(_consumerName, _consumerPassword, loginUrl, refreshUrl);  
        }

        [Fact]
        public void Can_Get_Tokens_And_Add_In_Header()
        {
            WebRequest.RegisterPrefix("test", new TestWebRequestCreate());
            RestClient client = new RestClient("test://");
            RestRequest request = new RestRequest(Method.GET);
            string response =  JsonConvert.SerializeObject(new { accessToken= "accessTest", refreshToken= "refreshTest" });
            TestWebRequest webRequest = TestWebRequestCreate.CreateTestRequest(response);

            _restAuthenticator.Authenticate(client, request);
            ApiLoginModel apiLoginModel = JsonConvert.DeserializeObject<ApiLoginModel>(webRequest.ContentAsString());

            Assert.True(request.Parameters[0].Name == "Authorization");
            Assert.True((string)request.Parameters[0].Value == "Bearer accessTest");
            Assert.True(request.Parameters[0].Type == ParameterType.HttpHeader);
            Assert.True(_restAuthenticator.AccessToken == "accessTest");
            Assert.True(_restAuthenticator.RefreshToken == "refreshTest");
            Assert.Equal(_consumerName, apiLoginModel.Name);
            Assert.Equal(_consumerPassword, apiLoginModel.Password);
        }

        [Fact]
        public void Can_Refresh_Tokens()
        {
            WebRequest.RegisterPrefix("test", new TestWebRequestCreate());
            string response = JsonConvert.SerializeObject(new { accessToken = "newAccessTest", refreshToken = "newRefreshTest" });
            TestWebRequest request = TestWebRequestCreate.CreateTestRequest(response);

            _restAuthenticator.RefreshTokens();

            Assert.True(_restAuthenticator.AccessToken == "newAccessTest");
            Assert.True(_restAuthenticator.RefreshToken == "newRefreshTest");
        }
    }

    public class TestWebRequestCreate : IWebRequestCreate
    {
        private static WebRequest _nextRequest;
        private static object _lockObject = new object();
        static public WebRequest NextRequest
        {
            get { return _nextRequest; }
            set
            {
                lock (_lockObject)
                {
                    _nextRequest = value;
                }
            }
        }
        public WebRequest Create(Uri uri)
        {
            return _nextRequest;
        }
        public static TestWebRequest CreateTestRequest(string response)
        {
            TestWebRequest request = new TestWebRequest(response);
            NextRequest = request;
            return request;
        }
    }

    public class TestWebRequest : WebRequest
    {
        MemoryStream _requestStream = new MemoryStream();
        MemoryStream _responseStream;

        public override string Method { get; set; }
        public override string ContentType { get; set; }
        public override long ContentLength { get; set; }

        public TestWebRequest(string response)
        {
            _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response));
        }

        public string ContentAsString()
        {
            return System.Text.Encoding.UTF8.GetString(_requestStream.ToArray());
        }

        public override Stream GetRequestStream()
        {
            return _requestStream;
        }

        public override WebResponse GetResponse()
        {
            return new TestWebReponse(_responseStream);
        }
    }

    public class TestWebReponse : WebResponse
    {
        Stream _responseStream;

        public TestWebReponse(Stream responseStream)
        {
            _responseStream = responseStream;
        }

        public override Stream GetResponseStream()
        {
            return _responseStream;
        }
    }
}
