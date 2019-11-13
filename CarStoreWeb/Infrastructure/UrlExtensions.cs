using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace CarStoreWeb.Infrastructure
{
    public static class UrlExtensions
    {
        public static string PathAndQuery(this HttpRequest request)
        {
            var res = request.QueryString.HasValue ? $"{request.Path}{request.QueryString}" :
            request.Path.ToString();
            return res;
        }
    }
}
