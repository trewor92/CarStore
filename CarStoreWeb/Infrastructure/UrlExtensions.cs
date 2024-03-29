﻿using Microsoft.AspNetCore.Http;

namespace CarStoreWeb.Infrastructure
{
    public static class UrlExtensions
    {
        public static string PathAndQuery(this HttpRequest request)
        {
            string res = request.QueryString.HasValue ? $"{request.Path}{request.QueryString}" :
            request.Path.ToString();
            return res;
        }
    }
}
